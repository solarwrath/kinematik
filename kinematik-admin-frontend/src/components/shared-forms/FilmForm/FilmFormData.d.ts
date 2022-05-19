export default interface FilmFormData {
  title: string | null;
  poster: File | null;
  posterUrl: string | null;
  description: string | null;
  genreIDs: number[] | null;
  languageID: number | null;
  runtime: number | null;
  imdbID: string | null;
  trailerUrl: string | null;
  featuredImage: File | null;
  featuredImageUrl: string | null;
}
