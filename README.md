Overview
MpcQtWpf is a modern WPF media player frontend built on the .NET 7 framework, powered by libmpv‑2.dll for high‑performance playback. Inspired by MPC‑QT, it combines a classic Windows UI with the flexibility of mpv, now enhanced with YouTube‑DL/yt‑dlp integration for streaming online content directly.

Key Features
🖼️ WPF Frontend
• 	Provides a familiar Windows desktop interface with playback controls (Open File, Play/Pause, Stop).
• 	Uses  to embed a WinForms panel inside WPF — mpv renders video directly into this surface.
• 	Status text area updates dynamically to reflect playback state.
🎥 libmpv Integration
• 	 bridges managed C# code with native libmpv‑2.dll.
• 	Supports core playback commands: , , , .
• 	Attaches mpv’s video output to a specific window handle for seamless rendering.
⚙️ MediaPlayerService.cs
• 	Encapsulates all mpv interop calls in a clean, reusable C# service.
• 	Handles initialization, window attachment, file loading, pausing, stopping, and disposal.
• 	Writes detailed logs to  for debugging and diagnostics.
🖱️ MainWindow.xaml.cs
• 	Connects the WPF UI to the media service.
• 	Open File launches a WPF  and passes the selected file path to mpv.
• 	Open URL now integrates with YouTube‑DL/yt‑dlp, resolving streaming links (e.g. YouTube) into playable streams before handing them to mpv.
• 	Playback controls (Play/Pause, Stop) update the status text to reflect current actions.

🖥️ End Result
When you run the application:
• 	A window opens with a clean, minimal playback interface.
• 	You can load local media files or stream online content via YouTube‑DL/yt‑dlp.
• 	mpv renders video/audio inside the embedded panel.
• 	Playback can be paused, resumed, or stopped with intuitive controls.

👉 In Short
MpcQtWpf is a lightweight yet extensible WPF wrapper around libmpv, built on .NET 7, with YouTube‑DL/yt‑dlp integration for streaming support. It provides the foundation for a full‑featured media player — ready to expand with seek bars, playlists, volume control, subtitle management, and more.

