using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Models.ViewModels;

partial class BubbleWrapPopViewModel : ObservableObject
{
    [ObservableProperty]
    private int score; 

    [ObservableProperty]
    private int highScore;

    [ObservableProperty]
    private double totalTime;
}
