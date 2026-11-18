using RuslanMurzaevKt_31_23.Database.Helpers;
using RuslanMurzaevKt_31_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RuslanMurzaevKt_31_23.Database.Configurations
{
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        private const string TableName = "cd_specialty";

        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            builder
                .HasKey(p => p.SpecialtyId)
                .HasName($"pk_{TableName}_specialty_id");

            builder.Property(p => p.SpecialtyId)
                .ValueGeneratedOnAdd()
                .HasColumnName("specialty_id")
                .HasComment("Идентификатор специальности");

            builder.Property(p => p.Title)
                .IsRequired()
                .HasColumnName("c_specialty_title")
                .HasColumnType(ColumnType.String).HasMaxLength(150)
                .HasComment("Название специальности");

            builder.Property(p => p.Code)
                .IsRequired()
                .HasColumnName("c_specialty_code")
                .HasColumnType(ColumnType.String).HasMaxLength(20)
                .HasComment("Код специальности");

            builder.ToTable(TableName);
        }
    }
}
