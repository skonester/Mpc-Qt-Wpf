# MpcQtWpf — WPF Media Player with libmpv (inspired by MPC‑QT)

## Overview
MpcQtWpf is a modern WPF media player frontend built on the .NET 7 framework, powered by **libmpv‑2.dll** for high‑performance playback. Inspired by MPC‑QT, it combines a classic Windows UI with the flexibility of mpv, now enhanced with **yt‑dlp integration** for streaming online content directly.

## Features

### WPF Frontend
- Provides a familiar Windows desktop interface with playback controls (Open File, Play/Pause, Stop).
- Uses `WindowsFormsHost` to embed a WinForms panel inside WPF, where mpv renders video.
- Status text area updates dynamically to reflect playback state.

### libmpv Integration
- `MpvInterop.cs` bridges managed C# code with native **libmpv‑2.dll**.
- Supports core playback commands: `loadfile`, `cycle pause`, `stop`, `quit`.
- Attaches mpv’s video output to a specific window handle for seamless rendering.

### MediaPlayerService.cs
- Encapsulates all mpv interop calls in a clean, reusable C# service.
- Handles initialization, window attachment, file loading, pausing, stopping, and disposal.
- Writes detailed logs to `mpv.log` for debugging and diagnostics.

### MainWindow.xaml.cs
- Connects the WPF UI to the media service.
- **Open File** launches a WPF `OpenFileDialog` and passes the selected file path to mpv.
- **Open URL** integrates with **yt‑dlp**, resolving streaming links (e.g. YouTube) into playable streams before handing them to mpv.
- Playback controls (Play/Pause, Stop) update the status text to reflect current actions.

## Requirements
- .NET 7 framework
- `libmpv-2.dll` (place in your `bin\Debug\net7.0-windows` folder during development)
- `yt-dlp.exe` (already included in the project)

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/MpcQtWpf.git
