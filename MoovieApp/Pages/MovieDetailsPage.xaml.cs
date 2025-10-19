using MoovieApp.ViewModels;

namespace MoovieApp.Pages;

public partial class MovieDetailsPage : ContentPage
{
    private readonly DetailViewModel _viewModel;
    public MovieDetailsPage(DetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}