using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FidgetSpace.Models
{
    /// <summary>
    /// This class represents a single dot in the dot-connecting game.
    /// It stores the dot's: position, color, selected state, visibility, and whether it's a Power dot.
    /// Implementing INotifyPropertyChanged ensures the UI automatically refreshes when properties change.
    /// </summary>
    public class ConnectDotsCell : INotifyPropertyChanged
    {
        // The number assigned to each point (for debugging or future expansion)
        public int Id { get; set; }

        // Which line is the point on?
        public int Row { get; set; }

        // Which column is the point in?
        public int Col { get; set; }

        // ================= Below are the properties bound to the UI. =================

        // The color of the point (bound to Frame.BackgroundColor)
        private string colorName; // use for store data
        public string ColorName     // use for acess and API
        {
            get { return colorName; }
            set
            {
                if (colorName != value)
                {
                    colorName = value;
                    OnPropertyChanged();   // Notification UI: Color changed
                }
            }
        }

        // Whether selected (a white border appears around the UI element when selected)
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();   // Notification UI: Selected state changed
                }
            }
        }

        // Visible (After successful pairing, we will set this to false to make the dot disappear from the UI)
        private bool isVisible = true; // Display all by default
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged();   // Notification UI: Show Change
                }
            }
        }

        // ================= INotifyPropertyChanged =================

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Trigger property change event
        /// [CallerMemberName]: If no parameter is passed, the current property name is automatically used
        /// </summary>
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
