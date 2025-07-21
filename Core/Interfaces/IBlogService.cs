using Core.DTOs;

namespace Core.Interfaces
{
    public interface IBlogService
    {
        Task<List<BlogDTO>> GetAllBlogsAsync();
        Task<BlogDTO> GetBlogByIDAsync(int blogId);
    }
}
