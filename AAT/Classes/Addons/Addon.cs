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

        public ResourceManifest manifest;

        public Dictionary<string,SoundeventFile> AddonSpecificAddonFiles = new ();
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
                        foreach (var events in item.Value.Soundevents)
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
            manifest = TryGetManifestorCreate();
        }

        public ResourceManifest TryGetManifestorCreate()
        {
            return new ResourceManifest(this);
        }
        public override string ToString()
        {
            return AddonName;
        }
        public void ApplyChanges()
        {
            manifest.Files = AddonSpecificAddonFiles.Values.ToList();
            manifest.SaveFile();
        }
        public void AddNewSoundEvent(Soundevent soundevent)
        {
            if (!AddonSpecificAddonFiles.ContainsKey(soundevent.FileName)) AddonSpecificAddonFiles[soundevent.FileName] = new SoundeventFile(soundevent.FileName);
            AddonSpecificAddonFiles[soundevent.FileName].Soundevents.Add(soundevent);
        }
    }
}
