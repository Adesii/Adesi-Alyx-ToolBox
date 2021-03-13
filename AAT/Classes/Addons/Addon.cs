using AAT.Soundevents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace AAT.Addons
{
    public class Addon
    {
        public string AddonName = "N/A";

        public bool OnlySoundeventsFolder;

        public string Path = "/";
        private bool refreshList = false;

        public ObservableCollection<SoundeventFile> AddonSpecificAddonFiles = new ObservableCollection<SoundeventFile>();
        private ObservableCollection<Soundevent> m_soundevents;
        public ObservableCollection<Soundevent> AllSoundevents
        {
            get
            {
                if(m_soundevents == null || refreshList)
                {
                    m_soundevents = new ObservableCollection<Soundevent>();
                    foreach (var item in AddonSpecificAddonFiles)
                    {
                        foreach (var events in item.Soundevents)
                        {
                            m_soundevents.Add(events);
                        }
                    }
                }
                System.Diagnostics.Debug.Print($"Count of all Soundevents in addon:{AddonName} = {m_soundevents.Count}");
                return m_soundevents;
            }
        }


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
        public void ApplyChanges()
        {

        }
        public void AddNewSoundEvent(Soundevent soundevent)
        {
            foreach (var item in AddonSpecificAddonFiles)
            {
                if (item.FileName.Equals(soundevent.FileName))
                {
                    item.Soundevents.Add(soundevent);
                }
            }
            AddonSpecificAddonFiles.Add(new SoundeventFile(soundevent.FileName));
        }
    }
}
