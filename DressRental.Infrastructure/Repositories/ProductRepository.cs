using DressRental.Application.Interfaces.Repositories;
using DressRental.Domain.Entities;
using DressRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DressRental.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EnchantedRentalContext _dbContext;

        public ProductRepository(EnchantedRentalContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(product, cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(long itemId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == itemId, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(long? categoryId, string? color, string? size, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductImages)
                        .AsQueryable();
            
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(p => p.Color == color);
            }

            if (!string.IsNullOrEmpty(size))
            {
                query = query.Where(p => p.ProductItems.Any(pi => pi.Size == size));
            }

            return await query.ToListAsync(cancellationToken);
        }

        public Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            _dbContext.Products.Update(product);
            return Task.CompletedTask;
        }
    }
}
