using DressRental.Application.DTOs.Auth;

namespace DressRental.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterCustomerAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<UserProfileDto?> GetUserProfileAsync(long userId, CancellationToken cancellationToken = default);
    }
}