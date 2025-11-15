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

        //private const int CurrentUserId = 1; 

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
        private async Task AddToMyListAsync(MovieModel? movie)
        {

            movie ??= TrendingMovie;
            if (movie is null) return;

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
                    movie.Id,
                    movie.DisplayTitle,
                    movie.ThumbnailSmall,
                    movie.Overview
                );
                await Shell.Current.DisplayAlert("Succsess", $"{movie.DisplayTitle} added to list!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error when trying to add movie to list: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RateMovieAsync(MovieModel? movie)
        {

            movie ??= TrendingMovie;
            if (movie is null) return;
            
            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                await Shell.Current.DisplayAlert("Error", "Log in to rate movies", "ok");
                return;
            }

            string ratingStr = await Shell.Current.DisplayPromptAsync("Rating", 
                $"How many stars would you give \"{movie.DisplayTitle}\"? (1-5)", keyboard: Keyboard.Numeric);

            if (int.TryParse(ratingStr, out int rating) && rating >= 1 && rating <= 5)
            {
                try
                {
                    await _databaseService.RateMovieAsync(
                        userId,
                        movie.Id,
                        rating,
                        movie.DisplayTitle,
                        movie.ThumbnailSmall,
                        movie.Overview
                    );
                    await Shell.Current.DisplayAlert("Thank You", $"You rated the movie ({rating}/5)!", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"An error occurred while submitting your rating: {ex.Message}", "OK");
                }
            }
            else if (!string.IsNullOrEmpty(ratingStr))
            {
                await Shell.Current.DisplayAlert("Error", "Invalid rating please give a rating between 1-5", "OK");
            }
        }


    }
}
