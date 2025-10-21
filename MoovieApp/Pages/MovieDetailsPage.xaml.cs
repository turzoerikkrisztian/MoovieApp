using MoovieApp.Models;
using MoovieApp.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Controls; 



namespace MoovieApp.Pages;

public partial class MovieDetailsPage : ContentPage
{
    //private readonly DetailViewModel _viewModel;

    //public static readonly BindableProperty MovieProperty =
    //    BindableProperty.Create(nameof(Movie), typeof(MovieModel),
    //    typeof(MovieDetailsPage), defaultValue: null);

    //public MovieModel Movie
    //{
    //    get => (MovieModel)GetValue(MovieDetailsPage.MovieProperty);
    //    set => SetValue(MovieDetailsPage.MovieProperty, value);
    //}

    //public MovieDetailsPage(DetailViewModel viewModel)
    //{
    //    InitializeComponent();
    //    _viewModel = viewModel;
    //    BindingContext = _viewModel;
    //}

    //protected async override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await _viewModel.InitializeAsyns();
    //}

    //protected override void OnSizeAllocated(double width, double height)
    //{
    //    base.OnSizeAllocated(width, height);
    //    if (width > 0)
    //    {
    //        _viewModel.SimilarItemWidth = Convert.ToInt32(width / 3);
    //    }
    //}

    //private async void ImageButton_Clicked(object sender, EventArgs e)
    //{
    //    var parameters = new Dictionary<string, object>
    //    {
    //        [nameof(DetailViewModel.Movie)] = Movie
    //    };
    //    await Shell.Current.GoToAsync(nameof(MovieDetailsPage), true, parameters);
    //}
    //private async void SimilarMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    var selected = e.CurrentSelection?.FirstOrDefault() as MovieModel;
    //    if (selected is null)
    //        return;

    //    // Deselect in UI if you want:
    //    if (sender is CollectionView cv)
    //        cv.SelectedItem = null;

    //    var parameters = new Dictionary<string, object>
    //    {
    //        [nameof(Movie)] = selected
    //    };

    //    await Shell.Current.GoToAsync(nameof(MovieDetailsPage), true, parameters);
    //}

    //private static void OnMoviePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var page = (MovieDetailsPage)bindable;
    //    if (page._viewModel is not null)
    //    {
    //        page._viewModel.Movie = newValue as MovieModel;
    //    }
    //}

    private readonly DetailViewModel _viewModel;

    public static readonly BindableProperty MovieProperty =
        BindableProperty.Create(nameof(Movie), typeof(MovieModel),
        typeof(MovieDetailsPage), defaultValue: null, propertyChanged: OnMoviePropertyChanged);

    public MovieModel Movie
    {
        get => (MovieModel)GetValue(MovieDetailsPage.MovieProperty);
        set => SetValue(MovieDetailsPage.MovieProperty, value);
    }

    public MovieDetailsPage(DetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel?.Movie is not null)
        {
            await _viewModel.InitializeAsyns();
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (width > 0)
        {
            _viewModel.SimilarItemWidth = Convert.ToInt32(width / 3);
        }
    }

   
    private async void SimilarMovie_Clicked(object sender, EventArgs e)
    {
        MovieModel? movie = null;

        if (sender is ImageButton ib)
        {
           
            if (ib.CommandParameter is MovieModel cpMovie)
                movie = cpMovie;
            else if (ib.BindingContext is MovieModel bcMovie)
                movie = bcMovie;
        }

        if (movie is null)
            return;

        var parameters = new Dictionary<string, object>
        {
            [nameof(DetailViewModel.Movie)] = movie
        };
        var nav = Shell.Current?.Navigation;
        if (nav != null && nav.NavigationStack?.Count > 1)
        {
            // The last entry is the current page (this). Remove it so back goes to root/home.
            var current = nav.NavigationStack.LastOrDefault();
            if (current != null && current != nav.NavigationStack.First())
            {
                nav.RemovePage(current);
            }
        }
        await Shell.Current.GoToAsync(nameof(MovieDetailsPage), true, parameters);
    }

    private static void OnMoviePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var page = (MovieDetailsPage)bindable;
        if (page._viewModel is not null)
        {
            page._viewModel.Movie = newValue as MovieModel;
        }
    }
}