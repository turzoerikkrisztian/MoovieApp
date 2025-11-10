using MoovieApp.ViewModels;



namespace MoovieApp.Pages;

public partial class UserDetailsPage : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    public UserDetailsPage(ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        _viewModel = profileViewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }

    private void MovieInfoBox_Closed(object sender, EventArgs e)
    {
        _viewModel.SelectMovieCommand.Execute(null);
    }

    private async void MovieInfoBox_RemoveClicked(object sender, Controls.MovieEventArgs e)
    {
        if (_viewModel.RemoveFromMyListCommand.CanExecute(e.Movie))
        {
            await _viewModel.RemoveFromMyListCommand.ExecuteAsync(e.Movie);
        }
    }

    private async void MovieInfoBox_RateClicked(object sender, Controls.MovieEventArgs e)
    {
        if (_viewModel.RateMovieCommand.CanExecute(e.Movie))
        {
            await _viewModel.RateMovieCommand.ExecuteAsync(e.Movie);
        }
    }

}