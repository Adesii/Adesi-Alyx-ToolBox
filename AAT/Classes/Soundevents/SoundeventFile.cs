using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace AAT.Soundevents
{
    public class SoundeventFile
    {
        public string FileName = "base";
        public ObservableCollection<Soundevent> Soundevents = new ();

        public SoundeventFile(string FileName= "base")
        {
            this.FileName = FileName;
        }
        public void SaveFileToEventsFolder(string path)
        {
            var serialized = Addons.AddonHelper.Serialize(Soundevents);
            var finalPath = Path.Combine(path, FileName + "_soundevents.vsndevts");
            File.WriteAllTextAsync(finalPath, serialized);
        }
    }
}
