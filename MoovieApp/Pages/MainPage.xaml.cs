using MoovieApp.ViewModels;
using MoovieApp.Controls;
using CommunityToolkit.Mvvm.Input;

namespace MoovieApp.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly HomeViewModel _homeViewModel;

        public MainPage(HomeViewModel homeViewModel)
        {
            try
            {
                InitializeComponent();
                _homeViewModel = homeViewModel;
                BindingContext = _homeViewModel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException}");
                throw;
            }
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _homeViewModel.InitalizeAsync();
        }

        private void MovieRow_OnMovieSelected(object sender, Controls.MovieSelectEventArgs e)
        {
            _homeViewModel.SelectMovieCommand.Execute(e.MovieModel);
        }

        private void MovieInfoBox_Closed(object sender, EventArgs e)
        {
            _homeViewModel.SelectMovieCommand.Execute(null);
        }



        private async void MovieInfoBox_AddToListClicked(object? sender, MovieEventArgs e)
        {            
            if (e.Movie != null && _homeViewModel.AddToMyListCommand.CanExecute(e.Movie))
            {                
                await _homeViewModel.AddToMyListCommand.ExecuteAsync(e.Movie);
            }
        }

        private async void MovieInfoBox_RateClicked(object? sender, MovieEventArgs e)
        {            
            if (e.Movie != null && _homeViewModel.RateMovieCommand.CanExecute(e.Movie))
            {                
                await _homeViewModel.RateMovieCommand.ExecuteAsync(e.Movie);
            }
        }
    }

}


