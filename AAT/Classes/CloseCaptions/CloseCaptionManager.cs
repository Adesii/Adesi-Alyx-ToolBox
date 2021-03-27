using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using HLACaptionReplacer;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Linq;

namespace AAT.CloseCaptions
{
    public static class CloseCaptionManager
    {

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
                if (Soundevents.SoundeventBuilder.CaptionDictionary[CurrLang] != null)
                    return Soundevents.SoundeventBuilder.CaptionDictionary[CurrLang];
                else return null;
            }
        }

        public static void LoadCaptions(bool reload = false)
        {
            if (!reload) return;
            Regex reger = new("(_)\\w+");
            Directory.GetFiles(Path.Combine(Addons.AddonFolderWatcher.GetInstallPath(), "game/hlvr/resource/subtitles")).AsParallel().ForAll((item) =>
            {
               LanguageCaption lc = new(reger.Match(item).Value[1..]);
               lc.captionFile = new ClosedCaptions();
               lc.captionFile.Read(File.OpenRead(item));
                Soundevents.SoundeventBuilder.CaptionDictionary.Add(lc.language,lc);
                //Debug.WriteLine(lc.language);
            });
            Pages.CaptionEditor.Instance.SetSource();
        }

    }
}
