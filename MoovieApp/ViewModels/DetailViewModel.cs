using CommunityToolkit.Mvvm.ComponentModel;
using MoovieApp.Models;
using MoovieApp.Services;
using System.Collections.ObjectModel;

namespace MoovieApp.ViewModels
{
    [QueryProperty(nameof(Movie), nameof(Movie))]
    public partial class DetailViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        public DetailViewModel(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        [ObservableProperty]
        private MovieModel _movie;

        [ObservableProperty]
        private HtmlWebViewSource _mainTrailerUrl;

        [ObservableProperty]
        private int _runtime;
        

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private int _similarItemWidth = 125;

        public ObservableCollection<MovieModel> SimilarMovies { get; set; } = new();

        partial void OnMovieChanged(MovieModel value)
        {
            if (value is not null)
            {
                _ = InitializeAsyns();
            }
        }

        public async Task InitializeAsyns()
        {
            
            IsBusy = true;

            try
            {
                var trailersTeasers = await _tmdbService.GetTrailersAsync(Movie.Id);
                var movieDetails = await _tmdbService.GetMovieDetailsAsync(Movie.Id);
                var similarMoviesTask = _tmdbService.GetSimilarAsync(Movie.Id);

                if (trailersTeasers?.Any() == true)
                {
                    var trailer = trailersTeasers.FirstOrDefault(t => t.type == "Trailer");

                    //if (trailer is null)
                    //{
                    //    trailer = trailersTeasers.First();
                    //}
                    var iframe = $@"
<html>
  <body style='margin:0;padding:0;'>
    <iframe width='100%' height='100%' src='https://www.youtube.com/embed/{trailer.key}?rel=0&playsinline=1'
      frameborder='0'
      allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture'
      allowfullscreen>
    </iframe>
  </body>
</html>";
                    MainTrailerUrl = new HtmlWebViewSource { Html = iframe };/*GetYoutubeUrl(trailer.key);*/

                    var details = await _tmdbService.GetMovieDetailsAsync(Movie.Id);
                    if (details is not null)
                    {
                        Runtime = details.runtime;
                    }
                    
                    
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "No trailer available for this movie.", "OK");
                }
                var similarMovies = await similarMoviesTask;
                SimilarMovies.Clear();
                if (similarMovies?.Any() == true)
                {
                    foreach (var m in similarMovies)
                        SimilarMovies.Add(m);
                }

            }
            finally
            {
                IsBusy = false;
            }

        }

        private static string GetYoutubeUrl(string key) => 
            $"https://www.youtube.com/embed/{key}";

    }   
}
