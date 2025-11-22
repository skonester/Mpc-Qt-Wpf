using System;
using System.Runtime.InteropServices;

namespace MpcQtWpf.Interop
{
    public static class MpvInterop
    {
        // mpv_format enum for property types
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
            MPV_FORMAT_BYTE_ARRAY = 9
        }

        // Create a new mpv instance
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mpv_create();

        // Initialize mpv
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_initialize(IntPtr ctx);

        // Destroy mpv
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mpv_terminate_destroy(IntPtr ctx);

        // Set string option
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_option_string(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string value);

        // Set option with typed value
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_option(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            mpv_format format,
            ref long data);

        // Run a command string (e.g. "loadfile", "stop", "seek")
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_command_string(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string args);

        // Get property as double (e.g. "time-pos", "duration")
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_get_property(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            mpv_format format,
            ref double data);

        // Get property as int64
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_get_property(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            mpv_format format,
            ref long data);

        // Get property as flag (boolean)
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_get_property(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            mpv_format format,
            ref int data);

        // Get property as string
        [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_get_property(IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            mpv_format format,
            ref IntPtr data);
    }
}