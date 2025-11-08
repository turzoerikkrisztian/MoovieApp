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
}