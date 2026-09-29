namespace DressRental.Application.DTOs.Orders
{
    public class OrderResponseDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly? ActualReturnDate { get; set; }
        public long TotalAmount { get; set; }
        public long TotalDeposit { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
    }

    public class OrderDetailDto
    {
        public long ItemId { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string RentalType { get; set; } = null!;
        public long Price { get; set; }
        public long Deposit { get; set; }
    }
}