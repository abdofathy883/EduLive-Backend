using Core.Interfaces;

namespace Core.Models
{
    public class GoogleMeetUserAccount: IDeletable, IAuditable
    {
        public int Id { get; set; }
        public string GoogleUserId { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string UserId { get; set; }
        public BaseUser User { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
