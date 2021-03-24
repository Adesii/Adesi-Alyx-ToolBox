using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AAT.Addons
{
    public static class AddonManager
    {
        public static Action AddonChanged;
        public static Addon CurrentAddon = new Addon("/");
        public static int CurrentFileIndex = 0;

        public static ObservableCollection<Addon> Addons
        {
            get
            {
                if (addons == null)
                {
                    addons = addons = new ObservableCollection<Addon>(GetAddons(true));
                }
                return addons;

            }
        }
        private static ObservableCollection<Addon> addons;

        public static List<Addon> GetAddons(bool setCurrentAddonToFirst = false)
        {
            var paths = GetAddonPaths();
            List<Addon> addonlist = new List<Addon>();

            foreach (var item in paths)
            {
                addonlist.Add(new Addon(item));
            }
            if (setCurrentAddonToFirst && addonlist.Count > 0)
                ChangeAddon(addonlist[0]);
            return addonlist;
        }

        public static List<string> GetAddonPaths()
        {
            List<string> AddonPaths = new List<string>();
            //Debug.Print(Properties.Settings.Default.InstallPath);
            var addonP = Properties.Settings.Default.InstallPath + "/content/hlvr_addons";
            foreach (var item in Directory.GetDirectories(addonP))
            {
                AddonPaths.Add(item);
            }
            return AddonPaths;
        }
        public static void ChangeAddon(Addon to)
        {
            CurrentAddon.ApplyChanges();
            CurrentAddon = to;
            AddonChanged?.Invoke();
        }
    }
}
