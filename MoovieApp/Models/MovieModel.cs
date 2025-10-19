namespace MoovieApp.Models
{
    public class MovieModel
    {
        public int Id { get; set; }
        public string DisplayTitle { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailSmall { get; set; }
        public string ThumbnailUrl { get; set; }

        public string Overview { get; set; }
        public string ReleaseDate { get; set; }

        public string TrailerURL { get; set; }
    }
}
