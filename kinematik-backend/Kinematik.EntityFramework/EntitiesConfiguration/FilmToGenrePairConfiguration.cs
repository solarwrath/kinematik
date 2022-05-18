using Kinematik.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kinematik.EntityFramework.EntitiesConfiguration
{
    public class FilmToGenrePairConfiguration : IEntityTypeConfiguration<FilmToGenrePair>
    {
        public void Configure(EntityTypeBuilder<FilmToGenrePair> builder)
        {
            builder.HasKey(filmToFilmGenrePair => new {filmToFilmGenrePair.FilmID, filmToFilmGenrePair.GenreID});

            builder.HasOne(filmToFilmGenrePair => filmToFilmGenrePair.Film)
                .WithMany(film => film.GenrePairs)
                .HasForeignKey(filmToFilmGenrePair => filmToFilmGenrePair.FilmID);

            builder.HasOne(filmToFilmGenrePair => filmToFilmGenrePair.Genre)
                .WithMany(film => film.FilmPairs)
                .HasForeignKey(filmToFilmGenrePair => filmToFilmGenrePair.GenreID);
        }
    }
}
