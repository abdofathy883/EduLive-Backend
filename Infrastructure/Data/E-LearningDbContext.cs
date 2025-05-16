using Core.Interfaces;
using Core.Models;
using Infrastructure.Data.ModelConfigrations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class E_LearningDbContext : IdentityDbContext<BaseUser>
    {
        public E_LearningDbContext(DbContextOptions<E_LearningDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<StudentUser> StudentUsers { get; set; }
        public DbSet<InstructorUser> InstructorUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<InstructorReview> InstructorReviews { get; set; }
        public DbSet<Certificate> Certificates { get; set; }



        public DbSet<ZoomUserConnection> ZoomUserConnections { get; set; }
        public DbSet<ZoomMeeting> ZoomMeetings { get; set; }


        public DbSet<GoogleMeetAccount> GoogleMeetAccount { get; set; }
        public DbSet<GoogleMeetLesson> GoogleMeetLessons { get; set; }
        public DbSet<GoogleMeetSettings> GoogleMeetSettings { get; set; }
        //public DbSet<CoursePaymentInfo> CoursePaymentInfos { get; set; }
        //public DbSet<Notification> Notifications {  get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(IDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(nameof(IDeletable.IsDeleted))
                        .HasDefaultValue(false);
                }
                if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(nameof(IAuditable.CreatedAt)).
                        HasDefaultValueSql("GETDATE()");
                    builder.Entity(entityType.ClrType)
                        .Property(nameof(IAuditable.UpdatedAt))
                        .HasDefaultValueSql("GETDATE()").ValueGeneratedOnUpdate();
                }
            }
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new BaseUserConfig());
            builder.ApplyConfiguration(new StudentConfig());
            builder.ApplyConfiguration(new InstructorConfig());
            builder.ApplyConfiguration(new CourseConfig());
            builder.ApplyConfiguration(new LessonConfig());
            builder.ApplyConfiguration(new GoogleMeetSettingsConfig());
            builder.ApplyConfiguration(new CertificateConfig());
            builder.ApplyConfiguration(new CourseReviewConfig());
            builder.ApplyConfiguration(new InstructorReviewConfig());
        }
    }
}
