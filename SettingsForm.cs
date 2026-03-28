using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpotiKnob
{
    public partial class SettingsForm : Form
    {
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x0002;
        private bool suppressEvents;

        public SettingsForm()
        {
            InitializeComponent();
            ApplyVisualAssets();

            adminToggle.CheckedChanged += delegate { if (!suppressEvents) AdminChanged(this, EventArgs.Empty); };
            startMinimizedToggle.CheckedChanged += delegate { if (!suppressEvents) StartMinimizedChanged(this, EventArgs.Empty); };
            bindModeCycleButton.Click += delegate { ToggleKeyRequested(this, EventArgs.Empty); };
            closeButton.Click += delegate { Close(); };

            githubIconButton.Click += delegate { OpenExternal("https://github.com/R1kkku"); };
            githubLabel.Click += delegate { OpenExternal("https://github.com/R1kkku"); };
            websiteIconButton.Click += delegate { OpenExternal("https://coderikku.cloud"); };
            websiteLabel.Click += delegate { OpenExternal("https://coderikku.cloud"); };
            coffeeButton.Click += delegate { OpenExternal("https://ko-fi.com/r1kku"); };
            issueLinkLabel.LinkClicked += delegate { OpenExternal("https://github.com/R1kkku/SpotiKnob/issues"); };

            AttachWindowDrag(this);
            AttachWindowDrag(cardPanel);
            AttachWindowDrag(titleLabel);
            ApplyOptionStyles();
        }

        public event EventHandler AdminChanged = delegate { };
        public event EventHandler StartMinimizedChanged = delegate { };
        public event EventHandler ToggleKeyRequested = delegate { };
        public event KeyEventHandler CaptureKeyPressed = delegate { };

        public bool AdminEnabled
        {
            get { return adminToggle.Checked; }
            set { adminToggle.Checked = value; }
        }

        public bool StartMinimizedEnabled
        {
            get { return startMinimizedToggle.Checked; }
            set { startMinimizedToggle.Checked = value; }
        }

        public void ApplyState(bool adminEnabled, bool startMinimizedEnabled)
        {
            suppressEvents = true;
            adminToggle.Checked = adminEnabled;
            startMinimizedToggle.Checked = startMinimizedEnabled;
            suppressEvents = false;
            ApplyOptionStyles();
        }

        public void SetToggleBindingState(bool listening)
        {
            bindModeCycleButton.Text = listening ? "Listening..." : "Change Toggle key";
            bindModeCycleButton.IsActive = listening;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateWindowRegion();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateWindowRegion();
        }

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            CaptureKeyPressed(this, e);
        }

        private void ApplyOptionStyles()
        {
            adminToggle.IsActive = adminToggle.Checked;
            startMinimizedToggle.IsActive = startMinimizedToggle.Checked;
        }

        private void ApplyVisualAssets()
        {
            adminUacIcon.Image = LoadBitmap("uac.png");
            githubIconButton.DisplayImage = LoadBitmap("github-social.png");
            websiteIconButton.DisplayImage = LoadBitmap("world-social.png");
            coffeeButton.DisplayImage = LoadBitmap("bmc-button-white.png");
        }

        private void UpdateWindowRegion()
        {
            using (GraphicsPath path = CreateRoundedPath(new Rectangle(Point.Empty, Size), 20))
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

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, new IntPtr(HTCAPTION), IntPtr.Zero);
        }

        private static Bitmap LoadBitmap(string fileName)
        {
            return AssetStore.LoadBitmap("icon\\" + fileName);
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

        private static void OpenExternal(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
