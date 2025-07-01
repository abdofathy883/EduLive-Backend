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
    internal class CourseConfig : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(p => p.ID)
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.NuOfLessons)
                .IsRequired();

            builder.Property(p => p.OriginalPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.SalePrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.CourseImagePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.CertificateTemplatePath)
                .HasMaxLength(500);

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnUpdate();

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany(cat => cat.Courses) // Assuming Category has a collection of Courses
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Instructor)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.IssuedCertificates)
                .WithOne(p => p.Course)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Lessons)
                .WithOne()
                .HasForeignKey("CourseId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.CourseReviews)
                .WithOne()
                .HasForeignKey("CourseId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.EnrolledStudents)
                .WithMany(c => c.EnrolledCourses)
                .UsingEntity(j => j.ToTable("StudentCourses"));

        }
    }
}
