using DressRental.Domain.Entities;

namespace DressRental.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        //Lấy thông tin chi tiết của đơn hàng
        Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken = default);

        //Kiểm tra xem một mặt hàng có sẵn trong khoảng thời gian cụ thể hay không
        Task<bool> IsItemAvailableAsync(long itemId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

        //Thêm một đơn hàng mới vào cơ sở dữ liệu
        Task AddOrderAsync(Order order, CancellationToken cancellationToken = default);

        //Cập nhật thông tin của một đơn hàng hiện có
        Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
        Task<CustomerProfile?> GetCustomerByIdAsync(long customerId, CancellationToken cancellationToken = default);
        Task<CustomerProfile> GetOrCreateCustomerProfileAsync(string name, string phoneNumber, string? email, string? address, CancellationToken cancellationToken = default);
    }
}

