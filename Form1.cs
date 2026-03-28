using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SpotiKnob
{
    public partial class Form1 : Form
    {
        private const int WM_HOTKEY = 0x0312;
        private const int WM_APPCOMMAND = 0x0319;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HWND_BROADCAST = 0xffff;
        private const int HTCAPTION = 0x0002;

        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private const int APPCOMMAND_VOLUME_DOWN = 9;
        private const int APPCOMMAND_VOLUME_UP = 10;
        private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        private const byte VK_MEDIA_PREV_TRACK = 0xB1;
        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const string StartupRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string StartupValueName = "SpotiKnob";
        private const string ToggleAudioAlias = "SpotiKnobToggleSound";
        private const string MusicModeAudioFile = "ToggleMusicMode.mp3";
        private const string SpotifyModeAudioFile = "SpotifyMode.mp3";
        private const string SystemModeAudioFile = "SystemVolumeMode.mp3";
        private const float VolumeStep = 0.05f;
        private readonly Dictionary<HotkeyAction, HotkeyBinding> hotkeyBindings = new Dictionary<HotkeyAction, HotkeyBinding>();
        private readonly Dictionary<HotkeyAction, Button> hotkeyButtons = new Dictionary<HotkeyAction, Button>();
        private readonly Dictionary<int, HotkeyAction> hotkeyIds = new Dictionary<int, HotkeyAction>();
        private readonly ToolTip bindingToolTip = new ToolTip();

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ModeOverlayForm modeOverlay;
        private SettingsForm settingsForm;
        private HotkeyAction? pendingBindingAction;
        private bool isRestoringFromTray;
        private bool isExiting;
        private bool minimizeOnStartup;
        private bool shouldAttemptSelfElevation;
        private int mode;

        public Form1()
        {
            InitializeComponent();
            InitializeHotkeyMappings();
            ConfigureForm();
            WireUi();
            SetupTrayIcon();
            LoadSavedBindings();
            LoadLaunchOptions();
            UpdateModeIndicator();
            RefreshHotkeyText();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegisterAllHotkeys();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnregisterAllHotkeys();
            base.OnHandleDestroyed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!isExiting && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideToTray();
                return;
            }

            CleanupResources();
            base.OnFormClosing(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                HotkeyAction action;
                if (hotkeyIds.TryGetValue(m.WParam.ToInt32(), out action))
                {
                    ExecuteHotkeyAction(action);
                }
            }

            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CsDropShadow = 0x00020000;
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CsDropShadow;
                return createParams;
            }
        }

        private void ConfigureForm()
        {
            MinimumSize = Size;
            Icon = GetApplicationIcon();

            AttachWindowDrag(this);
            AttachWindowDrag(mainCardPanel);
            AttachWindowDrag(titleLabel);
            AttachWindowDrag(modeHintLabel);
            AttachWindowDrag(creatorLabel);
            AttachWindowDrag(modePanel);
            AttachWindowDrag(modeLabel);
            AttachWindowDrag(modeDotPanel);
        }

        private void InitializeHotkeyMappings()
        {
            hotkeyIds[1000] = HotkeyAction.Clockwise;
            hotkeyIds[1001] = HotkeyAction.CounterClockwise;
            hotkeyIds[1002] = HotkeyAction.Press;
            hotkeyIds[1003] = HotkeyAction.ModeCycle;
        }

        private void WireUi()
        {
            hotkeyButtons[HotkeyAction.Clockwise] = bindClockwiseButton;
            hotkeyButtons[HotkeyAction.CounterClockwise] = bindCounterClockwiseButton;
            hotkeyButtons[HotkeyAction.Press] = bindPressButton;

            bindClockwiseButton.Click += delegate { BeginHotkeyCapture(HotkeyAction.Clockwise); };
            bindCounterClockwiseButton.Click += delegate { BeginHotkeyCapture(HotkeyAction.CounterClockwise); };
            bindPressButton.Click += delegate { BeginHotkeyCapture(HotkeyAction.Press); };
            settingsButton.Click += SettingsButton_Click;
            minimizeButton.Click += delegate { WindowState = FormWindowState.Minimized; };
            closeButton.Click += delegate { HideToTray(); };
        }

        private void SetupTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Show", null, delegate { ShowFromTray(); });
            trayMenu.Items.Add("Hide", null, delegate { HideToTray(); });
            ToolStripMenuItem startupMenuItem = new ToolStripMenuItem("Start with Windows");
            startupMenuItem.Name = "StartWithWindowsMenuItem";
            startupMenuItem.Checked = IsStartupEnabled();
            startupMenuItem.CheckOnClick = true;
            startupMenuItem.CheckedChanged += delegate
            {
                ApplyStartupSetting(startupMenuItem.Checked);
            };
            trayMenu.Items.Add(startupMenuItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Exit", null, delegate { ExitApplication(); });

            trayIcon = new NotifyIcon
            {
                ContextMenuStrip = trayMenu,
                Icon = GetApplicationIcon(),
                Text = "SpotiKnob",
                Visible = true
            };

            trayIcon.DoubleClick += delegate { ShowFromTray(); };
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotkeyCaptureKey(e);
        }

        private void HandleHotkeyCaptureKey(KeyEventArgs e)
        {
            if (!pendingBindingAction.HasValue)
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;

            if (e.KeyCode == Keys.Escape && e.Modifiers == Keys.None)
            {
                CancelHotkeyCapture("Binding cancelled.");
                return;
            }

            if (IsModifierKey(e.KeyCode))
            {
                return;
            }

            Keys modifiers = NormalizeModifiers(e.Modifiers);
            ApplyCapturedBinding(pendingBindingAction.Value, new HotkeyBinding(modifiers, e.KeyCode));
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            UpdateWindowRegion();

            if (shouldAttemptSelfElevation)
            {
                shouldAttemptSelfElevation = false;
                BeginInvoke(new Action(AttemptSelfElevation));
            }

            if (minimizeOnStartup)
            {
                BeginInvoke(new Action(HideToTray));
                minimizeOnStartup = false;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (!isRestoringFromTray && WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            UpdateWindowRegion();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (settingsForm == null || settingsForm.IsDisposed)
            {
                settingsForm = new SettingsForm();
                settingsForm.AdminChanged += SettingsForm_AdminChanged;
                settingsForm.StartMinimizedChanged += SettingsForm_StartMinimizedChanged;
                settingsForm.ToggleKeyRequested += SettingsForm_ToggleKeyRequested;
                settingsForm.CaptureKeyPressed += SettingsForm_CaptureKeyPressed;
                settingsForm.FormClosed += delegate { settingsForm = null; };
            }

            int centeredLeft = Left + Math.Max(0, (Width - settingsForm.Width) / 2);
            int centeredTop = Top + Math.Max(0, (Height - settingsForm.Height) / 2);
            settingsForm.Location = new Point(centeredLeft, centeredTop);
            settingsForm.ApplyState(
                Properties.Settings.Default.RunAsAdministrator,
                Properties.Settings.Default.StartMinimized);
            settingsForm.SetToggleBindingState(pendingBindingAction == HotkeyAction.ModeCycle);
            settingsForm.Show(this);
            settingsForm.BringToFront();
            settingsForm.Activate();
        }

        private void SettingsForm_AdminChanged(object sender, EventArgs e)
        {
            if (settingsForm != null)
            {
                ApplyAdministratorSetting(settingsForm.AdminEnabled);
            }
        }

        private void SettingsForm_StartMinimizedChanged(object sender, EventArgs e)
        {
            if (settingsForm != null)
            {
                ApplyStartMinimizedSetting(settingsForm.StartMinimizedEnabled);
            }
        }

        private void SettingsForm_ToggleKeyRequested(object sender, EventArgs e)
        {
            BeginHotkeyCapture(HotkeyAction.ModeCycle);
        }

        private void SettingsForm_CaptureKeyPressed(object sender, KeyEventArgs e)
        {
            HandleHotkeyCaptureKey(e);
        }

        private void BeginHotkeyCapture(HotkeyAction action)
        {
            UnregisterAllHotkeys();
            pendingBindingAction = action;
            UpdateBindingControls();

            Activate();
        }

        private void CancelHotkeyCapture(string statusMessage)
        {
            pendingBindingAction = null;
            UpdateBindingControls();

            string errorMessage;
            if (!TryRegisterAllHotkeys(out errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(this, errorMessage, "Hotkey Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ApplyCapturedBinding(HotkeyAction action, HotkeyBinding newBinding)
        {
            HotkeyBinding previousBinding = null;
            bool hadPreviousBinding = hotkeyBindings.TryGetValue(action, out previousBinding);

            hotkeyBindings[action] = newBinding;

            string validationMessage;
            if (!TryRegisterAllHotkeys(out validationMessage))
            {
                string failedRegistrationMessage = validationMessage;

                if (hadPreviousBinding)
                {
                    hotkeyBindings[action] = previousBinding;
                }
                else
                {
                    hotkeyBindings.Remove(action);
                }

                string restoreErrorMessage;
                TryRegisterAllHotkeys(out restoreErrorMessage);
                RefreshHotkeyText();
                CancelHotkeyCapture("Unable to apply that hotkey.");
                MessageBox.Show(this, failedRegistrationMessage, "Hotkey Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefreshHotkeyText();
            SaveBindings();
            CancelHotkeyCapture(string.Format("{0} is now bound to {1}.", GetActionDisplayName(action), newBinding));
        }

        private void RefreshHotkeyText()
        {
            toggleKeyCaptionLabel.Text = "Toggle key  " + GetBindingText(HotkeyAction.ModeCycle, "-");
            bindingToolTip.SetToolTip(bindClockwiseButton, "Clockwise: " + GetBindingText(HotkeyAction.Clockwise, "Not bound"));
            bindingToolTip.SetToolTip(bindPressButton, "Press: " + GetBindingText(HotkeyAction.Press, "Not bound"));
            bindingToolTip.SetToolTip(bindCounterClockwiseButton, "Counter-clockwise: " + GetBindingText(HotkeyAction.CounterClockwise, "Not bound"));
            if (settingsForm != null && !settingsForm.IsDisposed)
            {
                settingsForm.SetToggleBindingState(pendingBindingAction == HotkeyAction.ModeCycle);
            }
            UpdateBindingControls();
        }

        private void UpdateBindingControls()
        {
            bindClockwiseButton.IsListening = pendingBindingAction == HotkeyAction.Clockwise;
            bindPressButton.IsListening = pendingBindingAction == HotkeyAction.Press;
            bindCounterClockwiseButton.IsListening = pendingBindingAction == HotkeyAction.CounterClockwise;

            bindClockwiseButton.Enabled = !pendingBindingAction.HasValue || pendingBindingAction == HotkeyAction.Clockwise;
            bindPressButton.Enabled = !pendingBindingAction.HasValue || pendingBindingAction == HotkeyAction.Press;
            bindCounterClockwiseButton.Enabled = !pendingBindingAction.HasValue || pendingBindingAction == HotkeyAction.CounterClockwise;
            if (settingsForm != null && !settingsForm.IsDisposed)
            {
                settingsForm.SetToggleBindingState(pendingBindingAction == HotkeyAction.ModeCycle);
            }
        }

        private string GetBindingText(HotkeyAction action, string fallbackText)
        {
            HotkeyBinding binding;
            return hotkeyBindings.TryGetValue(action, out binding) ? binding.ToString() : fallbackText;
        }

        private void LoadSavedBindings()
        {
            TryLoadBinding(HotkeyAction.Clockwise, Properties.Settings.Default.ClockwiseHotkey);
            TryLoadBinding(HotkeyAction.CounterClockwise, Properties.Settings.Default.CounterClockwiseHotkey);
            TryLoadBinding(HotkeyAction.Press, Properties.Settings.Default.PressHotkey);
            TryLoadBinding(HotkeyAction.ModeCycle, Properties.Settings.Default.ModeCycleHotkey);
        }

        private void SaveBindings()
        {
            Properties.Settings.Default.ClockwiseHotkey = SerializeBinding(HotkeyAction.Clockwise);
            Properties.Settings.Default.CounterClockwiseHotkey = SerializeBinding(HotkeyAction.CounterClockwise);
            Properties.Settings.Default.PressHotkey = SerializeBinding(HotkeyAction.Press);
            Properties.Settings.Default.ModeCycleHotkey = SerializeBinding(HotkeyAction.ModeCycle);
            Properties.Settings.Default.Save();
        }

        private void TryLoadBinding(HotkeyAction action, string rawValue)
        {
            HotkeyBinding binding;
            if (TryParseBinding(rawValue, out binding))
            {
                hotkeyBindings[action] = binding;
            }
        }

        private string SerializeBinding(HotkeyAction action)
        {
            HotkeyBinding binding;
            if (!hotkeyBindings.TryGetValue(action, out binding))
            {
                return string.Empty;
            }

            return string.Format("{0}|{1}", (int)binding.Modifiers, (int)binding.Key);
        }

        private static bool TryParseBinding(string rawValue, out HotkeyBinding binding)
        {
            binding = null;
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            string[] parts = rawValue.Split('|');
            if (parts.Length != 2)
            {
                return false;
            }

            int modifierValue;
            int keyValue;
            if (!int.TryParse(parts[0], out modifierValue) || !int.TryParse(parts[1], out keyValue))
            {
                return false;
            }

            binding = new HotkeyBinding((Keys)modifierValue, (Keys)keyValue);
            return true;
        }

        private void LoadLaunchOptions()
        {
            UpdateTrayStartupState();

            minimizeOnStartup = Properties.Settings.Default.StartMinimized;

            if (Properties.Settings.Default.RunAsAdministrator && !IsRunningAsAdministrator())
            {
                shouldAttemptSelfElevation = true;
            }
        }

        private void ApplyStartupSetting(bool enabled)
        {
            bool success = SetStartupEnabled(enabled);
            if (!success)
            {
                SyncSettingsFormState();
                MessageBox.Show(this, "Windows startup registration could not be updated.", "Startup Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Properties.Settings.Default.StartWithWindows = enabled;
            Properties.Settings.Default.Save();
            UpdateTrayStartupState();
            SyncSettingsFormState();
        }

        private void ApplyAdministratorSetting(bool enabled)
        {
            if (enabled)
            {
                Properties.Settings.Default.RunAsAdministrator = true;
                Properties.Settings.Default.Save();

                if (!IsRunningAsAdministrator())
                {
                    AttemptSelfElevation();
                }
            }
            else
            {
                Properties.Settings.Default.RunAsAdministrator = false;
                Properties.Settings.Default.Save();
            }

            SyncSettingsFormState();
        }

        private void ApplyStartMinimizedSetting(bool enabled)
        {
            Properties.Settings.Default.StartMinimized = enabled;
            Properties.Settings.Default.Save();
            SyncSettingsFormState();
        }

        private void AttemptSelfElevation()
        {
            if (IsRunningAsAdministrator())
            {
                return;
            }

            DialogResult result = ShowTopMostMessageBox(
                "SpotiKnob will restart with administrator privileges. Windows may show a UAC prompt.",
                "Restart as Administrator",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (result != DialogResult.OK)
            {
                Properties.Settings.Default.RunAsAdministrator = false;
                Properties.Settings.Default.Save();
                SyncSettingsFormState();
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath)
                };

                Process.Start(startInfo);
                isExiting = true;
                Close();
            }
            catch
            {
                Properties.Settings.Default.RunAsAdministrator = false;
                Properties.Settings.Default.Save();
                SyncSettingsFormState();
                ShowTopMostMessageBox(
                    "Administrator restart was cancelled or failed.",
                    "Elevation Cancelled",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private DialogResult ShowTopMostMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            using (Form topMostOwner = new Form())
            {
                topMostOwner.StartPosition = FormStartPosition.Manual;
                topMostOwner.Location = new Point(-32000, -32000);
                topMostOwner.Size = new Size(1, 1);
                topMostOwner.ShowInTaskbar = false;
                topMostOwner.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                topMostOwner.TopMost = true;
                topMostOwner.Opacity = 0;

                topMostOwner.Show();
                topMostOwner.BringToFront();

                return MessageBox.Show(topMostOwner, text, caption, buttons, icon);
            }
        }

        private static bool IsRunningAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static bool IsStartupEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryPath, false))
            {
                string value = key == null ? null : key.GetValue(StartupValueName) as string;
                return string.Equals(value, GetStartupCommand(), StringComparison.OrdinalIgnoreCase);
            }
        }

        private static bool SetStartupEnabled(bool enabled)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(StartupRegistryPath))
                {
                    if (key == null)
                    {
                        return false;
                    }

                    if (enabled)
                    {
                        key.SetValue(StartupValueName, GetStartupCommand());
                    }
                    else
                    {
                        key.DeleteValue(StartupValueName, false);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetStartupCommand()
        {
            return "\"" + Application.ExecutablePath + "\"";
        }

        private void UpdateTrayStartupState()
        {
            if (trayMenu == null)
            {
                return;
            }

            ToolStripMenuItem startupMenuItem = trayMenu.Items["StartWithWindowsMenuItem"] as ToolStripMenuItem;
            if (startupMenuItem != null)
            {
                startupMenuItem.Checked = IsStartupEnabled();
            }
        }

        private void SyncSettingsFormState()
        {
            if (settingsForm != null && !settingsForm.IsDisposed)
            {
                settingsForm.ApplyState(
                    Properties.Settings.Default.RunAsAdministrator,
                    Properties.Settings.Default.StartMinimized);
            }
        }

        private void UpdateWindowRegion()
        {
            int cornerRadius = 18;
            using (GraphicsPath path = CreateRoundedPath(new Rectangle(Point.Empty, Size), cornerRadius))
            {
                Region = new Region(path);
            }
        }

        private void AttachWindowDrag(Control control)
        {
            control.MouseDown += DragWindow_MouseDown;
        }

        private void DragWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            NativeMethods.ReleaseCapture();
            NativeMethods.SendMessage(Handle, WM_NCLBUTTONDOWN, new IntPtr(HTCAPTION), IntPtr.Zero);
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void UpdateModeIndicator()
        {
            ModePresentation presentation = GetCurrentModePresentation();
            modeLabel.Text = presentation.Title;
            modePanel.BackColor = presentation.AccentBackgroundColor;
            modePanel.BorderColor = presentation.AccentBackgroundColor;
            modeLabel.ForeColor = presentation.AccentTextColor;
            modeDotPanel.BackColor = presentation.AccentTextColor;
            modeHintLabel.Text = presentation.Description;
        }

        private void CycleMode()
        {
            mode = (mode + 1) % 3;
            UpdateModeIndicator();
            ShowModeOverlay();
            PlayModeToggleSound();
        }

        private void ExecuteHotkeyAction(HotkeyAction action)
        {
            if (action == HotkeyAction.ModeCycle)
            {
                CycleMode();
                return;
            }

            if (action == HotkeyAction.Press)
            {
                if (mode == 2)
                {
                    SendGlobalMediaCommand(APPCOMMAND_MEDIA_PLAY_PAUSE);
                }
                else
                {
                    SendSpotifyCommand(APPCOMMAND_MEDIA_PLAY_PAUSE);
                }
                return;
            }

            switch (mode)
            {
                case 0:
                    if (action == HotkeyAction.Clockwise)
                    {
                        SendSpotifyCommand(APPCOMMAND_MEDIA_NEXTTRACK);
                    }
                    else if (action == HotkeyAction.CounterClockwise)
                    {
                        SendSpotifyCommand(APPCOMMAND_MEDIA_PREVIOUSTRACK);
                    }
                    break;
                case 1:
                    if (action == HotkeyAction.Clockwise)
                    {
                        AdjustSpotifyVolume(true);
                    }
                    else if (action == HotkeyAction.CounterClockwise)
                    {
                        AdjustSpotifyVolume(false);
                    }
                    break;
                case 2:
                    if (action == HotkeyAction.Clockwise)
                    {
                        AdjustSystemVolume(true);
                    }
                    else if (action == HotkeyAction.CounterClockwise)
                    {
                        AdjustSystemVolume(false);
                    }
                    break;
            }
        }

        private void SendSpotifyCommand(int appCommand)
        {
            IntPtr spotifyWindow = FindSpotifyWindow();
            if (spotifyWindow == IntPtr.Zero)
            {
                ShowAppNotification("Spotify Not Found", "Open the Spotify desktop app to receive media commands.", Color.FromArgb(180, 83, 9));
                return;
            }

            SendAppCommand(spotifyWindow, appCommand);
        }

        private void SendAppCommand(IntPtr targetWindow, int appCommand)
        {
            IntPtr command = new IntPtr(appCommand << 16);
            NativeMethods.SendMessage(targetWindow, WM_APPCOMMAND, targetWindow, command);
        }

        private void SendGlobalMediaCommand(int appCommand)
        {
            byte mediaVirtualKey;
            switch (appCommand)
            {
                case APPCOMMAND_MEDIA_NEXTTRACK:
                    mediaVirtualKey = VK_MEDIA_NEXT_TRACK;
                    break;
                case APPCOMMAND_MEDIA_PREVIOUSTRACK:
                    mediaVirtualKey = VK_MEDIA_PREV_TRACK;
                    break;
                case APPCOMMAND_MEDIA_PLAY_PAUSE:
                    mediaVirtualKey = VK_MEDIA_PLAY_PAUSE;
                    break;
                default:
                    IntPtr broadcastHandle = new IntPtr(HWND_BROADCAST);
                    SendAppCommand(broadcastHandle, appCommand);
                    return;
            }

            NativeMethods.keybd_event(mediaVirtualKey, 0, 0, UIntPtr.Zero);
            NativeMethods.keybd_event(mediaVirtualKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private void AdjustSpotifyVolume(bool increaseVolume)
        {
            ISimpleAudioVolume spotifyVolume = null;

            try
            {
                spotifyVolume = GetSpotifyAudioVolume();
                if (spotifyVolume == null)
                {
                    ShowAppNotification("Spotify Not Found", "Start audio playback in Spotify to adjust its app volume.", Color.FromArgb(180, 83, 9));
                    return;
                }

                float currentLevel;
                spotifyVolume.GetMasterVolume(out currentLevel);
                spotifyVolume.SetMasterVolume(ClampVolume(currentLevel + (increaseVolume ? VolumeStep : -VolumeStep)), Guid.Empty);
            }
            finally
            {
                ReleaseComObject(spotifyVolume);
            }
        }

        private void AdjustSystemVolume(bool increaseVolume)
        {
            IAudioEndpointVolume endpointVolume = null;

            try
            {
                endpointVolume = GetDefaultAudioEndpointVolume();
                if (endpointVolume == null)
                {
                    return;
                }

                float currentLevel;
                endpointVolume.GetMasterVolumeLevelScalar(out currentLevel);
                endpointVolume.SetMasterVolumeLevelScalar(ClampVolume(currentLevel + (increaseVolume ? VolumeStep : -VolumeStep)), Guid.Empty);
            }
            finally
            {
                ReleaseComObject(endpointVolume);
            }
        }

        private IntPtr FindSpotifyWindow()
        {
            Process[] spotifyProcesses = Process.GetProcessesByName("Spotify");
            if (spotifyProcesses.Length == 0)
            {
                return IntPtr.Zero;
            }

            HashSet<int> processIds = new HashSet<int>();
            foreach (Process process in spotifyProcesses)
            {
                processIds.Add(process.Id);
            }

            IntPtr foundHandle = IntPtr.Zero;
            NativeMethods.EnumWindows(
                delegate (IntPtr windowHandle, IntPtr lParam)
                {
                    if (!NativeMethods.IsWindowVisible(windowHandle))
                    {
                        return true;
                    }

                    uint windowProcessId;
                    NativeMethods.GetWindowThreadProcessId(windowHandle, out windowProcessId);
                    if (!processIds.Contains((int)windowProcessId))
                    {
                        return true;
                    }

                    StringBuilder className = new StringBuilder(256);
                    NativeMethods.GetClassName(windowHandle, className, className.Capacity);
                    if (className.ToString().IndexOf("Chrome_WidgetWin", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundHandle = windowHandle;
                        return false;
                    }

                    return true;
                },
                IntPtr.Zero);

            return foundHandle;
        }

        private bool TryRegisterAllHotkeys(out string errorMessage)
        {
            errorMessage = null;
            UnregisterAllHotkeys();

            HashSet<string> usedBindings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<int, HotkeyAction> pair in hotkeyIds)
            {
                HotkeyBinding binding;
                if (!hotkeyBindings.TryGetValue(pair.Value, out binding))
                {
                    continue;
                }

                if (!usedBindings.Add(binding.ToString()))
                {
                    errorMessage = string.Format("The hotkey {0} is assigned more than once. Each action needs a unique shortcut.", binding);
                    return false;
                }

                if (!NativeMethods.RegisterHotKey(Handle, pair.Key, (uint)binding.Modifiers, (uint)binding.Key))
                {
                    int win32Error = Marshal.GetLastWin32Error();
                    string errorDetail = GetHotkeyRegistrationErrorDetail(win32Error);
                    errorMessage = string.Format(
                        "Windows could not register {0} for {1}.{2}",
                        binding,
                        GetActionDisplayName(pair.Value),
                        errorDetail);
                    UnregisterAllHotkeys();
                    return false;
                }
            }

            return true;
        }

        private static string GetHotkeyRegistrationErrorDetail(int win32Error)
        {
            if (win32Error == 0)
            {
                return " The shortcut may already be in use by another app, blocked by Windows, or reserved as a system hotkey.";
            }

            string message = new Win32Exception(win32Error).Message;
            if (string.IsNullOrWhiteSpace(message))
            {
                return string.Format(" Windows error code: {0}.", win32Error);
            }

            return string.Format(" Windows error {0}: {1}.", win32Error, message);
        }

        private void RegisterAllHotkeys()
        {
            string errorMessage;
            if (!TryRegisterAllHotkeys(out errorMessage) && !string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(this, errorMessage, "Hotkey Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UnregisterAllHotkeys()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            foreach (int id in hotkeyIds.Keys)
            {
                NativeMethods.UnregisterHotKey(Handle, id);
            }
        }

        private void ShowFromTray()
        {
            isRestoringFromTray = true;

            try
            {
                SuspendLayout();
                ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;
                Show();
                Visible = true;
                BringToFront();
                Activate();
                PerformLayout();
                Refresh();
            }
            finally
            {
                ResumeLayout(true);
                isRestoringFromTray = false;
            }
        }

        private void HideToTray()
        {
            ShowInTaskbar = false;
            Hide();
        }

        private void ExitApplication()
        {
            isExiting = true;
            Close();
        }

        private void CleanupResources()
        {
            UnregisterAllHotkeys();
            CloseToggleSound();

            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }

            if (trayMenu != null)
            {
                trayMenu.Dispose();
                trayMenu = null;
            }

            if (modeOverlay != null)
            {
                modeOverlay.Dispose();
                modeOverlay = null;
            }

            if (settingsForm != null)
            {
                settingsForm.Dispose();
                settingsForm = null;
            }
        }

        private static Icon GetApplicationIcon()
        {
            try
            {
                Icon applicationIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (applicationIcon != null)
                {
                    return applicationIcon;
                }
            }
            catch
            {
            }

            return SystemIcons.Application;
        }

        private void ShowModeOverlay()
        {
            if (modeOverlay == null || modeOverlay.IsDisposed)
            {
                modeOverlay = new ModeOverlayForm();
            }

            ModePresentation presentation = GetCurrentModePresentation();
            modeOverlay.ShowMode(
                presentation.Title,
                presentation.ToastDescription,
                presentation.AccentBackgroundColor,
                presentation.Glyph);
        }

        private void ShowAppNotification(string title, string description, Color accentColor)
        {
            if (modeOverlay == null || modeOverlay.IsDisposed)
            {
                modeOverlay = new ModeOverlayForm();
            }

            modeOverlay.ShowNotification(title, description, accentColor);
        }

        private ModePresentation GetCurrentModePresentation()
        {
            switch (mode)
            {
                case 0:
                    return new ModePresentation(
                        "Spotify Mode",
                        "Spotify mode - Controls Next and Previous, can play/pause by pressing knob",
                        "Spotify Mode",
                        AppTheme.SpotifyAccentBackground,
                        AppTheme.SpotifyAccent,
                        ">");
                case 1:
                    return new ModePresentation(
                        "Spotify Volume Mode",
                        "Spotify Volume Mode - Controls volume and can play/pause",
                        "Spotify Volume Mode",
                        Color.FromArgb(30, 64, 175),
                        Color.FromArgb(191, 219, 254),
                        ">");
                default:
                    return new ModePresentation(
                        "System Mode",
                        "System Mode - Controls system volume and can play or pause media",
                        "System Mode",
                        Color.FromArgb(124, 45, 18),
                        Color.FromArgb(253, 186, 116),
                        ">");
            }
        }

        private static string GetActionDisplayName(HotkeyAction action)
        {
            switch (action)
            {
                case HotkeyAction.Clockwise:
                    return "Clockwise";
                case HotkeyAction.CounterClockwise:
                    return "Counter-clockwise";
                case HotkeyAction.Press:
                    return "Press (Play/Pause)";
                default:
                    return "Mode Cycle";
            }
        }

        private static bool IsModifierKey(Keys key)
        {
            return key == Keys.ControlKey
                || key == Keys.Menu
                || key == Keys.ShiftKey
                || key == Keys.LWin
                || key == Keys.RWin;
        }

        private static Keys NormalizeModifiers(Keys modifiers)
        {
            Keys normalized = Keys.None;

            if ((modifiers & Keys.Control) == Keys.Control)
            {
                normalized |= Keys.Control;
            }

            if ((modifiers & Keys.Alt) == Keys.Alt)
            {
                normalized |= Keys.Alt;
            }

            if ((modifiers & Keys.Shift) == Keys.Shift)
            {
                normalized |= Keys.Shift;
            }

            return normalized;
        }

        private static float ClampVolume(float level)
        {
            if (level < 0f)
            {
                return 0f;
            }

            if (level > 1f)
            {
                return 1f;
            }

            return level;
        }

        private void PlayModeToggleSound()
        {
            string audioFileName = GetModeAudioFileName();
            if (string.IsNullOrEmpty(audioFileName))
            {
                return;
            }

            string audioPath = AssetStore.ExtractTempFile("audio\\" + audioFileName);
            if (string.IsNullOrEmpty(audioPath) || !File.Exists(audioPath))
            {
                return;
            }

            NativeMethods.mciSendString("close " + ToggleAudioAlias, null, 0, IntPtr.Zero);
            NativeMethods.mciSendString(string.Format("open \"{0}\" type mpegvideo alias {1}", audioPath, ToggleAudioAlias), null, 0, IntPtr.Zero);
            NativeMethods.mciSendString("play " + ToggleAudioAlias + " from 0", null, 0, IntPtr.Zero);
        }

        private string GetModeAudioFileName()
        {
            switch (mode)
            {
                case 0:
                    return MusicModeAudioFile;
                case 1:
                    return SpotifyModeAudioFile;
                case 2:
                    return SystemModeAudioFile;
                default:
                    return null;
            }
        }

        private void CloseToggleSound()
        {
            NativeMethods.mciSendString("close " + ToggleAudioAlias, null, 0, IntPtr.Zero);
        }

        private static IAudioEndpointVolume GetDefaultAudioEndpointVolume()
        {
            IMMDeviceEnumerator deviceEnumerator = null;
            IMMDevice device = null;
            object endpointObject = null;

            try
            {
                deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out device);

                Guid endpointGuid = typeof(IAudioEndpointVolume).GUID;
                device.Activate(ref endpointGuid, CLSCTX.ALL, IntPtr.Zero, out endpointObject);
                return endpointObject as IAudioEndpointVolume;
            }
            finally
            {
                ReleaseComObject(device);
                ReleaseComObject(deviceEnumerator);
            }
        }

        private static ISimpleAudioVolume GetSpotifyAudioVolume()
        {
            Process[] spotifyProcesses = Process.GetProcessesByName("Spotify");
            if (spotifyProcesses.Length == 0)
            {
                return null;
            }

            HashSet<int> processIds = new HashSet<int>();
            foreach (Process process in spotifyProcesses)
            {
                processIds.Add(process.Id);
            }

            IMMDeviceEnumerator deviceEnumerator = null;
            IMMDevice device = null;
            object sessionManagerObject = null;
            IAudioSessionManager2 sessionManager = null;
            IAudioSessionEnumerator sessionEnumerator = null;

            try
            {
                deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumeratorComObject();
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out device);

                Guid sessionManagerGuid = typeof(IAudioSessionManager2).GUID;
                device.Activate(ref sessionManagerGuid, CLSCTX.ALL, IntPtr.Zero, out sessionManagerObject);
                sessionManager = sessionManagerObject as IAudioSessionManager2;
                if (sessionManager == null)
                {
                    return null;
                }

                sessionManager.GetSessionEnumerator(out sessionEnumerator);

                int count;
                sessionEnumerator.GetCount(out count);
                for (int index = 0; index < count; index++)
                {
                    IAudioSessionControl sessionControl = null;
                    IAudioSessionControl2 sessionControl2 = null;
                    ISimpleAudioVolume sessionVolume = null;
                    bool keepSessionInterfaces = false;

                    try
                    {
                        sessionEnumerator.GetSession(index, out sessionControl);
                        sessionControl2 = sessionControl as IAudioSessionControl2;
                        if (sessionControl2 == null)
                        {
                            continue;
                        }

                        uint processId;
                        sessionControl2.GetProcessId(out processId);
                        if (!processIds.Contains((int)processId))
                        {
                            continue;
                        }

                        sessionVolume = sessionControl as ISimpleAudioVolume;
                        if (sessionVolume != null)
                        {
                            keepSessionInterfaces = true;
                            return sessionVolume;
                        }
                    }
                    finally
                    {
                        if (!keepSessionInterfaces)
                        {
                            ReleaseComObject(sessionVolume);
                            ReleaseComObject(sessionControl2);
                            ReleaseComObject(sessionControl);
                        }
                    }
                }

                return null;
            }
            finally
            {
                ReleaseComObject(sessionEnumerator);
                ReleaseComObject(sessionManager);
                ReleaseComObject(device);
                ReleaseComObject(deviceEnumerator);
            }
        }

        private static void ReleaseComObject(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                Marshal.ReleaseComObject(comObject);
            }
        }

        private enum HotkeyAction
        {
            Clockwise = 0,
            CounterClockwise = 1,
            Press = 2,
            ModeCycle = 3
        }

        private sealed class HotkeyBinding
        {
            public HotkeyBinding(Keys modifiers, Keys key)
            {
                Modifiers = modifiers;
                Key = key;
            }

            public Keys Modifiers { get; private set; }

            public Keys Key { get; private set; }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

                if ((Modifiers & Keys.Control) == Keys.Control)
                {
                    builder.Append("Ctrl+");
                }

                if ((Modifiers & Keys.Alt) == Keys.Alt)
                {
                    builder.Append("Alt+");
                }

                if ((Modifiers & Keys.Shift) == Keys.Shift)
                {
                    builder.Append("Shift+");
                }

                builder.Append(Key);
                return builder.ToString();
            }
        }

        private sealed class ModePresentation
        {
            public ModePresentation(string title, string description, string toastDescription, Color accentBackgroundColor, Color accentTextColor, string glyph)
            {
                Title = title;
                Description = description;
                ToastDescription = toastDescription;
                AccentBackgroundColor = accentBackgroundColor;
                AccentTextColor = accentTextColor;
                Glyph = glyph;
            }

            public string Title { get; private set; }
            public string Description { get; private set; }
            public string ToastDescription { get; private set; }
            public Color AccentBackgroundColor { get; private set; }
            public Color AccentTextColor { get; private set; }
            public string Glyph { get; private set; }
        }

        private static class NativeMethods
        {
            public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("winmm.dll", CharSet = CharSet.Auto)]
            public static extern int mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);
        }

        private enum EDataFlow
        {
            eRender,
            eCapture,
            eAll
        }

        private enum ERole
        {
            eConsole,
            eMultimedia,
            eCommunications
        }

        [Flags]
        private enum CLSCTX : uint
        {
            INPROC_SERVER = 0x1,
            INPROC_HANDLER = 0x2,
            LOCAL_SERVER = 0x4,
            INPROC_SERVER16 = 0x8,
            REMOTE_SERVER = 0x10,
            INPROC_HANDLER16 = 0x20,
            RESERVED1 = 0x40,
            RESERVED2 = 0x80,
            RESERVED3 = 0x100,
            RESERVED4 = 0x200,
            NO_CODE_DOWNLOAD = 0x400,
            RESERVED5 = 0x800,
            NO_CUSTOM_MARSHAL = 0x1000,
            ENABLE_CODE_DOWNLOAD = 0x2000,
            NO_FAILURE_LOG = 0x4000,
            DISABLE_AAA = 0x8000,
            ENABLE_AAA = 0x10000,
            FROM_DEFAULT_CONTEXT = 0x20000,
            ACTIVATE_X86_SERVER = 0x40000,
            ACTIVATE_32_BIT_SERVER = ACTIVATE_X86_SERVER,
            ACTIVATE_64_BIT_SERVER = 0x80000,
            ENABLE_CLOAKING = 0x100000,
            APPCONTAINER = 0x400000,
            ACTIVATE_AAA_AS_IU = 0x800000,
            PS_DLL = 0x80000000,
            ALL = INPROC_SERVER | INPROC_HANDLER | LOCAL_SERVER | REMOTE_SERVER
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumeratorComObject
        {
        }

        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            int NotImpl1();
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice endpoint);
        }

        [ComImport]
        [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            int Activate(ref Guid iid, CLSCTX clsContext, IntPtr activationParams, [MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);
        }

        [ComImport]
        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioEndpointVolume
        {
            int RegisterControlChangeNotify(IntPtr notify);
            int UnregisterControlChangeNotify(IntPtr notify);
            int GetChannelCount(out uint channelCount);
            int SetMasterVolumeLevel(float levelDb, Guid eventContext);
            int SetMasterVolumeLevelScalar(float level, Guid eventContext);
            int GetMasterVolumeLevel(out float levelDb);
            int GetMasterVolumeLevelScalar(out float level);
            int SetChannelVolumeLevel(uint channelNumber, float levelDb, Guid eventContext);
            int SetChannelVolumeLevelScalar(uint channelNumber, float level, Guid eventContext);
            int GetChannelVolumeLevel(uint channelNumber, out float levelDb);
            int GetChannelVolumeLevelScalar(uint channelNumber, out float level);
            int SetMute([MarshalAs(UnmanagedType.Bool)] bool isMuted, Guid eventContext);
            int GetMute(out bool isMuted);
            int GetVolumeStepInfo(out uint step, out uint stepCount);
            int VolumeStepUp(Guid eventContext);
            int VolumeStepDown(Guid eventContext);
            int QueryHardwareSupport(out uint hardwareSupportMask);
            int GetVolumeRange(out float volumeMindB, out float volumeMaxdB, out float volumeIncrementdB);
        }

        [ComImport]
        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionManager2
        {
            int NotImpl0();
            int NotImpl1();
            int GetSessionEnumerator(out IAudioSessionEnumerator sessionEnum);
        }

        [ComImport]
        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionEnumerator
        {
            int GetCount(out int sessionCount);
            int GetSession(int sessionIndex, out IAudioSessionControl session);
        }

        [ComImport]
        [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionControl
        {
            int GetState(out int state);
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string displayName);
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, Guid eventContext);
            int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string iconPath);
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string value, Guid eventContext);
            int GetGroupingParam(out Guid groupingId);
            int SetGroupingParam(Guid groupingId, Guid eventContext);
            int NotImpl0();
            int NotImpl1();
        }

        [ComImport]
        [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionControl2
        {
            int GetState(out int state);
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string displayName);
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, Guid eventContext);
            int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string iconPath);
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string value, Guid eventContext);
            int GetGroupingParam(out Guid groupingId);
            int SetGroupingParam(Guid groupingId, Guid eventContext);
            int NotImpl0();
            int NotImpl1();
            int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string sessionIdentifier);
            int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string sessionInstanceIdentifier);
            int GetProcessId(out uint processId);
            int IsSystemSoundsSession();
            int SetDuckingPreference(bool optOut);
        }

        [ComImport]
        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISimpleAudioVolume
        {
            int SetMasterVolume(float level, Guid eventContext);
            int GetMasterVolume(out float level);
            int SetMute(bool isMuted, Guid eventContext);
            int GetMute(out bool isMuted);
        }
    }
}
