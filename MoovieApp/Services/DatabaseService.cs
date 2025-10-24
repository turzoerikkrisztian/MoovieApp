using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoovieApp.Models;

namespace MoovieApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "moovieapp.db3");

        public DatabaseService()
        {
            _database = new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            InitAsync().Wait();
        }

        private async Task InitAsync()
        {           
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<MovieObject>();
            await _database.CreateTableAsync<Rating>();
            await _database.CreateTableAsync<UserList>();
            await _database.CreateTableAsync<Actor>();
            await _database.CreateTableAsync<Director>();
            await _database.CreateTableAsync<MovieActor>();
            await _database.CreateTableAsync<MovieDirector>();
        }

       
        public async Task AddMovieToListAsync(int userId, int movieId, string title, string posterUrl)
        {            
            var movie = new MovieObject
            {
                movie_id = movieId,
                title = title,
                poster_url = posterUrl
            };
            await _database.InsertOrReplaceAsync(movie);

            var listItem = new UserList
            {
                user_id = userId,
                movie_id = movieId
            };

            var existing = await _database.Table<UserList>()
                                        .Where(x => x.user_id == userId && x.movie_id == movieId)
                                        .FirstOrDefaultAsync();
            if (existing == null)
            {
                await _database.InsertAsync(listItem);
            }
        }

        public async Task RateMovieAsync(int userId, int movieId, int rating, string ratingText = null)
        {
            var newRating = new Rating
            {   
                user_id = userId,
                movie_id = movieId,
                rating = rating,
                rating_text = ratingText
            };
            await _database.InsertOrReplaceAsync(newRating);
        }

        public async Task<List<MovieObject>> GetUserListAsync(int userId)
        {
            var userListItems = await _database.Table<UserList>()
                                             .Where(x => x.user_id == userId)
                                             .ToListAsync();
        
            var movieIds = userListItems.Select(x => x.movie_id).ToList();
            
            return await _database.Table<MovieObject>()
                                .Where(m => movieIds.Contains(m.movie_id))
                                .ToListAsync();
        }
    }
}
