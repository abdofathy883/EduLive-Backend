using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Category: IDeletable
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<Course>? Courses { get; set; }
        public bool IsDeleted { get; set; }
    }
}
