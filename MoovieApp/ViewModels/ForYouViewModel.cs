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

            Recomendations = new ObservableCollection<MovieModel>();
        }

        public ObservableCollection<MovieModel> Recomendations { get; }

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _statusMessage;

        public async Task InitalizeAsync()
        {
            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0)
            {
                StatusMessage = "Please log in to see recommendations.";
                return;
            }

            IsBusy = true;
            StatusMessage = "Loading recommendations...";
            Recomendations.Clear();

            try
            {
                var listMovies = await _databaseService.GetUserListAsync(userId);
                var ratedMovies = await _databaseService.GetRatedMoviesAsync(userId);

                var allUserMovies = listMovies.Concat(ratedMovies)
                    .GroupBy(m => m.movie_id)
                    .Select(g => g.First())
                    .Select(dbMovie => new MovieModel
                    {
                        Id = dbMovie.movie_id,
                        Overview = dbMovie.overview
                    })
                    .ToList();


                if (!allUserMovies.Any())
                {
                    StatusMessage = "Not enough data. Rate some Moovies!";
                    IsBusy = false;
                    return;
                }

                var candidates = (await _tmdbService.GetTrendingMoviesAsync()).ToList();

                var recommendedMovies = await _recommendationService.GetRecommendationAsync(allUserMovies, candidates);


                if (recommendedMovies.Any())
                {
                    StatusMessage = "Moovies For You:";
                    foreach (var id in recommendedMovies)
                    {
                        var movie = candidates.FirstOrDefault(c => c.Id == id);
                        if (movie != null)
                        {
                            Recomendations.Add(movie);
                        }
                    }
                }
                else
                {
                    StatusMessage = "No recommendations available at the moment. Here are some trending Moovies:";
                    foreach (var movie in candidates.Take(10))
                    {
                        Recomendations.Add(movie);
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
