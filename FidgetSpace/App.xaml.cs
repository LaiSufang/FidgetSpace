using FidgetSpace.Models;
using FidgetSpace.Services;

namespace FidgetSpace
{
    public partial class App : Application

    {
        public static DatabaseService Database { get; private set; }
        public static User LoggedInUser { get; set; }
        public App()
        {
            InitializeComponent();

            Database = new DatabaseService();

            MainPage = new AppShell();
        }
    }
}
