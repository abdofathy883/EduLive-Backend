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
    internal class InstructorReviewConfig : IEntityTypeConfiguration<InstructorReview>
    {
        public void Configure(EntityTypeBuilder<InstructorReview> builder)
        {
            builder.Property(p => p.InstructorReviewId)
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

            builder.HasOne(p => p.Instructor)
                .WithMany(p => p.InstructorReviews)
                .HasForeignKey(p => p.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Student)
                .WithMany(p => p.InstructorReviews)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
