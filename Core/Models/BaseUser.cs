using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class BaseUser: IdentityUser, IAuditable, IDeletable
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public List<WhatsappALert> Notifications { get; set; }
        public DateTime CreatedAt { get ; set ; }
        public DateTime? UpdatedAt { get ; set ; }
        public bool IsDeleted { get; set; }
    }
}
