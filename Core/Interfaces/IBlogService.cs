using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBlogService
    {
        Task<List<BlogDTO>> GetAllBlogsAsync();
        Task<BlogDTO> GetBlogByIDAsync(int blogId);
    }
}
