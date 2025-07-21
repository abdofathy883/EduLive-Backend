using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class InstructorReviewsService : IReviewsService
    {
        private readonly IGenericRepo<InstructorReview> repo;
        public InstructorReviewsService(IGenericRepo<InstructorReview> genericRepo)
        {
            repo = genericRepo;
        }
        public async Task<InstructorReview> GetAllReviewsAsync()
        {
            var reviews = await repo.GetAllAsync();
            return (InstructorReview)reviews;
        }
    }
}
