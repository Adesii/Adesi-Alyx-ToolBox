using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AAT.Classes.Addons
{
    public static class AddonManager
    {
        public static string CurrentAddon = "CustomGame";
        public static List<Addon> GetAddons()
        {
            var paths = GetAddonPaths();
            List<Addon> addonlist = new List<Addon>();

            foreach (var item in paths)
            {
                addonlist.Add(new Addon(item));
            }
            return addonlist;
        }

        public static List<string> GetAddonPaths()
        {
            List<string> AddonPaths = new List<string>();
            //Debug.Print(Properties.Settings.Default.InstallPath);
            var addonP= Properties.Settings.Default.InstallPath+"/content/hlvr_addons";
            foreach (var item in Directory.GetDirectories(addonP))
            {
                AddonPaths.Add(item);
            }
            return AddonPaths;
        }
    }
}
