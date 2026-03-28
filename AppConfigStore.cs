using System;
using System.IO;
using System.Xml.Serialization;

namespace SpotiKnob
{
    [Serializable]
    public sealed class SpotiKnobConfig
    {
        public string ClockwiseHotkey { get; set; }
        public string CounterClockwiseHotkey { get; set; }
        public string PressHotkey { get; set; }
        public string ModeCycleHotkey { get; set; }
        public bool StartWithWindows { get; set; }
        public bool RunAsAdministrator { get; set; }
        public bool StartMinimized { get; set; }
    }

    internal static class AppConfigStore
    {
        private static readonly object SyncRoot = new object();
        private static SpotiKnobConfig current;

        public static SpotiKnobConfig Current
        {
            get
            {
                EnsureLoaded();
                return current;
            }
        }

        public static string ConfigDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SpotiKnob");
            }
        }

        public static string ConfigPath
        {
            get { return Path.Combine(ConfigDirectory, "config.xml"); }
        }

        public static void Save()
        {
            lock (SyncRoot)
            {
                EnsureLoaded();
                Directory.CreateDirectory(ConfigDirectory);

                XmlSerializer serializer = new XmlSerializer(typeof(SpotiKnobConfig));
                using (FileStream stream = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    serializer.Serialize(stream, current);
                }
            }
        }

        private static void EnsureLoaded()
        {
            if (current != null)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (current != null)
                {
                    return;
                }

                current = LoadConfig() ?? CreateFromLegacySettings();
                Save();
            }
        }

        private static SpotiKnobConfig LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                return null;
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SpotiKnobConfig));
                using (FileStream stream = new FileStream(ConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return serializer.Deserialize(stream) as SpotiKnobConfig;
                }
            }
            catch
            {
                return null;
            }
        }

        private static SpotiKnobConfig CreateFromLegacySettings()
        {
            return new SpotiKnobConfig
            {
                ClockwiseHotkey = Properties.Settings.Default.ClockwiseHotkey,
                CounterClockwiseHotkey = Properties.Settings.Default.CounterClockwiseHotkey,
                PressHotkey = Properties.Settings.Default.PressHotkey,
                ModeCycleHotkey = Properties.Settings.Default.ModeCycleHotkey,
                StartWithWindows = Properties.Settings.Default.StartWithWindows,
                RunAsAdministrator = Properties.Settings.Default.RunAsAdministrator,
                StartMinimized = Properties.Settings.Default.StartMinimized
            };
        }
    }
}
