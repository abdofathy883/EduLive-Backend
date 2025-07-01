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
    internal class TransactionsConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Explicitly define the primary key
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();



            // Relationship: One-to-Many with StudentUser
            builder.HasOne(p => p.Student)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(p => p.StudentId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting payments if a student is deleted

            // Relationship: One-to-Many with Course
            builder.HasOne(p => p.Course)
                   .WithMany(c => c.Purchases)
                   .HasForeignKey(p => p.CourseId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevents deleting payments if a course is deleted
        }
    }
}
