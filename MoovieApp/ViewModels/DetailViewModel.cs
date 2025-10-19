using CommunityToolkit.Mvvm.ComponentModel;
using MoovieApp.Models;
using MoovieApp.Services;

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
        private string _mainTrailerUrl;

        [ObservableProperty]
        private bool _isBusy;

        public async Task InitializeAsyns()
        {
            IsBusy = true;

            try
            {
                var trailersTeasers = await _tmdbService.GetTrailersAsync(Movie.Id);
                if (trailersTeasers?.Any() == true)
                {
                    var trailer = trailersTeasers.FirstOrDefault(t => t.type == "Trailer");

                    if (trailer is null)
                    {
                        trailer = trailersTeasers.First();
                    }
                    MainTrailerUrl = GetYoutubeUrl(trailer.key);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "No trailer available for this movie.", "OK");
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
