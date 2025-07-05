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
    internal class ZoomLessonConfig : IEntityTypeConfiguration<ZoomMeeting>
    {
        public void Configure(EntityTypeBuilder<ZoomMeeting> builder)
        {

            builder.Property(z => z.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(z => z.ZoomMeetingId)
                .IsRequired();

            builder.Property(z => z.JoinUrl)
                .HasMaxLength(500);

            builder.Property(z => z.StartUrl)
                .HasMaxLength(500);

            builder.Property(z => z.Password)
                .HasMaxLength(100);

            builder.Property(z => z.Duration)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasOne(z => z.Lesson)
                .WithOne(l => l.ZoomMeeting)
                .HasForeignKey<ZoomMeeting>(z => z.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
