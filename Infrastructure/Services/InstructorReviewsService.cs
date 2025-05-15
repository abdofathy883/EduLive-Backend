using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
