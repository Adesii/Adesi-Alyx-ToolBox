using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ValveKeyValue;

namespace AAT.Addons
{
    public static class AddonFolderWatcher
    {
        public static string GetInstallPath()
        {
            var c = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
            if (c == null) return Properties.Settings.Default.InstallPath;
            var path = GetAlyxPath(c.ToString());
            Properties.Settings.Default.InstallPath = path;
            return path;
        }
        public static string GetAlyxPath(string SteamPath)
        {
            List<string> paths = new List<string>();
            if (Directory.Exists(SteamPath))
            {
                paths.Add(SteamPath);
                if (File.Exists(SteamPath + "\\steamapps\\libraryfolders.vdf"))
                {
                    var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
                    var kvalues = kv.Deserialize(File.OpenRead(SteamPath + "\\steamapps\\libraryfolders.vdf"));

                    foreach (var item in kvalues)
                    {
                        if (!int.TryParse(item.Name, out int num)) continue;
                        paths.Add(item.Value.ToString());
                    }
                }
            }
            foreach (var item in paths)
            {
                if (Directory.Exists(item + "\\steamapps\\common\\Half-Life Alyx\\game\\hlvr"))
                    return item.Replace("\\\\", "/", System.StringComparison.Ordinal) + "/steamapps/common/Half-Life Alyx";
            }

            return null;
        }
        static FileSystemWatcher watcher;
        public static event EventHandler<FileSystemEventArgs> SoundFileCreated;
        public static event EventHandler<RenamedEventArgs> SoundFileRenamed;
        public static event EventHandler<FileSystemEventArgs> SoundfileDeleted;
        public static void AddWatcher(string AddonName)
        {
            if (watcher != null)
            {
                ClearWatcher();
            }
            string pathToWatch;
            if (AddonName.Contains("\\"))
            {
                pathToWatch = AddonName;
            }
            else
            {
                pathToWatch = System.IO.Path.Combine(GetAlyxPath(GetInstallPath()), "content", "hlvr_addons");
            }
            watcher = new FileSystemWatcher(pathToWatch);
            watcher.Filter = "*.wav";
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Renamed;
            watcher.EnableRaisingEvents = true;
            
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            SoundFileRenamed?.Invoke(sender, e);
        }

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            SoundfileDeleted(sender, e);
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            SoundFileCreated(sender, e);
        }

        

        public static void ClearWatcher()
        {
            watcher.Dispose();
            watcher = null;
        }
    }
}
