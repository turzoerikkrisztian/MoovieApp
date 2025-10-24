using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Services;
using System.Collections.ObjectModel;

namespace MoovieApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        private readonly DatabaseService _databaseService;

        private const int CurrentUserId = 1; // Placeholder for current user ID

        public HomeViewModel(TmdbService tmdbService, DatabaseService databaseService)
        {
            _tmdbService = tmdbService;
            _databaseService = databaseService;
        }


        [ObservableProperty]
        private MovieModel _trendingMovie;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowMovieInfoBox))]
        private MovieModel? _selectedMovie;

        public bool ShowMovieInfoBox => SelectedMovie is not null;

        


        public ObservableCollection<MovieModel> TrendingMovies { get; set; } = new();
        public ObservableCollection<MovieModel> TopRatedMovies { get; set; } = new();
        public ObservableCollection<MovieModel> MyList { get; set; } = new();


        public async Task InitalizeAsync()
        {
            // ... (meglévő kód) ...
            try
            {
                var trendingMoviesTask = _tmdbService.GetTrendingMoviesAsync();
                var topRatedMoviesTask = _tmdbService.GetTopRatedMoviesAsync();

                var movieModels = await Task.WhenAll(trendingMoviesTask,
                                                     topRatedMoviesTask);

                var trendingMovies = movieModels[0];
                var topRatedMovies = movieModels[1];

                TrendingMovie = trendingMovies.OrderBy(t => Guid.NewGuid())
                                    .FirstOrDefault(t =>
                                        !string.IsNullOrWhiteSpace(t.DisplayTitle)
                                        && !string.IsNullOrWhiteSpace(t.Thumbnail));

                SetMovieCollection(trendingMovies, TrendingMovies);
                SetMovieCollection(topRatedMovies, TopRatedMovies);
            }
            catch (Exception ex)
            {
                
                await Shell.Current.DisplayAlert("Error", $"Failed to load movies: {ex.Message}", "OK");
            }
        }

        private static void SetMovieCollection(IEnumerable<MovieModel> movies,
            ObservableCollection<MovieModel> collection)
        {
            collection.Clear();
            {
                foreach (var movie in movies)
                {
                    collection.Add(movie);
                }
            }
        }

        [RelayCommand]
        private void SelectMovie(MovieModel? movie = null)
        {
            if (movie is not null)
            {
                if (movie.Id == SelectedMovie?.Id)
                {
                    movie = null;
                }
            }
            SelectedMovie = movie;
        }

        [RelayCommand]
        private async Task AddToMyListAsync()
        {
            // Itt a TrendingMovie-ra hajtjuk végre a műveletet
            if (TrendingMovie is null) return;

            try
            {
                await _databaseService.AddMovieToListAsync(
                    CurrentUserId,
                    TrendingMovie.Id,
                    TrendingMovie.DisplayTitle,
                    TrendingMovie.ThumbnailSmall
                );
                await Shell.Current.DisplayAlert("Siker", $"{TrendingMovie.DisplayTitle} hozzáadva a listádhoz!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt a mentéskor: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RateMovieAsync()
        {
            
            if (TrendingMovie is null) return;

            string ratingStr = await Shell.Current.DisplayPromptAsync("Értékelés", $"Hány csillagot adsz a(z) \"{TrendingMovie.DisplayTitle}\" filmre? (1-5)", keyboard: Keyboard.Numeric);

            if (int.TryParse(ratingStr, out int rating) && rating >= 1 && rating <= 5)
            {
                try
                {
                    await _databaseService.RateMovieAsync(
                        CurrentUserId,
                        TrendingMovie.Id,
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


    }
}
