using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("MovieActors")]
    public class MovieActor
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int actor_id { get; set; }

        [Indexed]
        public int movie_id { get; set; }

        public string role { get; set; }
    }
}
