using DressRental.Application.Interfaces.Repositories;
using DressRental.Infrastructure.Data;

namespace DressRental.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EnchantedRentalContext _context;

        public UnitOfWork(EnchantedRentalContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
     
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
