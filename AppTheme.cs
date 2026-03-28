using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace SpotiKnob
{
    public static class AppTheme
    {
        private static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        private static readonly object SyncRoot = new object();
        private static bool fontLoaded;

        public static Color WindowBackground
        {
            get { return Color.FromArgb(7, 11, 20); }
        }

        public static Color CardBackground
        {
            get { return Color.FromArgb(17, 24, 39); }
        }

        public static Color CardBorder
        {
            get { return Color.FromArgb(31, 41, 55); }
        }

        public static Color PrimaryText
        {
            get { return Color.FromArgb(243, 244, 246); }
        }

        public static Color SecondaryText
        {
            get { return Color.FromArgb(156, 163, 175); }
        }

        public static Color SpotifyAccent
        {
            get { return Color.FromArgb(147, 197, 253); }
        }

        public static Color SpotifyAccentBackground
        {
            get { return Color.FromArgb(30, 58, 138); }
        }

        public static Font CreateUiFont(float size)
        {
            return CreateUiFont(size, FontStyle.Regular);
        }

        public static Font CreateUiFont(float size, FontStyle style)
        {
            return new Font("Segoe UI", size, style, GraphicsUnit.Point);
        }

        public static Font CreateMonoFont(float size)
        {
            return CreateMonoFont(size, FontStyle.Regular);
        }

        public static Font CreateMonoFont(float size, FontStyle style)
        {
            EnsureFontLoaded();

            if (FontCollection.Families.Length > 0)
            {
                return new Font(FontCollection.Families[0], size, style, GraphicsUnit.Point);
            }

            return new Font("Consolas", size, style, GraphicsUnit.Point);
        }

        private static void EnsureFontLoaded()
        {
            lock (SyncRoot)
            {
                if (fontLoaded)
                {
                    return;
                }

                string[] candidatePaths =
                {
                    Path.Combine(Application.StartupPath, "font", "JetBrainsMonoNerdFont-Light.ttf"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "font", "JetBrainsMonoNerdFont-Light.ttf"),
                    Path.Combine(Application.StartupPath, "design", "font", "JetBrainsMonoNerdFontMono-LightItalic.ttf"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "design", "font", "JetBrainsMonoNerdFontMono-LightItalic.ttf")
                };

                foreach (string candidatePath in candidatePaths)
                {
                    if (File.Exists(candidatePath))
                    {
                        FontCollection.AddFontFile(candidatePath);
                        break;
                    }
                }

                fontLoaded = true;
            }
        }
    }
}
