using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool MusicEnabled { get; set; }

        public double MusicVolume { get; set; }

        // Measured in seconds
        public double TotalBubbleGameTime { get; set; }
        public double TotalConnectGameTime { get; set; }
        public double TotalDotGameTime { get; set; }
    }
}
