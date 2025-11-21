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

        public void LoadFile(string path)
        {
            // Escape backslashes so mpv parses Windows paths correctly
            string escaped = path.Replace("\\", "\\\\");
            MpvInterop.mpv_command_string(_mpv, $"loadfile \"{escaped}\"");
        }

        public void TogglePause() => MpvInterop.mpv_command_string(_mpv, "cycle pause");
        public void Stop() => MpvInterop.mpv_command_string(_mpv, "stop");

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
