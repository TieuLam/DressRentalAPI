using DressRental.Application.Interfaces.Repositories;
using DressRental.Domain.Entities;
using DressRental.Domain.Enums;
using DressRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DressRental.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EnchantedRentalContext _context;

        public OrderRepository(EnchantedRentalContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            foreach (var orderDetail in order.OrderDetails)
            {
                var item = await _context.ProductItems.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == orderDetail.ItemId, cancellationToken);
                if (item != null)
                {
                    long price = orderDetail.RentalType == "3_DAYS" ? item.Product.Price3Days : item.Product.Price7Days;
                    long deposit = (long)(item.Product.OriginalPrice * (item.Product.DepositRate / 100));
                    
                    orderDetail.Price = price;
                    orderDetail.DepositAmount = deposit;
                    order.TotalAmount += price;
                    order.TotalDeposit += deposit;
                }
            }

            order.CreateAt = DateTime.UtcNow;
            await _context.Orders.AddAsync(order);
        }

        public async Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.Include(o => o.Customer)
                                        .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Item)
                                            .ThenInclude(i => i.Product)
                                        .Include(p => p.Payments)
                                        .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task<bool> IsItemAvailableAsync(long itemId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            //Kiểm tra xem item có tồn tại hoặc bị thanh lý hoặc đang sửa chữa
            var item = _context.ProductItems.FirstOrDefault(i => i.Id == itemId);

            if (item == null || item.Status == nameof(ProductItemStatus.Discarded).ToUpper() || item.Status == nameof(ProductItemStatus.InRepair).ToUpper())
            {
                return false;
            }

            // Kiểm tra xem item có đang được đặt trong khoảng thời gian từ startDate đến endDate không
            var existingOrders = await _context.OrderDetails.Where(od => od.ItemId == itemId)
                .Select(od => od.Order)
                .Where(o => o.Status != nameof(OrderStatus.Cancelled).ToUpper())
                .ToListAsync(cancellationToken);

            foreach (var order in existingOrders)
            {

                //Đơn PENDING quá 12 giờ chưa thanh toán cọc -> Hết hạn giữ chỗ
                if (order.Status == nameof(OrderStatus.Pending).ToUpper() && order.CreateAt.AddHours(12) < DateTime.UtcNow)
                {
                    continue;
                }

                // Cộng 2 ngày đệm giặt hấp sau ngày khách trả đồ (EndDate)
                DateOnly actualAvailableDate = order.EndDate.AddDays(2);

                if (startDate <= order.EndDate && actualAvailableDate >= order.StartDate)
                {
                    return false;
                }
            }

            return true;
        }

        public Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task<CustomerProfile?> GetCustomerByIdAsync(long customerId, CancellationToken cancellationToken = default)
        {
            return await _context.CustomerProfiles.FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
        }

        public async Task<CustomerProfile> GetOrCreateCustomerProfileAsync(string name, string phoneNumber, string? email, string? address, CancellationToken cancellationToken = default)
        {
            // Tận dụng index IX_CustomerProfile_PhoneNumber trong DB để tìm khách quen quay lại
            var customer = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber, cancellationToken);

            if (customer == null)
            {
                customer = new CustomerProfile
                {
                    Name = name,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    Address = address,
                    UserId = null // Khách vãng lai: UserId để NULL theo đúng thiết kế database
                };
                await _context.CustomerProfiles.AddAsync(customer, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken); // Lưu để lấy Customer.Id
            }
            else
            {
                // Cập nhật thông tin mới nhất nếu khách có thay đổi email/địa chỉ
                customer.Name = name;
                if (!string.IsNullOrEmpty(email)) customer.Email = email;
                if (!string.IsNullOrEmpty(address)) customer.Address = address;
            }

            return customer;
        }
    }
}
