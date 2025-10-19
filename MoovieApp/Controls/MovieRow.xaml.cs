using MoovieApp.Models;
using MoovieApp.ViewModels;
using System.Windows.Input;

namespace MoovieApp.Controls;

public class MovieSelectEventArgs: EventArgs
{
	public MovieModel MovieModel { get; set; }

	public MovieSelectEventArgs(MovieModel movieModel)
    {
        MovieModel = movieModel;
    }
}

public partial class MovieRow : ContentView
{
	public static readonly BindableProperty 
		HeadingProperty = BindableProperty
		.Create(nameof(Heading), typeof(string), 
			typeof(MovieRow), string.Empty);

    public static readonly BindableProperty
        MoviesProperty = BindableProperty
        .Create(nameof(Movies), typeof(IEnumerable<MovieModel>),
            typeof(MovieRow), Enumerable.Empty<MovieModel>());

	public event EventHandler<MovieSelectEventArgs> OnMovieSelected;

    
    public MovieRow()
	{
		InitializeComponent();
        MovieDetailsCommand = new Command(ExecuteMovieDetailsCommand);

    }

	public string Heading
	{
		get => (string)GetValue(MovieRow.HeadingProperty);
		set => SetValue(MovieRow.HeadingProperty, value);
	}

    public IEnumerable<MovieModel> Movies
    {
        get => (IEnumerable<MovieModel>)GetValue(MovieRow.MoviesProperty);
        set => SetValue(MovieRow.MoviesProperty, value);
    }

    public ICommand MovieDetailsCommand { get; private set; }

    private void ExecuteMovieDetailsCommand(object parameter)
    {
        if (parameter is MovieModel movie && movie is not null)
        {
            OnMovieSelected?.Invoke(this, new MovieSelectEventArgs(movie));
        }
    }

    
}