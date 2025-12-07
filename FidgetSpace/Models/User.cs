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

        // Basic account info
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Total game time statistics (seconds)
        public int TotalTimePlayedSeconds { get; set; }
        public int TotalTimeBubbleSeconds { get; set; }
        public int TotalTimeDotSeconds { get; set; }
        public int TotalTimePillSeconds { get; set; }

        // Last Pill Game Info
        public int LastGameDuration { get; set; }        // Duration of last pill game
        public string LastPillChoice { get; set; } = ""; // "Red" or "Blue"
        public DateTime? LastGamePlayed { get; set; }    // Last time played pill game

        // Background Music Settings
        public bool MusicEnabled { get; set; } = true;   // Music ON by default
        public double MusicVolume { get; set; } = 0.5;   // 50% volume by default
    }
}
