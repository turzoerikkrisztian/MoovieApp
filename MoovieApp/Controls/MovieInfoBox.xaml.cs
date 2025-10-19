using MoovieApp.Models;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;
using MoovieApp.Pages;
using MoovieApp.ViewModels;

namespace MoovieApp.Controls;

public partial class MovieInfoBox : ContentView
{
	public static readonly BindableProperty MovieProperty =
		BindableProperty.Create(nameof(Movie), typeof(MovieModel),
		typeof(MovieInfoBox), defaultValue: null);

	public event EventHandler Closed;

    public MovieInfoBox()
	{
		InitializeComponent();
		ClosedCommand = new Command(ExecuteClosedCommand);
	}

	public MovieModel Movie
	{
		get => (MovieModel)GetValue(MovieInfoBox.MovieProperty);
        set => SetValue(MovieInfoBox.MovieProperty, value);
    }


    public ICommand ClosedCommand { get; private set; }

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
        await Shell.Current.GoToAsync($"{nameof(MovieDetailsPage)}",
            new Dictionary<string, object>
            {
                { "MovieViewModel", new MovieDetailsViewModel(Movie) }
            });
    }
}