using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOptions<StripeSettings> stripeSettings;
        private readonly ITransactionsService transactionsService;
        private readonly IEmailService emailService;
        private readonly ILogger logger;
        private readonly ICourse courseService;
        private readonly E_LearningDbContext _context;
        public PaymentService(
            IOptions<StripeSettings> _stripeSettings,
            ITransactionsService transactionsService,
            IEmailService _emailService,
            ICourse course,
            ILogger _logger,
            E_LearningDbContext context)
        {
            this.transactionsService = transactionsService;
            courseService = course;
            _context = context;
            stripeSettings = _stripeSettings;
            emailService = _emailService;
            logger = _logger;
        }

        public async Task<string> CreateCheckoutSessionAsync(string studentId, int courseId, decimal amount)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
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
                Mode = "payment",
                SuccessUrl = stripeSettings.Value.SuccessUrl,
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

        public async Task HandlePaymentSuccessAsync(string sessionId)
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
            await transactionsService.AddTransactionAsync(transaction);
            await EnrollStudentInCourseAsync(studentId, courseId);
        }

        private async Task EnrollStudentInCourseAsync(string studentId, int courseId)
        {
            try
            {
                // Check if student is already enrolled
                var existingEnrollment = await _context.Set<StudentUser>()
                    .Include(s => s.EnrolledCourses)
                    .Where(s => s.StudentId == studentId)
                    .SelectMany(s => s.EnrolledCourses)
                    .AnyAsync(c => c.ID == courseId);

                if (existingEnrollment)
                    return; // Already enrolled

                // Get student and course
                var student = await _context.Set<StudentUser>()
                    .Include(s => s.EnrolledCourses)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                var course = await _context.Set<Course>()
                    .FirstOrDefaultAsync(c => c.ID == courseId && !c.IsDeleted);

                if (student == null || course == null)
                    throw new Exception($"Student {studentId} or Course {courseId} not found");

                // Enroll student in course
                if (student.EnrolledCourses == null)
                    student.EnrolledCourses = new List<Course>();

                student.EnrolledCourses.Add(course);
                await _context.SaveChangesAsync();
                await emailService.SendEmailWithTemplateAsync(student.Email, "تأكيد اشتراكك في الدورة التعليمية",
                    "PaymentConfirmation", new Dictionary<string, string>
                    {
                        { "CourseName", course.Title },
                        { "CoursePrice", course.SalePrice.ToString() ?? course.OriginalPrice.ToString("C")},
                        { "InstructorName", course.Instructor.FirstName + " " + course.Instructor.LastName },
                        { "PurchaseDate", DateTime.UtcNow.ToString() },                        
                    });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error enrolling student in course");
                throw; 
            }
        }
    }
}
