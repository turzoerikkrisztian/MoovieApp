using MoovieApp.ViewModels;


namespace MoovieApp.Pages;

public partial class OnborardingPage : ContentPage
{
	private readonly OnboardingViewModel _viewModel;

    public OnborardingPage(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitalizeAsync();
    }
}