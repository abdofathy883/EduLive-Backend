using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await _paymentRepo.AddAsync(payment);
            await _paymentRepo.SaveAllAsync();
        }
    }
}
