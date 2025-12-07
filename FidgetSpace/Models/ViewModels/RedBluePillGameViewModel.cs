using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Diagnostics;
using System.Timers;
using FidgetSpace.Services;

namespace FidgetSpace.Models.ViewModels
{
    public partial class RedBluePillGameViewModel : ObservableObject, IDisposable
    {
        private readonly DatabaseService _db;

        readonly Stopwatch stopwatch = new();
        readonly System.Timers.Timer timer = new(1000);

        // Raised every tick with the *current* remaining seconds (after decrement).
        public event Action<int>? TimeUpdated;

        // Raised when countdown reaches zero.
        public event Action? TimeExpired;

        public RedBluePillGameViewModel()
        {
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }
        
        [ObservableProperty]
        private int remainingSeconds;

        // Computed property for elapsed seconds (stopwatch); raise notifications manually.
        public int TimeSpentSeconds => (int)stopwatch.Elapsed.TotalSeconds;

        // Start the countdown and stopwatch. Passing seconds = 0 will not start the timer.
        public void Start(int seconds)
        {
            Stop(); // ensure a clean state

            if (seconds <= 0)
            {
                RemainingSeconds = 0;
                OnPropertyChanged(nameof(TimeSpentSeconds));
                return;
            }

            RemainingSeconds = seconds;

            stopwatch.Reset();
            stopwatch.Start();

            // notify initial values
            OnPropertyChanged(nameof(TimeSpentSeconds));
            TimeUpdated?.Invoke(RemainingSeconds);

            timer.Start();
            OnPropertyChanged(nameof(IsRunning));
        }

        // Stop timer and stopwatch but keep current elapsed/remaining values.
        public void Stop()
        {
            if (timer.Enabled)
                timer.Stop();

            if (stopwatch.IsRunning)
                stopwatch.Stop();

            OnPropertyChanged(nameof(TimeSpentSeconds));
            OnPropertyChanged(nameof(IsRunning));
        }

        // Reset both countdown and stopwatch to zero.
        public void Reset()
        {
            Stop();
            stopwatch.Reset();
            RemainingSeconds = 0;
            OnPropertyChanged(nameof(TimeSpentSeconds));
            TimeUpdated?.Invoke(RemainingSeconds);
        }

        public bool IsRunning => timer.Enabled;

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // Timer runs on a threadpool thread; marshal updates to UI thread.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Defensive: if not running anymore, ignore.
                if (!timer.Enabled)
                    return;

                RemainingSeconds--;

                // Notify consumers and UI
                TimeUpdated?.Invoke(RemainingSeconds);
                OnPropertyChanged(nameof(TimeSpentSeconds));

                if (RemainingSeconds <= 0)
                {
                    // Stop and signal expiration
                    Stop();
                    TimeExpired?.Invoke();
                }
            });
        }

        public void Dispose()
        {
            try
            {
                timer.Elapsed -= Timer_Elapsed;
                timer.Stop();
                timer.Dispose();
            }
            catch
            {
                // swallow - nothing to do during dispose failure
            }

            if (stopwatch.IsRunning)
                stopwatch.Stop();
        }
       
    }
}