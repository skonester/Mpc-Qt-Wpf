using Microsoft.VisualBasic;
using Microsoft.Win32;
using MpcQtWpf.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace MpcQtWpf
{
    public partial class MainWindow : Window
    {
        private readonly MediaPlayerService _player;
        private readonly DispatcherTimer _seekTimer = new DispatcherTimer();
        private bool _isDragging = false;

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

            _seekTimer.Interval = TimeSpan.FromMilliseconds(500);
            _seekTimer.Tick += (s, e) =>
            {
                if (_player != null && !_isDragging)
                {
                    double pos = _player.GetPosition();
                    double dur = _player.GetDuration();
                    if (dur > 0)
                    {
                        SeekBar.Maximum = dur;
                        SeekBar.Value = pos;
                    }
                }
            };
            _seekTimer.Start();
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
                SeekBar.Value = 0;
                SeekBar.Maximum = _player.GetDuration();
            }
        }

        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            string url = Interaction.InputBox("Enter a media URL:", "Open URL", "");
            if (!string.IsNullOrWhiteSpace(url))
            {
                // Use yt-dlp to resolve the direct stream URL
                var psi = new ProcessStartInfo
                {
                    FileName = "yt-dlp.exe", // ensure yt-dlp.exe is in your output folder or PATH
                    Arguments = $"--get-url -f best {url}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                string streamUrl = null;
                using (var process = Process.Start(psi))
                {
                    streamUrl = process.StandardOutput.ReadLine();
                    process.WaitForExit();
                }

                if (!string.IsNullOrEmpty(streamUrl))
                {
                    _player.LoadFile(streamUrl);
                    StatusText.Text = $"Streaming: {url}";
                    SeekBar.Value = 0;
                    SeekBar.Maximum = _player.GetDuration();
                }
                else
                {
                    StatusText.Text = "Failed to resolve stream URL.";
                }
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e) => _player.TogglePause();
        private void Stop_Click(object sender, RoutedEventArgs e) => _player.Stop();

        private void CycleAudioTrack_Click(object sender, RoutedEventArgs e)
        {
            _player.CycleAudioTrack();
            StatusText.Text = "Audio track changed";
        }

        private void ToggleSubtitles_Click(object sender, RoutedEventArgs e)
        {
            _player.ToggleSubtitles();
            StatusText.Text = "Subtitles toggled";
        }

        private void ToggleHwAccel_Click(object sender, RoutedEventArgs e)
        {
            _player.ToggleHwAccel();
            StatusText.Text = "Hardware acceleration toggled";
        }

        private void ToggleStats_Click(object sender, RoutedEventArgs e)
        {
            _player.ToggleStats();
            StatusText.Text = "Stats overlay toggled";
        }

        private void SeekBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging) return; // only seek when drag completes
        }

        private void SeekBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void SeekBar_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            _player.Seek(SeekBar.Value);
            StatusText.Text = $"Seeked to {SeekBar.Value:F1} seconds";
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_player != null && IsLoaded)
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