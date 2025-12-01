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

namespace MoovieApp.ViewModels
{
    public partial class OnboardingViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        private readonly DatabaseService _databaseService;
        private Queue<MovieModel> _moviesQueue = new();

        public OnboardingViewModel(TmdbService tmdbService, DatabaseService databaseService)
        {
            _tmdbService = tmdbService;
            _databaseService = databaseService;
        }

        [ObservableProperty]
        private MovieModel _currentMovie;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRating;

        public async Task InitializeAsync()
        {
            IsBusy = true;
            try
            {
                var movies = await _tmdbService.GetTrendingMoviesAsync();

                if (movies != null) 
                { 
                    foreach (var movie in movies.Take(10))
                    {
                        _moviesQueue.Enqueue(movie);
                    }
                    ShowNextMovie();
                }
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load movies: {ex.Message}", "OK");
                }
                FinishOnBoarding();
            }
            finally
            {
                IsBusy = false;
            }
        }
       
        private void ShowNextMovie()
        {
            if (_moviesQueue.Count > 0) CurrentMovie = _moviesQueue.Dequeue();

            else FinishOnBoarding();
        }

        [RelayCommand]
        private async Task RateAsync(string ratingType)
        {
            if (IsRating) return;
            IsRating = true;

            try
            {
                int userId = Preferences.Get("current_user_id", 0);                
                if (userId == 0) return;
                int ratingValue = ratingType == "Like" ? 5 : 1;

                if (ratingType != "Skip" && CurrentMovie != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Saving rating for movie: {CurrentMovie.Id}");
                    await _databaseService.RateMovieAsync(
                        userId,
                        CurrentMovie.Id,
                        ratingValue,
                        CurrentMovie.DisplayTitle,
                        CurrentMovie.ThumbnailSmall,
                        CurrentMovie.Overview);
                }
                ShowNextMovie();
            }
            catch (Exception ex)
            {               
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save rating. Please try again.", "OK");
            }
            finally
            {
                IsRating = false;
            }
        }

        [RelayCommand]
        private void FinishOnBoarding()
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] Finishing onboarding, navigating to AppShell.");
            Application.Current.MainPage = new AppShell();
        }

        

    }
}
