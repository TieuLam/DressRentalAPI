using DressRental.Application.DTOs.Products;
using DressRental.Domain.Entities;

namespace DressRental.Application.Services
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductDto>> GetProductsAsync(long? categoryId, string? color, string? size, CancellationToken cancellationToken = default);
        public Task<ProductDetailDto?> GetProductByIdAsync(long itemId, CancellationToken cancellationToken = default);
    }
}
