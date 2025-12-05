using FidgetSpace.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Views
{
    public partial class Signin : ContentPage
    {
        public Signin()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();

        }
    }

}
