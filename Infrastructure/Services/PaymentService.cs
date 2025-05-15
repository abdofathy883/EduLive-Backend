using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        string publicKey = "pk_test_51RLJh9QdtujEkXDTBVd00fiQXx5jP83lEWQ51MWXQNz2AMfmCu5bgnQWNWGTXS5BQdujXwrxr8RnjsDh1dz7tDcN00MKHhzYKx";
        string secretKey = "sk_test_51RLJh9QdtujEkXDTmTEE70BRqzWDmxoIUSzYgnr6aCHgNjmpqys6Car4dMdfwgZewL6TYwQcbZwhHn41ImHtAx8Y00SmyFsmhX";

        public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
        {
            var service = new Stripe.SubscriptionService();
            var canceledSubscription = await service.CancelAsync(subscriptionId);
            return canceledSubscription.Status == "canceled";
        }

        public async Task<string> CreateCheckoutSessionAsync(decimal amount, string currency, string successURL, string cancelURL)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "Card" },
                Currency = currency,
                SuccessUrl = successURL,
                CancelUrl = cancelURL,
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new Stripe.Checkout.SessionLineItemOptions
                    {

                    }
                },
                Mode = "Payment"
            };
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = await service.CreateAsync(options);
            return session.Url;
        }

        public async Task<string> CreateSubscriptionSessionAsync(string customerEmail, string priceId, string successURL, string cancelURL)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                Mode = "subscription",
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = customerEmail,
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
        {
            new Stripe.Checkout.SessionLineItemOptions
            {
                Price = priceId, // Pre-created Price ID from Stripe dashboard
                Quantity = 1
            }
        },
                SuccessUrl = successURL,
                CancelUrl = cancelURL
            };

            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }

        public async Task<CheckoutSessionDTO> GetSessionDetailsAsync(string SessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(SessionId);

            return new CheckoutSessionDTO
            {
                Id = int.TryParse(session.Id, out var parsedId) ? parsedId : 0, // Convert string to int safely
                PaymentStatus = session.PaymentStatus,
                CustomerEmail = session.CustomerEmail,
                AmountTotla = session.AmountTotal ?? 0
            };
        }

        public async Task<bool> RefundPaymentAsync(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };
            var refundService = new RefundService();
            var refund = await refundService.CreateAsync(refundOptions);
            return refund.Status == "succeeded";
        }
    }
}
