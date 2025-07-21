namespace Core.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateCheckoutSessionAsync(string studentId, int courseId, decimal amount);
        Task HandlePaymentSuccessAsync(string sessionId);
        //Task<string> CreateSubscriptionSessionAsync(string customerEmail, string priceId, string successURL, string cancelURL);
        //Task<bool> CancelSubscriptionAsync(string subscriptionId);
    }
}
