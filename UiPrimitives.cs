using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SpotiKnob
{
    public sealed class RoundedPanel : Panel
    {
        private int cornerRadius = 30;
        private Color borderColor = Color.FromArgb(44, 52, 75);
        private int borderThickness = 1;
        private bool useGradient;
        private Color gradientStartColor = Color.FromArgb(9, 12, 20);
        private Color gradientEndColor = Color.FromArgb(6, 8, 14);

        public RoundedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = AppTheme.CardBackground;
        }

        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = Math.Max(1, value);
                Invalidate();
                UpdateRegion();
            }
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = Math.Max(1, value);
                Invalidate();
            }
        }

        public bool UseGradient
        {
            get { return useGradient; }
            set
            {
                useGradient = value;
                Invalidate();
            }
        }

        public Color GradientStartColor
        {
            get { return gradientStartColor; }
            set
            {
                gradientStartColor = value;
                Invalidate();
            }
        }

        public Color GradientEndColor
        {
            get { return gradientEndColor; }
            set
            {
                gradientEndColor = value;
                Invalidate();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = CreateRoundPath(ClientRectangle, cornerRadius))
            {
                if (useGradient)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        ClientRectangle,
                        gradientStartColor,
                        gradientEndColor,
                        LinearGradientMode.ForwardDiagonal))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle borderBounds = new Rectangle(
                borderThickness / 2,
                borderThickness / 2,
                Width - borderThickness,
                Height - borderThickness);

            using (GraphicsPath path = CreateRoundPath(borderBounds, cornerRadius))
            using (Pen pen = new Pen(borderColor, borderThickness))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = CreateRoundPath(ClientRectangle, cornerRadius))
            {
                Region = new Region(path);
            }
        }

        private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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
    }

    public enum MediaButtonGlyph
    {
        Previous,
        Play,
        Next
    }

    public enum WindowControlGlyph
    {
        Minimize,
        Close
    }

    public sealed class MediaBindButton : Button
    {
        private bool isListening;
        private bool isHovered;
        private MediaButtonGlyph glyph;

        public MediaBindButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            TabStop = false;
            BackColor = Color.Transparent;
            Size = new Size(44, 44);
        }

        public MediaButtonGlyph Glyph
        {
            get { return glyph; }
            set
            {
                glyph = value;
                Invalidate();
            }
        }

        public bool IsListening
        {
            get { return isListening; }
            set
            {
                isListening = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(new Rectangle(0, 0, Width, Height));
                Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = isListening
                ? AppTheme.SpotifyAccent
                : (isHovered ? Color.FromArgb(255, 255, 255) : Color.FromArgb(229, 231, 235));
            Color iconColor = Color.FromArgb(16, 18, 24);

            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                pevent.Graphics.FillEllipse(fillBrush, ClientRectangle);
            }

            DrawGlyph(pevent.Graphics, iconColor);

            if (Focused || isListening)
            {
                Rectangle focusBounds = new Rectangle(1, 1, Width - 3, Height - 3);
                using (Pen pen = new Pen(AppTheme.SpotifyAccent, 2f))
                {
                    pevent.Graphics.DrawEllipse(pen, focusBounds);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using (SolidBrush brush = new SolidBrush(Parent.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            base.OnPaintBackground(pevent);
        }

        private void DrawGlyph(Graphics graphics, Color iconColor)
        {
            using (SolidBrush brush = new SolidBrush(iconColor))
            {
                switch (glyph)
                {
                    case MediaButtonGlyph.Previous:
                        graphics.FillRectangle(brush, 13, 14, 3, 14);
                        graphics.FillPolygon(brush, new[]
                        {
                            new Point(17, 21),
                            new Point(28, 13),
                            new Point(28, 29)
                        });
                        break;
                    case MediaButtonGlyph.Next:
                        graphics.FillRectangle(brush, 28, 14, 3, 14);
                        graphics.FillPolygon(brush, new[]
                        {
                            new Point(15, 13),
                            new Point(26, 21),
                            new Point(15, 29)
                        });
                        break;
                    default:
                        graphics.FillPolygon(brush, new[]
                        {
                            new Point(17, 13),
                            new Point(30, 21),
                            new Point(17, 29)
                        });
                        break;
                }
            }
        }
    }

    public sealed class WindowControlButton : Button
    {
        private bool isHovered;
        private bool isPressed;
        private WindowControlGlyph glyph;

        public WindowControlButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = AppTheme.PrimaryText;
            Cursor = Cursors.Hand;
            Size = new Size(32, 32);
            TabStop = false;
        }

        public WindowControlGlyph Glyph
        {
            get { return glyph; }
            set
            {
                glyph = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width, Height), 10))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = isPressed
                ? Color.FromArgb(45, 55, 72)
                : (isHovered ? Color.FromArgb(30, 41, 59) : Color.FromArgb(17, 24, 39));

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 10))
            using (SolidBrush brush = new SolidBrush(fillColor))
            using (Pen borderPen = new Pen(Color.FromArgb(55, 75, 97), 1f))
            using (Pen glyphPen = new Pen(isHovered && glyph == WindowControlGlyph.Close ? Color.FromArgb(252, 165, 165) : AppTheme.PrimaryText, 1.8f))
            {
                pevent.Graphics.FillPath(brush, path);
                pevent.Graphics.DrawPath(borderPen, path);

                if (glyph == WindowControlGlyph.Minimize)
                {
                    pevent.Graphics.DrawLine(glyphPen, 10, 17, 22, 17);
                }
                else
                {
                    pevent.Graphics.DrawLine(glyphPen, 11, 11, 21, 21);
                    pevent.Graphics.DrawLine(glyphPen, 21, 11, 11, 21);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using (SolidBrush brush = new SolidBrush(Parent.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            base.OnPaintBackground(pevent);
        }

        private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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
    }

    public sealed class SettingsPillToggle : CheckBox
    {
        private bool isHovered;
        private bool isPressed;
        private bool isActive;

        public SettingsPillToggle()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            Appearance = Appearance.Button;
            AutoSize = false;
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = AppTheme.CreateUiFont(10f);
            Padding = new Padding(16, 0, 0, 0);
            TextAlign = ContentAlignment.MiddleLeft;
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                Invalidate();
            }
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 18))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = Checked || isActive
                ? Color.FromArgb(21, 38, 75)
                : Color.FromArgb(11, 17, 31);
            if (isHovered)
            {
                fillColor = Checked || isActive
                    ? Color.FromArgb(24, 46, 89)
                    : Color.FromArgb(14, 21, 38);
            }
            if (isPressed)
            {
                fillColor = Color.FromArgb(26, 47, 90);
            }

            Color borderColor = Checked || isActive
                ? Color.FromArgb(45, 86, 166)
                : Color.FromArgb(38, 61, 101);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 18))
            using (SolidBrush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(borderColor, 1.25f))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(41, 216, 255)))
            using (StringFormat format = new StringFormat())
            {
                format.LineAlignment = StringAlignment.Center;
                pevent.Graphics.FillPath(brush, path);
                pevent.Graphics.DrawPath(pen, path);

                Rectangle textBounds = new Rectangle(Padding.Left, 0, Width - Padding.Left - 10, Height);
                pevent.Graphics.DrawString(Text, Font, textBrush, textBounds, format);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using (SolidBrush brush = new SolidBrush(Parent.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            base.OnPaintBackground(pevent);
        }

        private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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
    }

    public sealed class SettingsPillButton : Button
    {
        private bool isHovered;
        private bool isPressed;
        private bool isActive;

        public SettingsPillButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = AppTheme.CreateUiFont(10f);
            Padding = new Padding(16, 0, 0, 0);
            TabStop = true;
        }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 18))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = isActive
                ? Color.FromArgb(21, 38, 75)
                : Color.FromArgb(11, 17, 31);
            if (isHovered)
            {
                fillColor = isActive
                    ? Color.FromArgb(24, 46, 89)
                    : Color.FromArgb(14, 21, 38);
            }
            if (isPressed)
            {
                fillColor = Color.FromArgb(26, 47, 90);
            }

            Color borderColor = isActive
                ? Color.FromArgb(45, 86, 166)
                : Color.FromArgb(38, 61, 101);
            Color textColor = isActive ? Color.FromArgb(125, 211, 252) : Color.FromArgb(41, 216, 255);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 18))
            using (SolidBrush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(borderColor, 1.25f))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (StringFormat format = new StringFormat())
            {
                format.LineAlignment = StringAlignment.Center;
                pevent.Graphics.FillPath(brush, path);
                pevent.Graphics.DrawPath(pen, path);

                Rectangle textBounds = new Rectangle(Padding.Left, 0, Width - Padding.Left - 10, Height);
                pevent.Graphics.DrawString(Text, Font, textBrush, textBounds, format);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using (SolidBrush brush = new SolidBrush(Parent.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            base.OnPaintBackground(pevent);
        }

        private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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
    }

    public sealed class ShieldBadge : Control
    {
        public ShieldBadge()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Point[] outline =
            {
                new Point(Width / 2, 2),
                new Point(Width - 4, 8),
                new Point(Width - 4, Height / 2 + 2),
                new Point(Width / 2, Height - 3),
                new Point(4, Height / 2 + 2),
                new Point(4, 8)
            };

            Rectangle rightPanel = new Rectangle(Width / 2, 9, (Width / 2) - 6, Height - 20);

            using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(252, 176, 36)))
            using (SolidBrush splitBrush = new SolidBrush(Color.FromArgb(255, 201, 87)))
            using (Pen outlinePen = new Pen(Color.FromArgb(255, 189, 52), 1.1f))
            {
                e.Graphics.FillPolygon(fillBrush, outline);
                e.Graphics.FillRectangle(splitBrush, rightPanel);
                e.Graphics.DrawPolygon(outlinePen, outline);
                e.Graphics.DrawLine(outlinePen, Width / 2, 4, Width / 2, Height - 6);
            }
        }
    }

    internal sealed class SocialLinkButton : Button
    {
        private Bitmap icon;
        private string caption;
        private bool isHovered;
        private bool isPressed;
        private int iconPillWidth = 62;
        private int iconSize = 28;
        private int textOffset = 74;

        public SocialLinkButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            TabStop = true;
        }

        public Bitmap Icon
        {
            get { return icon; }
            set
            {
                if (icon != null && !ReferenceEquals(icon, value))
                {
                    icon.Dispose();
                }

                icon = value;
                Invalidate();
            }
        }

        public int IconPillWidth
        {
            get { return iconPillWidth; }
            set
            {
                iconPillWidth = value;
                Invalidate();
            }
        }

        public int IconSize
        {
            get { return iconSize; }
            set
            {
                iconSize = value;
                Invalidate();
            }
        }

        public int TextOffset
        {
            get { return textOffset; }
            set
            {
                textOffset = value;
                Invalidate();
            }
        }

        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && icon != null)
            {
                icon.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle pillBounds = new Rectangle(0, 0, Math.Min(iconPillWidth, Width - 1), Height - 1);
            Color fillColor = isPressed
                ? Color.FromArgb(234, 234, 234)
                : (isHovered ? Color.FromArgb(248, 248, 248) : Color.White);
            Color borderColor = isHovered ? Color.FromArgb(213, 213, 213) : Color.FromArgb(225, 225, 225);

            using (GraphicsPath path = CreateRoundPath(pillBounds, 16))
            using (SolidBrush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(borderColor, 1f))
            using (SolidBrush textBrush = new SolidBrush(AppTheme.PrimaryText))
            using (StringFormat format = new StringFormat())
            {
                format.LineAlignment = StringAlignment.Center;
                pevent.Graphics.FillPath(brush, path);
                pevent.Graphics.DrawPath(pen, path);

                if (icon != null)
                {
                    Rectangle iconBounds = new Rectangle(
                        (pillBounds.Width - iconSize) / 2,
                        (Height - iconSize) / 2,
                        iconSize,
                        iconSize);
                    pevent.Graphics.DrawImage(icon, iconBounds);
                }

                Rectangle textBounds = new Rectangle(textOffset, 0, Math.Max(0, Width - textOffset), Height);
                pevent.Graphics.DrawString(caption ?? string.Empty, AppTheme.CreateMonoFont(13f), textBrush, textBounds, format);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using (SolidBrush brush = new SolidBrush(Parent.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            base.OnPaintBackground(pevent);
        }

        private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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
    }

    public sealed class ImageActionButton : Button
    {
        private Bitmap image;
        private bool isHovered;
        private bool isPressed;

        public ImageActionButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor,
                true);
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            TabStop = true;
        }

        public Bitmap DisplayImage
        {
            get { return image; }
            set
            {
                if (image != null && !ReferenceEquals(image, value))
                {
                    image.Dispose();
                }

                image = value;
                Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && image != null)
            {
                image.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 10))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color borderColor = isHovered ? Color.FromArgb(218, 218, 218) : Color.FromArgb(235, 235, 235);
            Color fillColor = isPressed
                ? Color.FromArgb(242, 242, 242)
                : (isHovered ? Color.FromArgb(252, 252, 252) : Color.White);

            using (GraphicsPath path = CreateRoundPath(new Rectangle(0, 0, Width - 1, Height - 1), 10))
            using (SolidBrush brush = new SolidBrush(fillColor))
            using (Pen pen = new Pen(borderColor, 1f))
            {
                pevent.Graphics.FillPath(brush, path);
                pevent.Graphics.DrawPath(pen, path);

                if (image != null)
                {
                    int imageWidth = Math.Min(image.Width, Width - 18);
                    int imageHeight = Math.Min(image.Height, Height - 12);
                    Rectangle imageBounds = new Rectangle(
                        (Width - imageWidth) / 2,
                        (Height - imageHeight) / 2,
                        imageWidth,
                        imageHeight);
                    pevent.Graphics.DrawImage(image, imageBounds);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null)
            {
                using (SolidBrush brush = new SolidBrush(Parent.BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, ClientRectangle);
                }
                return;
            }

            base.OnPaintBackground(pevent);
        }

        private static GraphicsPath CreateRoundPath(Rectangle bounds, int radius)
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
    }
}
