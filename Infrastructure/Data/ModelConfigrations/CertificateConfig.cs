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
    internal class CertificateConfig : IEntityTypeConfiguration<CertificateIssued>
    {
        public void Configure(EntityTypeBuilder<CertificateIssued> builder)
        {
            builder.HasKey(p => p.SerialNumber);

            builder.Property(p => p.SerialNumber)
                .IsRequired()
                .HasMaxLength(8)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<CertificateSerialNumberGenerator>();

            builder.Property(p => p.IssueDate)
                .IsRequired();

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
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Template)
                .WithMany()
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
