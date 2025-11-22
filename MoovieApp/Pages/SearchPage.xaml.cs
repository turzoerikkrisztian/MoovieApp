using MoovieApp.ViewModels;


namespace MoovieApp.Pages;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}