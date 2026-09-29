using DressRental.Application.DTOs.Products;
using DressRental.Application.Interfaces.Repositories;
using DressRental.Domain.Entities;


namespace DressRental.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDetailDto?> GetProductByIdAsync(long itemId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetProductByIdAsync(itemId, cancellationToken);
            if (product == null)
            {
                return null;
            }
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                OriginalPrice = product.OriginalPrice,
                Price3Days = product.Price3Days,
                Price7Days = product.Price7Days,
                CategoryName = product.Category.Name,
                DepositRate = product.DepositRate,
                Description = product.Description,
                Color = product.Color,
                ThumbnailImageUrl = product.ProductImages.FirstOrDefault(pi => pi.IsThumbnail)?.ImageUrl,
                ImageUrls = product.ProductImages.OrderBy(img => img.DisplayOrder).Select(img => img.ImageUrl).ToList(),
                AvailableItems = product.ProductItems.Select(pi => new ProductItemDto
                {
                    Id = pi.Id,
                    Size = pi.Size,
                    ItemCode = pi.ItemCode,
                    Status = pi.Status.ToString(),
                    CurrentCondition = pi.CurrentCondition.ToString(),
                }).ToList()
            };
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(long? categoryId, string? color, string? size, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetProductsAsync(categoryId, color, size, cancellationToken);
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                OriginalPrice = p.OriginalPrice,
                Price3Days = p.Price3Days,
                Price7Days = p.Price7Days,
                CategoryName = p.Category.Name,
                DepositRate = p.DepositRate,
                Description = p.Description,
                Color = p.Color,
                ThumbnailImageUrl = p.ProductImages.FirstOrDefault(pi => pi.IsThumbnail)?.ImageUrl
            }).ToList();
        }
    }
}
