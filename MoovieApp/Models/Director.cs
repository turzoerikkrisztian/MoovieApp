using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoovieApp.Models
{
    [Table("Directors")]
    public class Director
    {
        [PrimaryKey] 
        public int director_id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public DateTime? birthday { get; set; }
        public string biography { get; set; }
        public string profile_url { get; set; }
    }
}
