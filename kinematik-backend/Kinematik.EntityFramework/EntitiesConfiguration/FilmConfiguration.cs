using Kinematik.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kinematik.EntityFramework.EntitiesConfiguration
{
    public class FilmConfiguration : IEntityTypeConfiguration<Film>
    {
        public void Configure(EntityTypeBuilder<Film> builder)
        {
            builder.Property(film => film.Description)
                .IsRequired()
                .IsUnicode();
        }
    }
}