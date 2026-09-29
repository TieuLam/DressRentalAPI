namespace DressRental.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        // Lưu các thay đổi vào cơ sở dữ liệu
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
