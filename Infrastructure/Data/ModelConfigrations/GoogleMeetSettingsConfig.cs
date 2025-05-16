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
    internal class GoogleMeetSettingsConfig : IEntityTypeConfiguration<GoogleMeetSettings>
    {
        public void Configure(EntityTypeBuilder<GoogleMeetSettings> builder)
        {
            builder.HasNoKey();
        }
    }
}
