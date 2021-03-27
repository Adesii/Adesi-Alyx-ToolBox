using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AAT.Soundevents;
using System.IO;

namespace AAT.Addons
{
    public class ResourceManifest : Interfaces.IWriteable
    {
        public static string Header = "<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->";
        public Addon ownAddon;
        public List<SoundeventFile> Files = new();

        public ResourceManifest(Addon owner)
        {
            ownAddon = owner;
        }
        public void SaveFile()
        {
            
            foreach (var EventFile in Files)
            {
                EventFile.SaveFileToEventsFolder(Path.Combine(ownAddon.Path,"soundevents"));
            }


        }
    }
}
