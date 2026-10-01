using DressRental.Application.Interfaces.Repositories;
using DressRental.Domain.Entities;
using DressRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DressRental.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EnchantedRentalContext _context;

        public UserRepository(EnchantedRentalContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user, CustomerProfile? profile, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);

            if (profile != null)
            {
                profile.User = user;
                await _context.CustomerProfiles.AddAsync(profile, cancellationToken);
            }
        }

        public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public async Task<User?> GetByIdWithProfileAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
        }

        
    }
}
