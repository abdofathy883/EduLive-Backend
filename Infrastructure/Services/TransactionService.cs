using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class TransactionService : ITransactionsService
    {
        private readonly IGenericRepo<Payment> _paymentRepo;
        public TransactionService(IGenericRepo<Payment> genericRepo)
        {
            _paymentRepo = genericRepo;
        }
        public async Task AddTransactionAsync(Payment payment)
        {
            if (payment is null)
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null");

            await _paymentRepo.AddAsync(payment);
            await _paymentRepo.SaveAllAsync();
        }
    }
}
