using MoovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Services
{
    public class RecommendationService
    {
        private readonly HttpClient _httpClient;
        private const string Baseurl = "http://10.0.2.2:5000";

        public RecommendationService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(Baseurl)};
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<List<int>> GetRecommendationAsync(List<MovieModel> likedMovies, List<MovieModel> candidateMovies)
        {
            try
            {
                var requestData = new
                {
                    liked_movies = likedMovies.Select(m => new { id = m.Id, overview = m.Overview ?? "", genres = "" }).ToList(),
                    candidate_movies = candidateMovies.Select(m => new { id = m.Id, overview = m.Overview ?? "", genres = "" }).ToList()
                };

                var response = await _httpClient.PostAsJsonAsync("/recommend", requestData);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RecommendationResponse>();
                    return result?.Recommendations ?? new List<int>();
                }
            }
            catch (Exception ex)
            {
               System.Diagnostics.Debug.WriteLine($"Error fetching recommendations: {ex.Message}");
            }   
            return new List<int>();
        }
        private class RecommendationResponse
        {
            public List<int> Recommendations { get; set; }};
        }
    }

