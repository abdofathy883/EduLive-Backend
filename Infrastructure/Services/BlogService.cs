using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var blog = await repo.GetByIdAsync(blogId);
            if (blog is not null && blog.IsDeleted)
            {
                blog = null;
            }
            var blogDTO = new BlogDTO
            {
                Title = blog.Title,
                Content = blog.Content,
                PostImage = blog.PostImage
            };
            return blogDTO;
        }
    }
}
