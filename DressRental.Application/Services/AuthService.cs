using DressRental.Application.DTOs.Auth;
using DressRental.Application.Interfaces.Repositories;
using DressRental.Application.Interfaces.Services;
using DressRental.Domain.Entities;
using DressRental.Domain.Enums;
using DressRental.Domain.Exceptions;

namespace DressRental.Application.Services
{
    internal class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(long userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithProfileAsync(userId, cancellationToken);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role,
                FullName = user.CustomerProfile?.Name ?? user.UserName,
                PhoneNumber = user.CustomerProfile?.PhoneNumber ?? "",
                Email = user.CustomerProfile?.Email,
                Address = user.CustomerProfile?.Address
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            //Tìm tài khoản
            var user = await _userRepository.GetByUserNameAsync(request.UserName, cancellationToken);
            if (user == null || !user.IsActive)
            {
                throw new DomainException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            //Kiểm tra mật khẩu với hash BCrypt
            bool isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.Password);
            if (!isValidPassword)
            {
                throw new DomainException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            string fullName = user.CustomerProfile?.Name ?? user.UserName;

            // Cấp token
            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, fullName);

            return new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Role = user.Role,
                FullName = fullName,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDto> RegisterCustomerAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            //Kiểm tra tài khoản đã tồn tại chưa
            bool exists = await _userRepository.ExistsByUserNameAsync(request.UserName, cancellationToken);
            if (exists)
            {
                throw new DomainException($"Tên đăng nhập '{request.UserName}' đã được sử dụng. Vui lòng chọn tên khác.");
            }

            //Băm mật khẩu bằng BCrypt
            string passwordHash = _passwordHasher.HashPassword(request.Password);

            //Tạo User
            var user = new User
            {
                UserName = request.UserName,
                Password = passwordHash,
                Role = nameof(UserRole.Customer).ToUpper(),
                IsActive = true
            };

            //Tạo CustomerProfile tương ứng
            var profile = new CustomerProfile
            {
                Name = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address
            };

            await _userRepository.AddAsync(user, profile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //Cấp token cho khách sau khi đăng ký thành công
            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, profile.Name);

            return new AuthResponseDto
            {
                Token = token,
                UserName = user.UserName,
                Role = user.Role,
                FullName = profile.Name,
                ExpiresAt = expiresAt
            };
        }
    }
}
