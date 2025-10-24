using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoovieApp.Models
{
    [Table("Ratings")]
    public class Rating
    {
        [PrimaryKey, AutoIncrement]
        public int rating_id { get; set; } 

        [Indexed] 
        public int movie_id { get; set; }

        [Indexed] 
        public int user_id { get; set; }
        public int rating { get; set; }
        public string rating_text { get; set; }
    }
}
