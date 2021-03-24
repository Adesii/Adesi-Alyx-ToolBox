using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using HLACaptionReplacer;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AAT.Classes.CloseCaptions.LanguageCaption;

namespace AAT.Classes.CloseCaptions
{
    public static class CloseCaptionManager
    {
        public static SortedDictionary<string, LanguageCaption> AddonSpecificCaptions = new();

        public static Action LanguageChanged;
        private static string m_lang = "english";
        public static string CurrLang
        {
            get
            {
                return m_lang;
            }
            set
            {
                m_lang = value;
                LanguageChanged?.Invoke();
            }
        }
        public static LanguageCaption LanguageSpecificCaptions
        {
            get
            {
                AddonSpecificCaptions.TryGetValue(CurrLang, out LanguageCaption val);
                if (val != null)
                    return val;
                else return null;
            }
        }

        public static async void LoadCaptions(bool reload = false)
        {
            if (AddonSpecificCaptions.Count > 0 && !reload) return;
            Regex reger = new("(_)\\w+");
            foreach (var item in Directory.GetFiles(Path.Combine(Addons.AddonFolderWatcher.GetInstallPath(), "game/hlvr/resource/subtitles")))
            {
                LanguageCaption lc = new(reger.Match(item).Value[1..]);
                lc.captionFile = new ClosedCaptions();
                await Task.Run(() => { lc.captionFile.Read(File.OpenRead(item)); });
                AddonSpecificCaptions.Add(lc.language,lc);
                //Debug.WriteLine(lc.language);
            }
            Pages.CaptionEditor.Instance.SetSource();
        }

    }
}
