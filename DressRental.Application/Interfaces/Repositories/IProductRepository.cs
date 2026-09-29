using DressRental.Domain.Entities;

namespace DressRental.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        // Lấy danh sách sản phẩm dựa trên các tiêu chí lọc
        Task<IEnumerable<Product>> GetProductsAsync(long? categoryId, string? color, string? size, CancellationToken cancellationToken = default);

        // Lấy thông tin chi tiết của một sản phẩm
        Task<Product?> GetProductByIdAsync(long itemId, CancellationToken cancellationToken = default);

        // Thêm một sản phẩm mới
        Task AddProductAsync(Product product, CancellationToken cancellationToken = default);

        // Cập nhật thông tin của một sản phẩm hiện có
        Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);

    }
}
