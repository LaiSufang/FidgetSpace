using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidgetSpace.Services
{
    public class MusicService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _player;
        public bool IsPlaying => _player?.IsPlaying ?? false;

        public MusicService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task LoadAsync()
        {
            if (_player != null) return;

            var file = await FileSystem.OpenAppPackageFileAsync("relaxing.mp3");
            _player = _audioManager.CreatePlayer(file);
            _player.Loop = true;
        }

        public async Task Play()
        {
            await LoadAsync();
            _player?.Play();
        }

        public void Pause()
        {
            _player?.Pause();
        }

        public void SetVolume(double volume)
        {
            if (_player != null)
                _player.Volume = (float)volume;
        }
    }
}

