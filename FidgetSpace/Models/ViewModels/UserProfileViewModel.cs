using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using FidgetSpace.Models;
using System;

namespace FidgetSpace.Models.ViewModels
{
    public partial class UserProfileViewModel : ObservableObject
    {
        // This is the ACTUAL User object used for binding
        [ObservableProperty]
        private User user;

        // Last pill game duration (seconds)
        [ObservableProperty]
        private int lastGameDuration;

        //Last pill color choice ("Red" / "Blue")
        [ObservableProperty]
        private string lastPillChoice;

        // Last time user played any game
        [ObservableProperty]
        private DateTime? lastGamePlayed;

        // Constructor receives the logged-in user instance
        public UserProfileViewModel(User userLoginInfo)
        {
            User = userLoginInfo;

            // Load last game info from database
            if (User != null)
            {
                LastGameDuration = User.LastGameDuration;
                LastPillChoice = User.LastPillChoice;
                LastGamePlayed = User.LastGamePlayed;
            }
        }
    }
}

