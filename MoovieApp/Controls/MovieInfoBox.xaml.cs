using MoovieApp.Models;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;
using MoovieApp.Pages;
using MoovieApp.ViewModels;

namespace MoovieApp.Controls;


public class MovieEventArgs : EventArgs
{
    public MovieModel Movie { get; }
    public MovieEventArgs(MovieModel movie) { Movie = movie; }
}


public partial class MovieInfoBox : ContentView
{
	public static readonly BindableProperty MovieProperty =
		BindableProperty.Create(nameof(Movie), typeof(MovieModel),
		typeof(MovieInfoBox), defaultValue: null);

    public MovieModel Movie
    {
        get => (MovieModel)GetValue(MovieInfoBox.MovieProperty);
        set => SetValue(MovieInfoBox.MovieProperty, value);
    }

    public event EventHandler Closed;
    public event EventHandler<MovieEventArgs>? AddToListClicked;
    public event EventHandler<MovieEventArgs>? RateClicked;

    public ICommand ClosedCommand { get; private set; }

    public MovieInfoBox()
	{
		InitializeComponent();
		ClosedCommand = new Command(ExecuteClosedCommand);
	}
	   
    private void ExecuteClosedCommand()
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private async void ImageButton_Details_Clicked(object sender, EventArgs e)
    {
        if (Movie is null) return;
        var parameters = new Dictionary<string, object> { [nameof(DetailViewModel.Movie)] = Movie };
        await Shell.Current.GoToAsync(nameof(MovieDetailsPage), true, parameters);
    }

    public static readonly BindableProperty IsRemoveModeProperty =
        BindableProperty.Create(nameof(IsRemoveMode), typeof(bool),
        typeof(MovieInfoBox), defaultValue: false);

    public bool IsRemoveMode
    {
        get => (bool)GetValue(IsRemoveModeProperty);
        set => SetValue(IsRemoveModeProperty, value);
    }

    public event EventHandler<MovieEventArgs>? RemoveClicked;

    private void AddOrRemoveList_Clicked(object sender, EventArgs e)
    {
        if (Movie is null) return;
        if (IsRemoveMode)
        {
            RemoveClicked?.Invoke(this, new MovieEventArgs(Movie));
        }
        else
        {
            AddToListClicked?.Invoke(this, new MovieEventArgs(Movie));
        }       
    }

    private void Rate_Clicked(object sender, EventArgs e)
    {
        if (Movie is not null)
        {
            RateClicked?.Invoke(this, new MovieEventArgs(Movie));
        }
    }

    
}