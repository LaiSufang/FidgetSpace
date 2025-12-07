using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string LoggedInUser { get; set; }
        public int LoggedInUserId { get; set; }

        [Unique]
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        // music setting
        public bool MusicEnabled { get; set; } = false;
        public double MusicVolume { get; set; } = 0.5;

        // ==== New: Total Game Duration Related ====
        // Total duration of all games (seconds)
        public int TotalTimePlayedSeconds { get; set; } = 0;

        // Bubble Game Total Duration (seconds)
        public int TotalTimeBubbleSeconds { get; set; } = 0;

        // Red / Blue Pill Game Total Playtime
        public int TotalTimePillSeconds { get; set; } = 0;

        // Connect the Dots Game Total Duration (seconds)
        public int TotalTimeDotSeconds { get; set; } = 0;
    }
}
