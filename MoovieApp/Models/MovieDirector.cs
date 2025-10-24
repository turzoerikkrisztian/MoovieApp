using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("MovieDirectors")]
    public class MovieDirector
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 

        [Indexed]
        public int director_id { get; set; }

        [Indexed]
        public int movie_id { get; set; }
    }
}
