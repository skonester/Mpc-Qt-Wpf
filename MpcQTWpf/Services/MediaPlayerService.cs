using System;
using MpcQtWpf.Interop;

namespace MpcQtWpf.Services
{
    public class MediaPlayerService : IDisposable
    {
        private IntPtr _mpv = IntPtr.Zero;
        private bool _initialized = false;

        public MediaPlayerService()
        {
            _mpv = MpvInterop.mpv_create();
            if (_mpv == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create mpv instance.");

            MpvInterop.mpv_set_option_string(_mpv, "log-file", "mpv.log");
            MpvInterop.mpv_set_option_string(_mpv, "msg-level", "all=v");

            int init = MpvInterop.mpv_initialize(_mpv);
            if (init < 0)
                throw new InvalidOperationException($"mpv_initialize failed: {init}");

            _initialized = true;
        }

        public void AttachToWindowHandle(IntPtr hwnd)
        {
            if (!_initialized) throw new InvalidOperationException("mpv not initialized");
            long wid = hwnd.ToInt64();
            MpvInterop.mpv_set_option(_mpv, "wid", MpvInterop.mpv_format.MPV_FORMAT_INT64, ref wid);
        }

        public void LoadFile(string pathOrUrl)
        {
            string escaped = pathOrUrl.Replace("\\", "\\\\");
            MpvInterop.mpv_command_string(_mpv, $"loadfile \"{escaped}\"");

            // Explicitly unpause so playback begins
            MpvInterop.mpv_command_string(_mpv, "set pause no");
        }

        public void TogglePause() => MpvInterop.mpv_command_string(_mpv, "cycle pause");
        public void Stop() => MpvInterop.mpv_command_string(_mpv, "stop");

        public void CycleAudioTrack() => MpvInterop.mpv_command_string(_mpv, "cycle audio");
        public void ToggleSubtitles() => MpvInterop.mpv_command_string(_mpv, "cycle sub");
        public void ToggleStats() => MpvInterop.mpv_command_string(_mpv, "script-message stats-toggle");

        /// <summary>
        /// Seek to an absolute position in seconds.
        /// </summary>
        public void Seek(double positionSeconds)
        {
            MpvInterop.mpv_command_string(_mpv, $"seek {positionSeconds} absolute+exact");
        }

        public void ToggleHwAccel()
        {
            MpvInterop.mpv_command_string(_mpv, "set hwdec auto");
        }

        public void SetVolume(double volumePercent)
        {
            MpvInterop.mpv_command_string(_mpv, $"set volume {volumePercent}");
        }

        /// <summary>
        /// Current playback position in seconds.
        /// </summary>
        public double GetPosition()
        {
            double pos = 0;
            int err = MpvInterop.mpv_get_property(_mpv, "time-pos", MpvInterop.mpv_format.MPV_FORMAT_DOUBLE, ref pos);

            if (err < 0 || pos < 0.01)
            {
                // fallback: try playback-time
                err = MpvInterop.mpv_get_property(_mpv, "playback-time", MpvInterop.mpv_format.MPV_FORMAT_DOUBLE, ref pos);
            }

            return (err >= 0 && pos >= 0) ? pos : 0.0;
        }

        /// <summary>
        /// Total duration of the loaded file in seconds.
        /// </summary>
        public double GetDuration()
        {
            double dur = 0;
            int err = MpvInterop.mpv_get_property(_mpv, "duration", MpvInterop.mpv_format.MPV_FORMAT_DOUBLE, ref dur);
            return (err >= 0 && dur > 0) ? dur : 0.0;
        }

        public void Dispose()
        {
            if (_mpv != IntPtr.Zero)
            {
                MpvInterop.mpv_command_string(_mpv, "quit");
                MpvInterop.mpv_terminate_destroy(_mpv);
                _mpv = IntPtr.Zero;
            }
        }
    }
}