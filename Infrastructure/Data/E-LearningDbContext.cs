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
        { }
        public DbSet<BaseUser> BaseUsers { get; set; }
        //Courses Tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        //Reviews Tables
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<InstructorReview> InstructorReviews { get; set; }
        //Zoom
        public DbSet<ZoomUserConnection> ZoomUserConnections { get; set; }
        //Google Meet
        public DbSet<GoogleMeetAccount> GoogleMeetAccount { get; set; }
        public DbSet<GoogleMeetLesson> GoogleMeetLessons { get; set; }
        public DbSet<GoogleMeetSettings> GoogleMeetSettings { get; set; }
        // Payments
        public DbSet<Payment> Payments { get; set; }
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
            builder.Entity<BaseUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<BaseUser>("BaseUser")
                .HasValue<StudentUser>("StudentUser")
                .HasValue<InstructorUser>("InstructorUser")
                .HasValue<AdminUser>("AdminUser");

            builder.Entity<StudentUser>()
                .Property(p => p.StudentId)
                .IsRequired();

            builder.Entity<InstructorUser>()
                .Property(p => p.InstructorId)
                .IsRequired();

            builder.Entity<AdminUser>()
                .Property(p => p.AdminId)
                .IsRequired();

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
            builder.ApplyConfiguration(new TransactionsConfig());
        }
    }
}
