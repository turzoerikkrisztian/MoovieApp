using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Pages;
using MoovieApp.Services;

namespace MoovieApp.ViewModels
{
    public partial class AuthViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        public AuthViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _userPreferences;


        private async Task ShowAlert(string title, string message, string cancel)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
        }


        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            var user = await _databaseService.LoginUserAsync(Email, Password);
            if (user != null)
            {
                Preferences.Set("current_user_id", user.user_id);
                Preferences.Set("current_username", user.username);

                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await ShowAlert("Error", "Invalid email or password.", "OK");
            }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await ShowAlert("Error", "Please fill in all fields.", "OK");
                return;
            }
            var success = await _databaseService.RegisterUserAsync(Username, Email, UserPreferences ?? "", Password);
            if (success)
            {
                await ShowAlert("Success", "Registration successful! You can now log in.", "OK");
            }
            else
            {
                await ShowAlert("Error", "Username or email already exists.", "OK");
            }
        }

    }
}
