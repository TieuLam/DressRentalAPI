using DressRental.Domain.Entities;

namespace DressRental.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user, string fullName);
    }

}