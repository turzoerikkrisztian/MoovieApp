using MoovieApp.Models;
using System.Formats.Asn1;
using System.Net.Http.Json;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MoovieApp.Services
{
    public class TmdbService
    {
        private const string ApiKey = "e5b202186db542fe0aaa0671d17b206c";
        public const string HttpClientName = "Tmdbclient";
        private readonly IHttpClientFactory _httpClientFactory;

        public TmdbService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        private HttpClient HttpClient => _httpClientFactory.CreateClient(HttpClientName);

        public async Task<IEnumerable<MovieModel>> GetTrendingMoviesAsync() =>
            await GetMovieModelsAsync(TmdbUrls.Trending);

        public async Task<IEnumerable<MovieModel>> GetTopRatedMoviesAsync() =>
            await GetMovieModelsAsync(TmdbUrls.TopRated);

        public async Task<IEnumerable<MovieModel>> GetSimilarAsync(int movieId) =>
            await GetMovieModelsAsync(TmdbUrls.GetSimilar(movieId));


        public async Task<IEnumerable<Video>?> GetTrailersAsync(int id)
        {
            var videosWrapper = await HttpClient.GetFromJsonAsync<VideosWrapper>(
                $"{TmdbUrls.GetTrailers(id)}&api_key={ApiKey}");

            if (videosWrapper?.results?.Length > 0)
            {
                var trailers = videosWrapper.results.Where(VideosWrapper.FilterTrailerTeasers);
                return trailers;
            }
            return null;
        }


        public async Task<MovieDetail?> GetMovieDetailsAsync(int id)
        {
            var movieDetails = await HttpClient.GetFromJsonAsync<MovieDetail>(
                $"{TmdbUrls.GetMovieDetails(id)}&api_key={ApiKey}");
            return movieDetails;
        }   

        private async Task<IEnumerable<MovieModel>> GetMovieModelsAsync(string url)
        {
            var moviesCollection = await HttpClient.GetFromJsonAsync<Movie>($"{url}&api_key={ApiKey}");
            return moviesCollection.results
                    .Select(r => r.ToMovieObject());
        }

       
    }

    public static class TmdbUrls
    {
        public const string Trending = "3/trending/movie/week?language=en-US";
        public const string Search = "3/search/movie";
        public const string TopRated = "3/movie/top_rated?language=en-US";
        

        public static string GetTrailers(int movieId, string type = "movie") => $"3/{type ?? "movie"}/{movieId}/videos?language=en-US";
        public static string GetMovieDetails(int movieId, string type = "movie") => $"3/{type ?? "movie"}/{movieId}?language=en-US";
        public static string GetSimilar(int movieId, string type = "movie") => $"3/{type ?? "movie"}/{movieId}/similar?language=en-US";


    }

    public class Movie
    {
        public int page { get; set; }
        public Result[] results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }

    public class Result
    {
        public string backdrop_path { get; set; }
        public int[] genre_ids { get; set; }
        public int id { get; set; }
        public string original_title { get; set; }
        public string original_name { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public bool video { get; set; }
        //public string media_type { get; set; } // "movie" or "tv"
        public string ThumbnailPath => poster_path ?? backdrop_path;
        public string Thumbnail => $"https://image.tmdb.org/t/p/w600_and_h900_bestv2/{ThumbnailPath}";
        public string ThumbnailSmall => $"https://image.tmdb.org/t/p/w220_and_h330_face/{ThumbnailPath}";
        public string ThumbnailUrl => $"https://image.tmdb.org/t/p/original/{ThumbnailPath}";
        public string DisplayTitle => title ?? name ?? original_title ?? original_name;

        public MovieModel ToMovieObject() =>
            new()
            {
                Id = id,
                DisplayTitle = DisplayTitle,
                Overview = overview,
                ReleaseDate = release_date,
                Thumbnail = Thumbnail,
                ThumbnailSmall = ThumbnailSmall,
                ThumbnailUrl = ThumbnailUrl
            };
    }


    public class VideosWrapper
    {
        public int id { get; set; }
        public Video[] results { get; set; }

        public static Func<Video, bool> FilterTrailerTeasers => v =>
            v.official
            && v.site.Equals("Youtube", StringComparison.OrdinalIgnoreCase)
            && (v.type.Equals("Teaser", StringComparison.OrdinalIgnoreCase) || v.type.Equals("Trailer", StringComparison.OrdinalIgnoreCase));
    }

    public class Video
    {
        public string name { get; set; }
        public string key { get; set; }
        public string site { get; set; }
        public string type { get; set; }
        public bool official { get; set; }
        public DateTime published_at { get; set; }
        public string Thumbnail => $"https://i.ytimg.com/vi/{key}/mqdefault.jpg";
    }


    public class MovieDetail
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public object belongs_to_collection { get; set; }
        public int budget { get; set; }
        public Genre[] genres { get; set; }
        public string homepage { get; set; }
        public int id { get; set; }
        public string imdb_id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public float popularity { get; set; }
        public string poster_path { get; set; }
        public Production_Companies[] production_companies { get; set; }
        public Production_Countries[] production_countries { get; set; }
        public string release_date { get; set; }
        public int revenue { get; set; }
        public int runtime { get; set; }
        public Spoken_Languages[] spoken_languages { get; set; }
        public string status { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public float vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class Production_Companies
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
        public string origin_country { get; set; }
    }

    public class Production_Countries
    {
        public string iso_3166_1 { get; set; }
        public string name { get; set; }
    }

    public class Spoken_Languages
    {
        public string english_name { get; set; }
        public string iso_639_1 { get; set; }
        public string name { get; set; }
    }
    public class GenreWrapper
    {
        public IEnumerable<Genre> Genres { get; set; }
    }
    public record struct Genre(int Id, string Name);
}

