## ☕ Support me on Ko-fi

[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20Development-ff5f5f?style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/r1kku)


# SpotiKnob

  

SpotiKnob is a small Windows desktop app for mapping knob-like controls or custom hotkeys to Spotify playback, Spotify volume, and system volume actions.

  

It is built with WinForms on .NET Framework 4.7.2 and includes:

  

- A custom main control window

- A settings window for app behavior

- A toast-style mode overlay

- Tray support for background use

- Configurable hotkeys for clockwise, counter-clockwise, press, and mode switching

  

## What It Does

  

SpotiKnob works around three control modes:

  

1.  `Spotify Mode`

Turns left/right input into Spotify previous/next track controls.

Press triggers Spotify play/pause.

  

2.  `Spotify Volume Mode`

Turns left/right input into Spotify app volume down/up.

Press triggers Spotify play/pause.

  

3.  `System Mode`

Turns left/right input into system volume down/up.

Press triggers global media play/pause.

  

You can cycle between these modes with the mode-cycle hotkey. When the mode changes, the app shows a small overlay notification and plays a mode sound.

  

## Main Features

  

- Global hotkey support

- Tray minimize / background operation

- Start minimized option

- Start with Windows option

- Optional restart as administrator

- Spotify window targeting for media commands

- Spotify per-app volume control when Spotify audio is active

- System master volume control

  

## Hotkeys

  

SpotiKnob stores four user-configurable hotkeys:

  

-  `Clockwise`

-  `Counter-clockwise`

-  `Press`

-  `Mode Cycle`

  

Hotkeys are stored in user settings and reloaded when the app starts.

  

## Settings

  

The settings window currently supports:

  

-  `Run as Admin Always`

-  `Start Minimized`

-  `Change Toggle key`

- GitHub profile link

- Website link

- Buy Me a Coffee link

- GitHub issues link

  

## Tray Behavior

  

SpotiKnob is meant to stay available in the background.

  

- Closing the main window hides it to the tray

- Minimizing can hide it to the tray

- Double-clicking the tray icon restores it

- The tray menu provides `Show`, `Hide`, `Start with Windows`, and `Exit`

  

## Spotify Notes

  

Spotify-related controls depend on the Spotify desktop app being available.

  

- Track controls are sent to the Spotify desktop window

- Spotify volume control works against Spotify’s audio session

- If Spotify is not available, SpotiKnob shows a tray balloon message

  

## Build

  

This project targets:

  

-  `.NET Framework 4.7.2`

-  `WinForms`

  

### Debug Build

  

```powershell

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe SpotiKnob.csproj /t:Build /p:Configuration=Debug

```

  

### Release Build

  

```powershell

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe SpotiKnob.csproj /t:Build /p:Configuration=Release

```

  

Release output is written to:

  

`bin\Release\`






  

## Notes

  

- This app is Windows-only

- Keep spotify open and not hidden on the tray else SpotiKnob will not work

- Administrator mode may be useful in setups where global input or elevated app interaction is needed



