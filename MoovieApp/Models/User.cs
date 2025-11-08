using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int user_id { get; set; } 
        public string username { get; set; }
        [Unique] //
        public string email { get; set; }
        public string password { get; set; }
        public string preferences { get; set; }
    }
}
