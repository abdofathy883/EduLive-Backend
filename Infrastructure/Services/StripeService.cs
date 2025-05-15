using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration configuration;
        private readonly string apiKey;
        private readonly string webHookSecret;
        public StripeService(IConfiguration _configuration)
        {
            configuration = _configuration;
            apiKey = configuration["Stripe:SecretKey"];
            webHookSecret = configuration["Stripe:WebHookSecret"];
            StripeConfiguration.ApiKey = apiKey;
        }
        public Task CancelSubscriptionAsync(string subscriptionId)
        {
            throw new NotImplementedException();
        }
        //Done-------------------
        public async Task<string> CreateCheckoutSessionForCourseAsync(string customerId, string priceId, int courseId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },
                Mode = "payment",
                Customer = customerId,
                SuccessUrl = $"{configuration["AppUrl"]}/payment/success?courseId={courseId}",
                CancelUrl = $"{configuration["AppUrl"]}/payment/cancel?courseId={courseId}",
                Metadata = new Dictionary<string, string>
                {
                    { "CourseId", courseId.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }
        //Done--------------------
        public async Task<string> CreateCheckoutSessionForSubscriptionAsync(string customerId, string priceId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },
                Mode = "subscription",
                Customer = customerId,
                SuccessUrl = $"{configuration["AppUrl"]}/subscription/success",
                CancelUrl = $"{configuration["AppUrl"]}/subscription/cancel"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public Task<string> CreateConnectAccountAsync(InstructorUser instructor)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateConnectAccountOnboardingLinkAsync(string accountId, string returnUrl)
        {
            throw new NotImplementedException();
        }
        //DONE --------------------------------------
        public async Task<PaymentIntent> CreatePaymentIntentAsync(string customerId, decimal amount, string courseId, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount),
                Currency = currency,
                Customer = customerId,
                Metadata = new Dictionary<string, string>
                {
                    {"CourseId", courseId }
                },
                CaptureMethod = "automatic"
            };
            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public Task<PaymentIntent> CreatePaymentIntentAsync(string customerId, decimal amount, string courseId)
        {
            throw new NotImplementedException();
        }

        //Done--------------------------------------------
        public async Task<(string ProductId, string PriceId)> CreateProductAsync(string name, string description, decimal price,string currency, bool isSubscription = false, BillingInterval interval = BillingInterval.Monthly)
        {
            var producrOptions = new ProductCreateOptions
            {
                Name = name,
                Description = description,
                Active = true
            };

            var service = new ProductService();
            var product = await service.CreateAsync(producrOptions);

            var priceOptions = new PriceCreateOptions
            {
                Product = product.Id,
                UnitAmount = (long)(price * 100),
                Currency = currency
            };

            if (isSubscription)
            {
                priceOptions.Recurring = new PriceRecurringOptions
                {
                    Interval = interval == BillingInterval.Monthly ? "month" : "year"
                };
            }

            var priceService = new PriceService();
            var Price = await priceService.CreateAsync(priceOptions);
            return (product.Id, Price.Id);
        }

        public Task<(string ProductId, string PriceId)> CreateProductAsync(string name, string description, decimal price, bool isSubscription = false, BillingInterval interval = BillingInterval.Monthly)
        {
            throw new NotImplementedException();
        }

        public Task<Core.Models.Subscription> CreateSubscriptionAsync(string customerId, string priceId)
        {
            throw new NotImplementedException();
        }

        public Task<Transfer> CreateTransferToInstructorAsync(string connectedAccountId, decimal amount, string description)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductPriceAsync(string productId, decimal newPrice)
        {
            throw new NotImplementedException();
        }
    }
}
