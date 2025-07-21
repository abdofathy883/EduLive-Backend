using Core.Models;
namespace Core.Interfaces
{
    public interface ITransactionsService
    {
        Task AddTransactionAsync(Payment payment);
    }
}
