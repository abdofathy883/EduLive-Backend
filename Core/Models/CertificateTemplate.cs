using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CertificateTemplate: IAuditable, IDeletable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string HTMLFilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
