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
    internal class GoogleMeetLessonConfig : IEntityTypeConfiguration<GoogleMeetLesson>
    {
        public void Configure(EntityTypeBuilder<GoogleMeetLesson> builder)
        {
            builder.Property(g => g.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(g => g.GoogleEventId)
                .HasMaxLength(300);

            builder.Property(g => g.GoogleMeetURL)
                .HasMaxLength(500);

            builder.Property(g => g.GoogleCalendarId)
                .HasMaxLength(100);

            builder.Property(g => g.Duration)
                .IsRequired()
                .HasMaxLength(3);

            builder.HasOne(g => g.Lesson)
                .WithOne(l => l.GoogleMeetLesson)
                .HasForeignKey<GoogleMeetLesson>(g => g.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
