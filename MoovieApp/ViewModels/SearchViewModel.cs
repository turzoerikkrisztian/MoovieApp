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
                var result = await _tmdbService.SearchMoviesAsync(SearchText);

                SearchResults.Clear();

                if (result != null)
                {
                    foreach (var m in result)
                    {
                        SearchResults.Add(m);
                    }
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
