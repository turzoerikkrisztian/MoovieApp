using CommunityToolkit.Mvvm.ComponentModel;
using MoovieApp.Models;
using MoovieApp.Services;

namespace MoovieApp.ViewModels
{
    [QueryProperty(nameof(MovieModel), nameof(MovieModel))]
    public partial class DetailViewModel : ObservableObject
    {
        private readonly TmdbService _tmdbService;
        public DetailViewModel(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public MovieModel Movie { get; set; }
    }   
}
