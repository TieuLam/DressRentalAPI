namespace DressRental.Domain.Exceptions
{
    public class DoubleBookingException : Exception
    {
        public DoubleBookingException(string itemCode, DateTime startDate, DateTime endDate) 
            : base($"Chiếc váy mã '{itemCode}' đã có người đặt trong khoảng thời gian từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}")
        {
        }
    }
}
