using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SpotiKnob
{
    public partial class ModeOverlayForm : Form
    {
        private const int CornerRadius = 20;
        private const int ScreenMargin = 20;
        private const double MaxOpacityLevel = 0.94d;
        private const double FadeStep = 0.08d;
        private const int HoldTicks = 75;

        private int holdTickCount;
        private OverlayAnimationState animationState;

        public ModeOverlayForm()
        {
            InitializeComponent();
            animationTimer.Tick += AnimationTimer_Tick;
        }

        public void ShowMode(string title, string description, Color accentColor, string glyph)
        {
            titleLabel.Text = title;
            descriptionLabel.Text = description ?? string.Empty;
            cardPanel.BorderColor = Color.FromArgb(72, accentColor);

            Rectangle workingArea = Screen.FromPoint(Cursor.Position).WorkingArea;
            Location = new Point(
                workingArea.Right - Width - ScreenMargin,
                workingArea.Top + ScreenMargin);

            holdTickCount = 0;
            animationState = OverlayAnimationState.FadingIn;

            if (!Visible)
            {
                Show();
            }

            Opacity = 0d;
            animationTimer.Start();
        }

        public void ShowNotification(string title, string description, Color accentColor)
        {
            ShowMode(title, description, accentColor, string.Empty);
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WsExNoActivate = 0x08000000;
                const int WsExToolWindow = 0x00000080;

                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WsExNoActivate | WsExToolWindow;
                return createParams;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateRoundedRegion();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRoundedRegion();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            switch (animationState)
            {
                case OverlayAnimationState.FadingIn:
                    Opacity = Math.Min(MaxOpacityLevel, Opacity + FadeStep);
                    if (Opacity >= MaxOpacityLevel)
                    {
                        animationState = OverlayAnimationState.Holding;
                    }
                    break;
                case OverlayAnimationState.Holding:
                    holdTickCount++;
                    if (holdTickCount >= HoldTicks)
                    {
                        animationState = OverlayAnimationState.FadingOut;
                    }
                    break;
                case OverlayAnimationState.FadingOut:
                    Opacity = Math.Max(0d, Opacity - FadeStep);
                    if (Opacity <= 0d)
                    {
                        animationTimer.Stop();
                        Hide();
                    }
                    break;
            }
        }

        private void UpdateRoundedRegion()
        {
            using (GraphicsPath path = CreateRoundedPath(ClientRectangle, CornerRadius))
            {
                Region = new Region(path);
            }
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

        private enum OverlayAnimationState
        {
            FadingIn,
            Holding,
            FadingOut
        }
    }
}
