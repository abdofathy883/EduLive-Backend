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
    internal class CourseReviewConfig : IEntityTypeConfiguration<CourseReview>
    {
        public void Configure(EntityTypeBuilder<CourseReview> builder)
        {
            builder.Property(p => p.CourseReviewId)
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.Rating)
                .IsRequired();

            builder.Property(p => p.Comment)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnUpdate();

            builder.HasOne(p => p.Course)
                .WithMany(p => p.CourseReviews)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Student)
                .WithMany(p => p.CourseReviews)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
