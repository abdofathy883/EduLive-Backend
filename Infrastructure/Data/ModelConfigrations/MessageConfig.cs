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
    internal class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(m => m.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(m => m.Content)
                .HasMaxLength(1000);

            builder.Property(m => m.SentAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();


        }
    }
}
