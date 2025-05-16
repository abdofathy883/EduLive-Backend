using Core.Models;
using Infrastructure.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.ModelConfigrations
{
    internal class CertificateConfig : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.HasKey(p => p.SerialNumber);

            builder.Property(p => p.SerialNumber)
                .IsRequired()
                .HasMaxLength(8)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<CertificateSerialNumberGenerator>();

            builder.Property(p => p.IssueDate)
                .IsRequired();

            builder.Property(p => p.Score)
                .IsRequired();

            builder.Property(p => p.TemplatePath)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnUpdate();

            // Relationships
            builder.HasOne(p => p.Course)
                .WithMany(p => p.IssuedCertificates)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Student)
                .WithMany(p => p.Certificates)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Instructor)
                .WithMany(p => p.IssuedCertificates)
                .HasForeignKey(p => p.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
