using DressRental.Domain.Entities;

namespace DressRental.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithProfileAsync(long id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CustomerProfile? profile, CancellationToken cancellationToken = default);
    }
}