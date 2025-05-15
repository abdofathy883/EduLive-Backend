using Core.Enums;
using Core.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subscription = Core.Models.Subscription;

namespace Core.Interfaces
{
    public interface IStripeService
    {
        //Task<string> CreateCustomerAsync(BaseUser user);
        //Task UpdateCustomerAsync(string customerId, BaseUser user);

        // Product Management
        Task<(string ProductId, string PriceId)> CreateProductAsync(string name, string description, decimal price, bool isSubscription = false, BillingInterval interval = BillingInterval.Monthly);
        Task UpdateProductPriceAsync(string productId, decimal newPrice);

        // Payment Processing
        Task<PaymentIntent> CreatePaymentIntentAsync(string customerId, decimal amount, string courseId);
        Task<string> CreateCheckoutSessionForCourseAsync(string customerId, string priceId, int courseId);
        Task<string> CreateCheckoutSessionForSubscriptionAsync(string customerId, string priceId);

        // Subscription Management
        Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId);
        Task CancelSubscriptionAsync(string subscriptionId);

        // Connect Account for Instructors
        Task<string> CreateConnectAccountAsync(InstructorUser instructor);
        Task<string> CreateConnectAccountOnboardingLinkAsync(string accountId, string returnUrl);

        // Payouts to Instructors
        Task<Transfer> CreateTransferToInstructorAsync(string connectedAccountId, decimal amount, string description);
    }
}
