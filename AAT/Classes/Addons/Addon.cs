using AAT.Soundevents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Linq;
using AAT.CloseCaptions;

namespace AAT.Addons
{
    public class Addon
    {
        public string AddonName = "N/A";

        public bool OnlySoundeventsFolder;

        public string Path = "/";
        private bool refreshList = false;

        public ResourceManifest Manifest;

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

        public static Dictionary<string, LanguageCaption> AvailableCloseCaptions
        {
            get
            {
                return AddonManager.CurrentAddon.Manifest.AddonCaptionDictionary.Concat(SoundeventBuilder.CaptionDictionary.Where(kvp => !Addons.AddonManager.CurrentAddon.Manifest.AddonCaptionDictionary.ContainsKey(kvp.Key))).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public static Dictionary<uint, Soundevent> AvailableSoundeventDictionary 
        {
            get
            {
                return AddonManager.CurrentAddon.Manifest.AddonSoundeventDictionary.Concat(SoundeventBuilder.SoundeventDictionary.Where(kvp => !Addons.AddonManager.CurrentAddon.Manifest.AddonSoundeventDictionary.ContainsKey(kvp.Key))).ToDictionary(x => x.Key, x => x.Value); ;
            }
        }
        public Addon(string path, bool onlySoundeventsFolder = false)
        {
            AddonName = path[(path.LastIndexOf("\\") + 1)..];
            OnlySoundeventsFolder = onlySoundeventsFolder;
            Path = path;
            Manifest = TryGetManifestorCreate();
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
            Manifest.Files = AddonSpecificAddonFiles.Values.ToList();
            Manifest.SaveFile();
        }
        public void AddNewSoundEvent(Soundevent soundevent)
        {
            if (!AddonSpecificAddonFiles.ContainsKey(soundevent.FileName)) AddonSpecificAddonFiles[soundevent.FileName] = new SoundeventFile(soundevent.FileName);
            AddonSpecificAddonFiles[soundevent.FileName].Soundevents.Add(soundevent);
        }
    }
}
