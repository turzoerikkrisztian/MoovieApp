using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Services;
using System.Collections.ObjectModel;

namespace MoovieApp.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        public HomeViewModel(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }


        [ObservableProperty]
        private MovieModel _trendingMovie;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowMovieInfoBox))]
        private MovieModel? _selectedMovie;

        public bool ShowMovieInfoBox => SelectedMovie is not null;

        


        public ObservableCollection<MovieModel> TrendingMovies { get; set; } = new();
        public ObservableCollection<MovieModel> TopRatedMovies { get; set; } = new();
        public ObservableCollection<MovieModel> MyList { get; set; } = new();


        public async Task InitalizeAsync()
        {
            var trendingMoviesTask = _tmdbService.GetTrendingMoviesAsync();
            var topRatedMoviesTask = _tmdbService.GetTopRatedMoviesAsync();

            var movieModels = await Task.WhenAll(trendingMoviesTask,
                                                 topRatedMoviesTask);

            var trendingMovies = movieModels[0];
            var topRatedMovies = movieModels[1];

            TrendingMovie = trendingMovies.OrderBy(t => Guid.NewGuid())
                                .FirstOrDefault(t =>
                                    !string.IsNullOrWhiteSpace(t.DisplayTitle)
                                    && !string.IsNullOrWhiteSpace(t.Thumbnail));

            SetMovieCollection(trendingMovies, TrendingMovies);
            SetMovieCollection(topRatedMovies, TopRatedMovies);

            //SelectedMovie = TrendingMovie;

        }

        private static void SetMovieCollection(IEnumerable<MovieModel> movies,
            ObservableCollection<MovieModel> collection)
        {
            collection.Clear();
            {
                foreach (var movie in movies)
                {
                    collection.Add(movie);
                }
            }
        }

        [RelayCommand]
        private void SelectMovie(MovieModel? movie = null)
        {
            if (movie is not null)
            {
                if (movie.Id == SelectedMovie?.Id)
                {
                    movie = null;
                }
            }
            SelectedMovie = movie;
        }
             

    }
}
