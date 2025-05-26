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
    internal class BaseUserConfig : IEntityTypeConfiguration<BaseUser>
    {
        public void Configure(EntityTypeBuilder<BaseUser> builder)
        {
            
            
            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.FirstName)
                .HasMaxLength(35)
                .IsRequired();
            builder.Property(p => p.LastName)
                .IsRequired();
            //builder.Property(p => p.PhoneNumber)
            //    .IsRequired();
            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnUpdate();

            builder.OwnsMany(p => p.RefreshTokens, rt =>
            {
                rt.WithOwner();
                rt.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(512);
                rt.Property(r => r.ExpiresOn)
                .IsRequired();
                rt.Property(r => r.CreateOn)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            });
            
        }
    }
}
