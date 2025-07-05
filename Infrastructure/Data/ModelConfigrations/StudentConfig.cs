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
    internal class StudentConfig : IEntityTypeConfiguration<StudentUser>
    {
        public void Configure(EntityTypeBuilder<StudentUser> builder)
        {
            builder.Property(p => p.StudentId)
                .ValueGeneratedOnAdd();

            builder.HasMany(p => p.Certificates)
                .WithOne(p => p.Student)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.EnrolledCourses)
                .WithMany(c => c.EnrolledStudents)
                .UsingEntity(j => j.ToTable("StudentCourses"));

            builder.HasMany(p => p.EnrolledLessons)
                .WithOne(l => l.Student)
                .HasForeignKey(k => k.StudentId);

            builder.HasMany(p => p.CourseReviews)
                .WithOne(r => r.Student)
                .HasForeignKey(k => k.StudentId);

            builder.HasMany(p => p.InstructorReviews)
                .WithOne(r => r.Student)
                .HasForeignKey(k => k.StudentId);

            builder.HasMany(p => p.Payments)
                .WithOne(p => p.Student)
                .HasForeignKey(k => k.StudentId);

            builder.HasMany(p => p.LessonReports)
                .WithOne(r => r.Student)
                .HasForeignKey(k => k.StudentId);
        }
    }
}
