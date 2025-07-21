using Core.Models;

namespace Core.Interfaces
{
    public interface IReviewsService
    {
        Task<InstructorReview> GetAllReviewsAsync();
    }
}
