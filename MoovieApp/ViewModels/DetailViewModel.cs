using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Services;
using System.Collections.ObjectModel;

namespace MoovieApp.ViewModels
{
    [QueryProperty(nameof(Movie), nameof(Movie))]
    public partial class DetailViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        private readonly DatabaseService _databaseService;


       /* private const int CurrentUserId = 1;*/ // Placeholder for current user ID

        public DetailViewModel(TmdbService tmdbService, DatabaseService databaseService)
        {
            _tmdbService = tmdbService;
            _databaseService = databaseService;
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
                    MainTrailerUrl = new HtmlWebViewSource { Html = iframe };

                    var details = await _tmdbService.GetMovieDetailsAsync(Movie.Id);
                    if (details is not null)
                    {
                        Runtime = details.runtime;
                    }
                    
                    
                }
                else
                {
                    //await Shell.Current.DisplayAlert("Not Found", "No trailer available for this movie.", "OK");
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


        [RelayCommand]
        private async Task AddToMyListAsync()
        {
            if (Movie is null) return;

            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                await Shell.Current.DisplayAlert("Error", "Log in to save movies", "ok");
                return;
            }

            try
            {
                
                await _databaseService.AddMovieToListAsync(
                    userId, 
                    Movie.Id,
                    Movie.DisplayTitle,
                    Movie.ThumbnailSmall 
                );

                
                await Shell.Current.DisplayAlert("Succsess", $"{Movie.DisplayTitle} added to MyList!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"There was an error while trying to save the movie: {ex.Message}", "OK");
            }
        }


        [RelayCommand]
        private async Task RateMovieAsync()
        {
            if (Movie is null) return;

            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                await Shell.Current.DisplayAlert("Error", "Log in to rate movies", "ok");
                return;
            }

            string ratingStr = await Shell.Current.DisplayPromptAsync("Értékelés", $"Hány csillagot adsz a(z) \"{Movie.DisplayTitle}\" filmre? (1-5)", keyboard: Keyboard.Numeric);

            if (int.TryParse(ratingStr, out int rating) && rating >= 1 && rating <= 5)
            {
                try
                {
                    
                    await _databaseService.RateMovieAsync(
                        userId,
                        Movie.Id,
                        rating
                    
                    );

                    await Shell.Current.DisplayAlert("Siker", $"Sikeresen értékelted ({rating}/5) a filmet!", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Hiba", $"Hiba történt az értékeléskor: {ex.Message}", "OK");
                }
            }
            else if (!string.IsNullOrEmpty(ratingStr))
            {
                await Shell.Current.DisplayAlert("Hiba", "Érvénytelen értékelés. Kérlek, 1 és 5 közötti számot adj meg.", "OK");
            }
            
        }


        private static string GetYoutubeUrl(string key) => 
            $"https://www.youtube.com/embed/{key}";

    }   
}
