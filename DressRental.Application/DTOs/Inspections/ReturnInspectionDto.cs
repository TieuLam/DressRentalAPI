namespace DressRental.Application.DTOs.Inspections
{
    public class ReturnInspectionRequest
    {
        public long OrderDetailId { get; set; }
        public long? InspectUserId { get; set; }
        public long DamageFee { get; set; }
        public long LateFee { get; set; }
        public string? DamageDescription { get; set; }
    }
    public class ReturnInspectionResponse
    {
        public long InspectionId { get; set; }
        public long OrderDetailId { get; set; }
        public long? DamageFee { get; set; }
        public long? LateFee { get; set; }
        public long OriginalDeposit { get; set; }
        public long RefundedDeposit { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
