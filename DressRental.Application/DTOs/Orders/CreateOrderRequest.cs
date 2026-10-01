namespace DressRental.Application.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public long? CustomerId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<OrderItemRequest> OrderDetails { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        public long itemId { get; set; }
        public string RentalType { get; set; } = "3_DAYS"; // "3_DAYS" hoặc "7_DAYS"
    }
}