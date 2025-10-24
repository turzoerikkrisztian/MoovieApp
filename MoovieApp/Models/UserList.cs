using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("Lists")]
    public class UserList
    {
        [PrimaryKey, AutoIncrement]
        public int list_id { get; set; }

        [Indexed]
        public int movie_id { get; set; }

        [Indexed]
        public int user_id { get; set; }
    }
}
