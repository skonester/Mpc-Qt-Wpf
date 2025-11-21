requires libmpv-2.dll

🎬 MpcQtWpf — WPF Media Player with libmpv( inspired by MPC-QT)
What This Project Does
- WPF Frontend
- Provides a modern Windows UI with playback buttons (Open File, Play/Pause, Stop) and a status text area.
- Uses a WindowsFormsHost to embed a WinForms Panel inside WPF — this is where mpv renders video.
- libmpv Integration
- Through MpvInterop.cs, you call into the native libmpv-2.dll.
- This lets you control playback (loadfile, cycle pause, stop, quit) and attach mpv’s video output to a specific window handle.
- MediaPlayerService.cs
- Wraps all the mpv interop calls in a clean C# class.
- Handles initialization, attaching mpv to the panel, loading files, pausing, stopping, and disposing.
- Writes logs to mpv.log so you can debug playback issues.
- MainWindow.xaml.cs
- Wires up the UI to the service.
- When you click Open File, it launches a WPF OpenFileDialog, passes the chosen file path to mpv, and starts playback.
- Play/Pause toggles playback state, Stop halts playback.
- Status text updates to show what’s happening.

🖥️ End Result
When you run the app:
- A window opens with basic playback controls.
- You choose a media file (video or audio).
- mpv renders the media inside the black panel embedded in your WPF window.
- You can pause/resume or stop playback with the buttons.
It’s basically a minimal mpv‑based media player with a WPF UI, giving you the foundation to expand into a full‑featured player (seek bar, playlist, volume control, subtitles, etc.).

👉 In short: this project is a WPF wrapper around libmpv that plays media files inside a custom UI.


