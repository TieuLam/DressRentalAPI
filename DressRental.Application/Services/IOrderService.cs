using DressRental.Application.DTOs.Inspections;
using DressRental.Application.DTOs.Orders;

namespace DressRental.Application.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
        Task<OrderResponseDto?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken = default);
        Task<CheckAvailabilityResponse> CheckAvailabilityAsync(CheckAvailabilityRequest request, CancellationToken cancellationToken = default);
        Task<bool> ConfirmDepositAsync(long orderId, long amount, string paymentMethod, CancellationToken cancellationToken = default);
    }
}
