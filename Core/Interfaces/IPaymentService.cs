using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
        //Task<CheckoutSessionDTO> GetSessionDetailsAsync(string SessionId);
        Task<string> CreateCheckoutSessionAsync(string studentId, int courseId, decimal amount);
        Task HandelPAymentSuccessAsync(string sessionId);
        //Task<string> CreateSubscriptionSessionAsync(string customerEmail, string priceId, string successURL, string cancelURL);
        //Task<bool> CancelSubscriptionAsync(string subscriptionId);
        //Task<bool> RefundPaymentAsync(string paymentIntentId);
    }
}
