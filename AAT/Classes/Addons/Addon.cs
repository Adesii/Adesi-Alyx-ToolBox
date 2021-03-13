using AAT.Soundevents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AAT.Addons
{
    public class Addon
    {
        public string AddonName = "N/A";

        public bool OnlySoundeventsFolder;

        public string Path = "/";

        public List<SoundeventFile> AddonSpecificAddonFiles = new List<SoundeventFile>(); 

        public Addon(string path, bool onlySoundeventsFolder = false)
        {
            AddonName = path[(path.LastIndexOf("\\") + 1)..];
            OnlySoundeventsFolder = onlySoundeventsFolder;
            Path = path;
        }

        public override string ToString()
        {
            return AddonName;
        }
    }
}
