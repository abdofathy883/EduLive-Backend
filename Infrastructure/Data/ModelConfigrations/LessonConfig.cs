using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.ModelConfigrations
{
    internal class LessonConfig : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.Property(p => p.LessonId)
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.LessonPlatform)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Course)
                .WithMany(p => p.Lessons)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Instructor)
                .WithMany(p => p.Lessons)
                .HasForeignKey(p => p.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Student)
                .WithMany(p => p.EnrolledLessons)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ZoomMeeting)
                .WithOne(p => p.Lesson)
                .HasForeignKey<ZoomMeeting>(p => p.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.GoogleMeetLesson)
                .WithOne(p => p.Lesson)
                .HasForeignKey<GoogleMeetLesson>(p => p.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
