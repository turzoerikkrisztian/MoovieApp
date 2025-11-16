using MoovieApp.ViewModels;


namespace MoovieApp.Pages;

public partial class ForYouPage : ContentPage
{
    private readonly ForYouViewModel _viewModel;
    public ForYouPage(ForYouViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}