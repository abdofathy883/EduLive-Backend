using Core.DTOs;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class BlogService : IBlogService
    {
        private readonly IGenericRepo<Blog> repo;
        public BlogService(IGenericRepo<Blog> _repo)
        {
            repo = _repo;
        }
        public async Task<List<BlogDTO>> GetAllBlogsAsync()
        {
            var blogs = await repo.GetAllAsync();
            return (List<BlogDTO>)blogs;
        }

        public async Task<BlogDTO> GetBlogByIDAsync(int blogId)
        {
            var blog = await repo.GetByIdAsync(blogId)
                ?? throw new ArgumentNullException(nameof(blogId), "Blog not found");
            if (blog.IsDeleted)
                throw new InvalidOperationException("Blog is deleted");

            return new BlogDTO
            {
                Title = blog.Title,
                Content = blog.Content,
                PostImage = blog.PostImage
            };
        }
    }
}
