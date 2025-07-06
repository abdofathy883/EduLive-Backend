using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ModelConfigrations
{
    internal class CertificateTemplateConfig : IEntityTypeConfiguration<CertificateTemplate>
    {
        public void Configure(EntityTypeBuilder<CertificateTemplate> builder)
        {
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.HTMLFilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnUpdate();
        }
    }
}
