using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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
        private readonly E_LearningDbContext _context;
        public PaymentService(IConfiguration _config,
            ITransactionsService transactionsService,
            ICourse course,
            E_LearningDbContext context)
        {
            config = _config;
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            this.transactionsService = transactionsService;
            courseService = course;
            _context = context;
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
            Console.WriteLine("Session Created -----");
            return session.Url;
        }

        public async Task HandelPaymentSuccessAsync(string sessionId)
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
                {
                    return; // Already enrolled
                }

                // Get student and course
                var student = await _context.Set<StudentUser>()
                    .Include(s => s.EnrolledCourses)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                var course = await _context.Set<Course>()
                    .FirstOrDefaultAsync(c => c.ID == courseId && !c.IsDeleted);

                if (student == null || course == null)
                {
                    throw new Exception($"Student {studentId} or Course {courseId} not found");
                }

                // Enroll student in course
                student.EnrolledCourses.Add(course);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to use a proper logging framework)
                Console.WriteLine($"Error enrolling student {studentId} in course {courseId}: {ex.Message}");
                throw; // Re-throw to handle at higher level if needed
            }
        }
    }
}
