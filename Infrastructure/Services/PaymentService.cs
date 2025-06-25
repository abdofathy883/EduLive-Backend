using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration config;
        private readonly ITransactionsService transactionsService;
        private readonly ICourse courseService;
        public PaymentService(IConfiguration _config, ITransactionsService transactionsService, ICourse course)
        {
            config = _config;
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            this.transactionsService = transactionsService;
            courseService = course;
        }

        public async Task<string> CreateCheckoutSessionAsync(string studentId, int courseId, decimal amount)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "Card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(amount * 100), // Convert to cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Course Payment",
                                Description = $"Payment for course ID: {courseId} by student ID: {studentId}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "Payment",
                SuccessUrl = config["Stripe:SuccessUrl"],
                CancelUrl = config["Stripe:CancelUrl"],
                Metadata = new Dictionary<string, string>
                {
                    { "StudentId", studentId },
                    { "CourseId", courseId.ToString() }
                }
            };
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Url;
        }

        public async Task HandelPAymentSuccessAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            string studentId = session.Metadata["StudentId"];
            int courseId = int.Parse(session.Metadata["CourseId"]);
            decimal amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0m;

            var course = await courseService.GetCourseByIdAsync(courseId);
            var instructorId = course.InstructorId;

            var transaction = new Payment
            {
                StudentId = studentId,
                CourseId = courseId,
                InstructorId = instructorId,
                Amount = amount,
                Status = PaymentStatus.Succeeded,
                CreatedAt = DateTime.UtcNow
            };
            await transactionsService.AddAsync(transaction);
        }
    }
}
