namespace DressRental.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        long? UserId { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
    }
}