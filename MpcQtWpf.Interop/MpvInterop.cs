using System;
using System.Runtime.InteropServices;

namespace MpcQtWpf.Interop
{
    public static class MpvInterop
    {
        private const string Dll = "libmpv-2.dll";

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mpv_create();

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_initialize(IntPtr handle);

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mpv_terminate_destroy(IntPtr handle);

        public enum mpv_format
        {
            MPV_FORMAT_NONE = 0,
            MPV_FORMAT_STRING = 1,
            MPV_FORMAT_OSD_STRING = 2,
            MPV_FORMAT_FLAG = 3,
            MPV_FORMAT_INT64 = 4,
            MPV_FORMAT_DOUBLE = 5,
            MPV_FORMAT_NODE = 6,
            MPV_FORMAT_NODE_ARRAY = 7,
            MPV_FORMAT_NODE_MAP = 8,
            MPV_FORMAT_BYTE_ARRAY = 9,
        }

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_option(IntPtr handle, string name, mpv_format format, ref long data);

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_option_string(IntPtr handle, string name, string value);

        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_command_string(IntPtr handle, string command);
    }
}
