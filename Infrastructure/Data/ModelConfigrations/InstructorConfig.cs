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
    internal class InstructorConfig : IEntityTypeConfiguration<InstructorUser>
    {
        public void Configure(EntityTypeBuilder<InstructorUser> builder)
        {
            builder.Property(p => p.InstructorId)
                .ValueGeneratedNever();

            builder.HasMany(p => p.Courses)
                .WithOne(i => i.Instructor)
                .HasForeignKey(i => i.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Lessons)
                .WithOne(l => l.Instructor)
                .HasForeignKey(k => k.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.IssuedCertificates)
                .WithOne(p => p.Instructor)
                .HasForeignKey(p => p.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.InstructorReviews)
                .WithOne(r => r.Instructor)
                .HasForeignKey(r => r.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
