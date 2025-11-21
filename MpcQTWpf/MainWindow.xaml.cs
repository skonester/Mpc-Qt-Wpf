using Microsoft.Win32;
using System;
using System.Windows;
using MpcQtWpf.Services;

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
        private void PlayPause_Click(object sender, RoutedEventArgs e) => _player.TogglePause();
        private void Stop_Click(object sender, RoutedEventArgs e) => _player.Stop();
        protected override void OnClosed(EventArgs e) { base.OnClosed(e); _player.Dispose(); }
    }
}
