using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("Movies")]
    public class MovieObject
    {
        [PrimaryKey] 
        public int movie_id { get; set; }
        public string title { get; set; }
        public string genre { get; set; } 
        public int release_year { get; set; } 
        public string poster_url { get; set; }
        public string overview { get; set; }
    }
}
