using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoovieApp.Models;
using MoovieApp.Services;
using MoovieApp.Pages;
using System.Collections.ObjectModel;

namespace MoovieApp.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;

        public ObservableCollection<MovieModel> SearchResults { get; }

        public SearchViewModel(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
            SearchResults = new ObservableCollection<MovieModel>();
        }

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isBusy;

        public ObservableCollection<MovieModel> SearchResult { get; }

        [RelayCommand]
        private async Task SearchMoviesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SearchResults.Clear();
                return;
            }

            IsBusy = true;
            try
            {
                SearchResults.Clear();

                var titleSearchTask = _tmdbService.SearchMoviesAsync(SearchText);

                var keywordId = await _tmdbService.GetKeywordIdAsync(SearchText);

                IEnumerable<MovieModel> keywordResults = Enumerable.Empty<MovieModel>();

                if (keywordId.HasValue)
                {
                    keywordResults = await _tmdbService.GetMoviesByKeywordAsync(keywordId.Value);
                }

                var titleResults = await titleSearchTask;

                var combinedResults = new List<MovieModel>();

                if (titleResults != null) combinedResults.AddRange(titleResults);
                if (keywordResults != null) combinedResults.AddRange(keywordResults);

                var uniqueResults = combinedResults
                    .GroupBy(m => m.Id)
                    .Select(g => g.First())
                    .ToList();

                foreach (var movie in uniqueResults)
                {
                    SearchResults.Add(movie);
                }

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"An error occurred while searching for movies: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoDetailAsync(MovieModel movie)
        {
            if (movie == null) return;

            var parameters = new Dictionary<string, object>
            {
                [nameof(DetailViewModel.Movie)] = movie
            };

            await Shell.Current.GoToAsync(nameof(MovieDetailsPage), true, parameters);
        }
    }
}
