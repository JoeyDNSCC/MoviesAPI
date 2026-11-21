namespace MoviesAPI.DTOs
{
    public class Movie
    {
        public int Id { get; set; }
        public int GenreId { get; set; }
        public int AudienceRatingId { get; set; }


        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Runtime { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ImageUrl { get; set; } = string.Empty;


        public DateTime CreatedDate { get; set; }


        public Genre Genre { get; set; } = null!;
        public AudienceRating AudienceRating { get; set; } = null!;
        public List<Review> Reviews { get; set; } = null!;
    }
}
