using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Services;
using System.Collections.ObjectModel;
using MoovieApp.Pages;

namespace MoovieApp.ViewModels
{
    public partial class ForYouViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        private readonly DatabaseService _databaseService;
        private readonly RecommendationService _recommendationService;
        public ForYouViewModel(TmdbService tmdbService, DatabaseService databaseService, RecommendationService recommendationService)
        {
            _tmdbService = tmdbService;
            _databaseService = databaseService;
            _recommendationService = recommendationService;

            Recommendations = new ObservableCollection<MovieModel>();
        }

        public ObservableCollection<MovieModel> Recommendations { get; }

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _statusMessage;

        public async Task InitializeAsync()
        {
            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                StatusMessage = "Please log in to see recommendations.";
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading recommendations...";
            Recommendations.Clear();

            try
            {
        
                var seenMovieIds = await _databaseService.GetAllInteractedMovieAsync(userId);

                var listMovies = await _databaseService.GetUserListAsync(userId);
                var ratedMovies = await _databaseService.GetRatedMoviesAsync(userId);


                var allUserMovies = listMovies.Concat(ratedMovies)
                    .GroupBy(m => m.movie_id)
                    .Select(g => g.First())                  
                    .ToList();

             


                var likedMoviesForService = allUserMovies
                    .Select(m => new MovieModel
                    {
                        Id = m.movie_id,
                        Overview = m.overview
                    })
                    .ToList();

                if (!allUserMovies.Any())
                {
                    StatusMessage = "Not enough data. Rate some Moovies!";
                    IsBusy = false;
                    return;
                }

                var candidates = (await _tmdbService.GetTrendingMoviesAsync()).ToList();

                var recommendedMovies = await _recommendationService.GetRecommendationAsync(likedMoviesForService, candidates);


                if (recommendedMovies.Any())
                {
                    StatusMessage = "Moovies For You:";
                    foreach (var id in recommendedMovies)
                    {
                        if (seenMovieIds.Contains(id)) continue;

                        var movie = candidates.FirstOrDefault(c => c.Id == id);
                        if (movie != null)
                        {
                            Recommendations.Add(movie);
                        }
                    }
                }
                if(Recommendations.Count == 0)
                {
                    StatusMessage = "No recommendations available at the moment. Here are some trending Moovies:";
                    
                    var fallbackMovies = candidates
                        .Where(m => !seenMovieIds.Contains(m.Id))
                        .Take(10);

                    foreach (var movie in fallbackMovies)
                    {
                        Recommendations.Add(movie);
                    }
                }

            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load recommendations: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        private async Task AddToListAsync(MovieModel movie)
        {
            if (movie == null) return;

            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please log in to add movies to your list.", "OK");
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
                await Application.Current.MainPage.DisplayAlert("Success", $"{movie.DisplayTitle} has been added to your list.", "OK");
            }
            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("Error", $"Could not add movie to list: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RateMovieAsync(MovieModel movie)
        {
            if (movie == null) return;

            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please log in to rate movies.", "OK");
                return;
            }
            string result = await Application.Current.MainPage.DisplayPromptAsync("Rate Movie", $"Rate {movie.DisplayTitle} (1 to 5):", keyboard: Keyboard.Numeric);
            
            if (int.TryParse(result, out int rating) && rating >= 1 && rating <= 5)
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
                    await Application.Current.MainPage.DisplayAlert("Success", $"You rated {movie.DisplayTitle} with {rating} stars.", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Could not rate movie: {ex.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid rating input.", "OK");
            }
        }


        [RelayCommand]
        private async Task GoToDetailsAsync(MovieModel movie)
        {
            if (movie == null) return;
            var parameters = new Dictionary<string, object>
            {
                [nameof(DetailViewModel.Movie)] = movie
            };
            await Shell.Current.GoToAsync(nameof(MovieDetailsPage), true, parameters);
        }
    }
}
