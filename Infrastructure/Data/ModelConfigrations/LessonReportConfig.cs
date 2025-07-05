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
    internal class LessonReportConfig : IEntityTypeConfiguration<LessonReport>
    {
        public void Configure(EntityTypeBuilder<LessonReport> builder)
        {
            builder.Property(r => r.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(r => r.DiscussedValues)
                .IsRequired()
                .HasMaxLength(800);

            builder.Property(r => r.ExplainedTopics)
                .IsRequired()
                .HasMaxLength(800);
            builder.Property(r => r.MemorizedContent)
                .IsRequired()
                .HasMaxLength(800);

            builder.HasOne(r => r.Student)
                .WithMany(r => r.LessonReports)
                .HasForeignKey(r => r.StudentId);

            builder.HasOne(r => r.Instructor)
                .WithMany(i => i.LessonReports)
                .HasForeignKey(r => r.InstructorId);
        }
    }
}
