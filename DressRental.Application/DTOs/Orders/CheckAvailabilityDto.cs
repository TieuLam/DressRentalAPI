namespace DressRental.Application.DTOs.Orders {
    public class CheckAvailabilityRequest
    {
        public long ItemId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }

    public class CheckAvailabilityResponse
    {
        public long ItemId { get; set; }
        public bool IsAvailable { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}