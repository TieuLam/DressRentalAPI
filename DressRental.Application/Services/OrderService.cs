using DressRental.Application.DTOs.Orders;
using DressRental.Application.Interfaces.Repositories;
using DressRental.Domain.Entities;
using DressRental.Domain.Enums;
using DressRental.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace DressRental.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CheckAvailabilityResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request, CancellationToken cancellationToken = default)
        {
            if (request.StartDate > request.EndDate)
            {
                throw new ValidationException("Ngày trả đồ không được trước ngày nhận đồ");
            }
            var isAvailable = await _orderRepository.IsItemAvailableAsync(request.ItemId, request.StartDate, request.EndDate, cancellationToken);

            return new CheckAvailabilityResponse
            {
                ItemId = request.ItemId,
                IsAvailable = isAvailable,
                Message = isAvailable ? "Sản phẩm có sẵn trong khoảng thời gian này." : "Sản phẩm đã được cho thuê trong khoảng thời gian này."
            };
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            if (request.StartDate > request.EndDate)
            {
                throw new DomainException("Ngày trả đồ không được trước ngày nhận đồ");
            }
            if (request.OrderDetails == null || !request.OrderDetails.Any())
            {
                throw new DomainException("Đơn hàng phải có ít nhất một sản phẩm.");
            }

            CustomerProfile customer;
            if (request.CustomerId.HasValue && request.CustomerId.Value > 0)
            {
                customer = await _orderRepository.GetCustomerByIdAsync(request.CustomerId.Value, cancellationToken)
                    ?? throw new DomainException($"Không tìm thấy khách hàng với ID #{request.CustomerId.Value}.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.CustomerName) || string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    throw new DomainException("Vui lòng nhập họ tên và số điện thoại của người thuê.");
                }
                customer = await _orderRepository.GetOrCreateCustomerProfileAsync(
                    request.CustomerName.Trim(),
                    request.PhoneNumber.Trim(),
                    request.Email?.Trim(),
                    request.Address?.Trim(),
                    cancellationToken);
            }

            var order = new Order
            {
                CustomerId = customer.Id,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = nameof(OrderStatus.Pending).ToUpper(),
                CreateAt = DateTime.UtcNow,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in request.OrderDetails)
            {
                var product = await _productRepository.GetProductByIdAsync(item.itemId, cancellationToken);
                if (product == null)
                {
                    throw new DomainException($"Sản phẩm với ID {item.itemId} không tồn tại.");
                }
                var isAvailable = await _orderRepository.IsItemAvailableAsync(item.itemId, request.StartDate, request.EndDate, cancellationToken);
                if (!isAvailable)
                {
                    throw new DoubleBookingException($"ID #{item.itemId}",
                    request.StartDate.ToDateTime(TimeOnly.MinValue),
                    request.EndDate.ToDateTime(TimeOnly.MinValue));
                }
                order.OrderDetails.Add(new OrderDetail
                {
                    ItemId = item.itemId,
                    RentalType = item.RentalType
                });
            }

            await _orderRepository.AddOrderAsync(order, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetOrderByIdAsync(order.Id, cancellationToken) ?? throw new DomainException("Lỗi tạo đơn hàng.");
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                return null;
            }

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                StartDate = order.StartDate,
                EndDate = order.EndDate,
                ActualReturnDate = order.ActualReturnDate,
                TotalAmount = order.TotalAmount,
                TotalDeposit = order.TotalDeposit,
                Status = order.Status,
                CreateAt = order.CreateAt,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ItemId = od.ItemId,
                    RentalType = od.RentalType,
                    ItemCode = od.Item?.ItemCode ?? string.Empty,
                    ItemName = od.Item?.Product.Name ?? string.Empty,
                    Size = od.Item?.Size ?? string.Empty,
                    Price = od.Price,
                    Deposit = od.DepositAmount
                }).ToList()
            };
        }

        public async Task<bool> ConfirmDepositAsync(long orderId, long amount, string paymentMethod, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);
            if (order == null) throw new DomainException("Không tìm thấy đơn hàng.");
            order.Payments.Add(new Payment
            {
                OrderId = orderId,
                PaymentMethod = paymentMethod,
                Amount = amount,
                PaymentType = nameof(PaymentType.Deposit).ToUpper(),
                PaidAt = DateTime.UtcNow
            });
            order.Status = nameof(OrderStatus.Confirmed).ToUpper();

            await _orderRepository.UpdateOrderAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
