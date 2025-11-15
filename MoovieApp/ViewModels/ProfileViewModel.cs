using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Services;
using MoovieApp.Pages;
using System.Collections.ObjectModel;

namespace MoovieApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<MovieObject> MyList { get; }

        public ProfileViewModel(DatabaseService databaseService, IServiceProvider serviceProvider)
        {
            _databaseService = databaseService;
            _serviceProvider = serviceProvider;
            MyList = new ObservableCollection<MovieObject>();
        }

        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        private string _userEmail;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowMovieInfoBox))]
        private MovieModel? _selectedMovie;

        public bool ShowMovieInfoBox => SelectedMovie is not null;


        public async Task InitializeAsync()
        {
            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0) return;

            var user = await _databaseService.GetUserAsync(userId);
            if (user != null)
            {
                UserName = user.username;
                UserEmail = user.email;
            }

            var list = await _databaseService.GetUserListAsync(userId);
            MyList.Clear();
            foreach (var movie in list)
            {
                MyList.Add(movie);
            }
        }


        [RelayCommand]
        private void SelectMovie(MovieObject movieObject)
        {
            if (movieObject == null)
            {
                SelectedMovie = null;
                return;
            }

            SelectedMovie = new MovieModel
            {
                Id = movieObject.movie_id,
                DisplayTitle = movieObject.title,
                Thumbnail = movieObject.poster_url,
                ThumbnailSmall = movieObject.poster_url,
                Overview = movieObject.overview

            };
        }

        [RelayCommand]
        private async Task RemoveFromMyListAsync(MovieModel movie)
        {
            if (movie == null) return;

            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", $"Are you sure you want to remove \"{movie.DisplayTitle}\" from your list?", "Yes", "No");
            if (!confirm) return;

            await _databaseService.RemoveMovieFromListAsync(userId, movie.Id);

            var itemToRemove = MyList.FirstOrDefault(m => m.movie_id == movie.Id);
            if (itemToRemove != null)
            {
                MyList.Remove(itemToRemove);
            }

            SelectedMovie = null;

            await Application.Current.MainPage.DisplayAlert("Removed", $"\"{movie.DisplayTitle}\" has been removed from your list.", "OK");

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

        [RelayCommand]
        private async Task LogoutAsync()
        {
            Preferences.Remove("current_user_id");
            Preferences.Remove("current_username");
           
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            Application.Current.MainPage = loginPage;
        }

        [RelayCommand]
        private async Task RateMovieAsync(MovieModel movie)
        {
            if (movie == null) return;

            int userId = Preferences.Get("current_user_id", 0);
            if (userId == 0) return;

            string ratingStr = await Application.Current.MainPage.DisplayPromptAsync("Rate Movie", $"How many stars do you give to \"{movie.DisplayTitle}\"? (1-5)", keyboard: Keyboard.Numeric);

            if (int.TryParse(ratingStr, out int rating) && rating >= 1 && rating <= 5)
            {
                try
                {
                    await _databaseService.RateMovieAsync(userId, movie.Id, rating, movie.DisplayTitle, movie.ThumbnailSmall, movie.Overview);
                    await Application.Current.MainPage.DisplayAlert("Thank You", $"You rated \"{movie.DisplayTitle}\" You rated", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred while submitting your rating: {ex.Message}", "OK");
                }
            }
        }
    }
}

