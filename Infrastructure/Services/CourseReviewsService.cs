using Core.DTOs;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class CourseReviewsService: ICourseReviews
    {
        private readonly IGenericRepo<CourseReview> reviewRepo;
        private readonly IGenericRepo<Course> courseRepo;
        public CourseReviewsService(
            IGenericRepo<CourseReview> repo,
            IGenericRepo<Course> repo1)
        {
            reviewRepo = repo;
            courseRepo = repo1;
        }

        public async Task AddReviewAsync(CourseReviewDTO review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review), "Review cannot be null");

            var newReview = new CourseReview
            {
                StudentId = review.StudentId,
                CourseId = review.CourseId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await reviewRepo.AddAsync(newReview);
            await reviewRepo.SaveAllAsync();
            //await reviewRepo.SaveAllAsync();
        }

        public async Task<double> GetAverageRatingByCourseIdAsync(int courseId)
        {
            var course = await courseRepo.GetByIdAsync(courseId)
                ?? throw new ArgumentException("Course not found", nameof(courseId));

            var reviews = await reviewRepo.GetAllAsync();
            var CourseReviews = reviews.Where(r => r.CourseId == courseId && !r.IsDeleted).ToList();

            if (!CourseReviews.Any())
                return 0.0; // No reviews found for the course

            return CourseReviews.Average(r => r.Rating);
        }

        public async Task<CourseReview> GetReviewByIdAsync(int reviewId)
        {
            var review = await reviewRepo.GetByIdAsync(reviewId)
                ?? throw new ArgumentException("Review not found", nameof(reviewId));

            if (review.IsDeleted)
                throw new InvalidOperationException("Review is deleted");

            return review;
        }

        public async Task<List<CourseReview>> GetReviewsByCourseIdAsync(int courseId)
        {
            var course = await courseRepo.GetByIdAsync(courseId)
                ?? throw new ArgumentException("Course not found", nameof(courseId));

            var reviews = await reviewRepo.FindAsync(r => r.CourseId == courseId && !r.IsDeleted);
            return reviews.Where(r => r.CourseId == courseId && !r.IsDeleted).ToList();
        }

        public async Task<List<CourseReview>> GetReviewsByStudentIdAsync(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentNullException(nameof(studentId));

            var reviews =await reviewRepo.FindAsync(r => r.StudentId == studentId && !r.IsDeleted);
            return (List<CourseReview>)reviews;
        }
    }
}
