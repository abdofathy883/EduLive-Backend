using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CourseReviewsService: ICourseReviews
    {
        private readonly IGenericRepo<CourseReview> reviewRepo;
        private readonly IGenericRepo<Course> courseRepo;
        public CourseReviewsService(IGenericRepo<CourseReview> repo,
            IGenericRepo<Course> repo1)
        {
            reviewRepo = repo;
            courseRepo = repo1;
        }

        public async Task AddReviewAsync(CourseReviewDTO review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review), "Review cannot be null");
            }
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
            var course = courseRepo.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new ArgumentException("Course not found", nameof(courseId));
            }
            var reviews = reviewRepo.GetAllAsync();
            var CourseReviews = reviews.Result.Where(r => r.CourseId == courseId && !r.IsDeleted).ToList();
            if (!CourseReviews.Any())
            {
                return 0.0; // No reviews found for the course
            }
            return CourseReviews.Average(r => r.Rating);
        }

        public Task<CourseReview> GetReviewByIdAsync(int reviewId)
        {
            var review = reviewRepo.GetByIdAsync(reviewId);
            if (review == null || review.Result.IsDeleted)
            {
                throw new ArgumentException("Review not found", nameof(reviewId));
            }
            return review;
        }

        public async Task<List<CourseReview>> GetReviewsByCourseIdAsync(int courseId)
        {
            var course = await courseRepo.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new ArgumentException("Course not found", nameof(courseId));
            }
            var reviews = await reviewRepo.GetAllAsync();
            return reviews.Where(r => r.CourseId == courseId && !r.IsDeleted).ToList();
        }

        public async Task<List<CourseReview>> GetReviewsByStudentIdAsync(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                throw new ArgumentNullException(nameof(studentId));
            }
            var reviews = reviewRepo.GetAllAsync().Result;
            return reviews.Where(r => r.StudentId == studentId && !r.IsDeleted).ToList();
        }
    }
}
