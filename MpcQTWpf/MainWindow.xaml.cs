using Microsoft.Win32;
using System;
using System.Windows;
using MpcQtWpf.Services;
// Add this for InputBox
using Microsoft.VisualBasic;

namespace MpcQtWpf
{
    public partial class MainWindow : Window
    {
        private readonly MediaPlayerService _player;

        public MainWindow()
        {
            InitializeComponent();
            _player = new MediaPlayerService();

            Loaded += (s, e) =>
            {
                var handle = VideoPanel.Handle;
                _player.AttachToWindowHandle(handle);
                StatusText.Text = "mpv attached";
            };
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Media files|*.mp4;*.mkv;*.avi;*.mov;*.mp3;*.flac;*.wav|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                _player.LoadFile(dlg.FileName);
                StatusText.Text = $"Playing: {dlg.FileName}";
            }
        }

        private void CycleAudioTrack_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
            {
                _player.CycleAudioTrack();
                StatusText.Text = "Audio track changed";
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e) => _player.TogglePause();

        private void Stop_Click(object sender, RoutedEventArgs e) => _player.Stop();

        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            // Simple string input using InputBox
            string url = Interaction.InputBox("Enter a media URL:", "Open URL", "");
            if (!string.IsNullOrWhiteSpace(url))
            {
                _player.LoadFile(url);
                StatusText.Text = $"Streaming: {url}";
            }
        }

        private void SeekBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_player != null)
            {
                double newPosition = e.NewValue;
                _player.Seek(newPosition);
                StatusText.Text = $"Seeked to {newPosition:F1} seconds";
            }
        }

        private void ToggleHwAccel_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
            {
                _player.ToggleHwAccel();
                StatusText.Text = "Hardware acceleration toggled";
            }
        }

        private void ToggleStats_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
            {
                _player.ToggleStats();
                StatusText.Text = "Stats overlay toggled";
            }
        }

        private void ToggleSubtitles_Click(object sender, RoutedEventArgs e)
        {
            if (_player != null)
            {
                _player.ToggleSubtitles();
                StatusText.Text = "Subtitles toggled";
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_player != null)
            {
                double newVolume = e.NewValue;
                _player.SetVolume(newVolume);
                StatusText.Text = $"Volume set to {newVolume:F0}%";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _player.Dispose();
        }
    }
}