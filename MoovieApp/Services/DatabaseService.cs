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
        private SQLiteAsyncConnection? _database;
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "moovieapp.db3");

        private readonly Lazy<Task<SQLiteAsyncConnection>> _databaseLazyInitializer;
        public DatabaseService()
        {
            _databaseLazyInitializer = new Lazy<Task<SQLiteAsyncConnection>>(async () =>
            {               
                var database = new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
                await InitDatabaseAsync(database); 
                return database;
            });
        }

        private async Task InitDatabaseAsync(SQLiteAsyncConnection database)
        {
            await database.CreateTableAsync<User>();
            await database.CreateTableAsync<MovieObject>();
            await database.CreateTableAsync<Rating>();
            await database.CreateTableAsync<UserList>();
            await database.CreateTableAsync<Actor>();
            await database.CreateTableAsync<Director>();
            await database.CreateTableAsync<MovieActor>();
            await database.CreateTableAsync<MovieDirector>();
        }

        private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
           
            return _database ??= await _databaseLazyInitializer.Value;
        }

        public async Task AddMovieToListAsync(int userId, int movieId, string title, string posterUrl)
        {
            var db = await GetDatabaseAsync();

            var movie = new MovieObject
            {
                movie_id = movieId,
                title = title,
                poster_url = posterUrl
            };
            
            await db.InsertOrReplaceAsync(movie); 

            var listItem = new UserList
            {
                user_id = userId,
                movie_id = movieId
            };

            
            var existing = await db.Table<UserList>()
                                    .Where(x => x.user_id == userId && x.movie_id == movieId)
                                    .FirstOrDefaultAsync();
            if (existing == null)
            {
                
                await db.InsertAsync(listItem);
            }
        }

        public async Task RateMovieAsync(int userId, int movieId, int rating, string ratingText = null)
        {
            var db = await GetDatabaseAsync();

            var newRating = new Rating
            {
                user_id = userId,
                movie_id = movieId,
                rating = rating,
                rating_text = ratingText
            };
           
            await db.InsertOrReplaceAsync(newRating);
        }

        public async Task<List<MovieObject>> GetUserListAsync(int userId)
        {
            var db = await GetDatabaseAsync(); 

            var userListItems = await db.Table<UserList>()         
                                         .Where(x => x.user_id == userId)
                                         .ToListAsync();

            var movieIds = userListItems.Select(x => x.movie_id).ToList();

            return await db.Table<MovieObject>()
                            .Where(m => movieIds.Contains(m.movie_id))
                            .ToListAsync();
        }
    }
}
