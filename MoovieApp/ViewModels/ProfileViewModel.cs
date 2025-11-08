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
        private async Task GoToDetailsAsync(MovieObject movieObject)
        {
            if (movieObject == null) return;

            var movieModel = new MovieModel
            {
                Id = movieObject.movie_id,
                DisplayTitle = movieObject.title,
                Thumbnail = movieObject.poster_url,
                ThumbnailSmall = movieObject.poster_url
            };

            var parameters = new Dictionary<string, object>
            {
                [nameof(DetailViewModel.Movie)] = movieModel
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
    }
}

