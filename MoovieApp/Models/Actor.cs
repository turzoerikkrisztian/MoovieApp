using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("Actors")]
    public class Actor
    {
        [PrimaryKey] 
        public int actor_id { get; set; }
        public string name { get; set; }
        public string gender { get; set; } 
        public DateTime? birthday { get; set; }
        public string biography { get; set; }
        public string profile_url { get; set; } 
    }
}
