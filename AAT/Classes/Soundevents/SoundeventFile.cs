using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AAT.Soundevents
{
    public class SoundeventFile
    {
        public string FileName = "base";
        public ObservableCollection<Soundevent> Soundevents = new ObservableCollection<Soundevent>();

        public SoundeventFile(string FileName= "base")
        {
            this.FileName = FileName;
        }
    }
}
