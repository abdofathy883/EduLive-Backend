using Core.DTOs;
using Core.Models;

namespace Core.Interfaces
{
    public interface ICourseReviews
    {
        Task AddReviewAsync(CourseReviewDTO review);
        Task<List<CourseReview>> GetReviewsByCourseIdAsync(int courseId);
        Task<List<CourseReview>> GetReviewsByStudentIdAsync(string studentId);
        Task<CourseReview> GetReviewByIdAsync(int reviewId);
        Task<double> GetAverageRatingByCourseIdAsync(int courseId);
    }
}
