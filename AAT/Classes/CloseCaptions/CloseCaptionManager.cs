using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using HLACaptionCompiler;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Linq;
using System.Threading;

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
                if (Soundevents.SoundeventBuilder.CaptionDictionary.ContainsKey(CurrLang) && Soundevents.SoundeventBuilder.CaptionDictionary?[CurrLang] != null)
                    return Soundevents.SoundeventBuilder.CaptionDictionary[CurrLang];
                else return null;
            }
        }

        public static Task LoadCaptions()
        {
            Regex reger = new("(_)\\w+");
            Directory.GetFiles(Path.Combine(Addons.AddonFolderWatcher.GetInstallPath(), "game/hlvr/resource/subtitles")).AsParallel().ForAll((item) =>
            {
               LanguageCaption lc = new(reger.Match(item).Value[1..]);
               lc.captionFile = new ClosedCaptions();
               lc.captionFile.Read(File.OpenRead(item));
               Soundevents.SoundeventBuilder.CaptionDictionary.TryAdd(lc.language,lc);
                //Debug.WriteLine(lc.language);
            });
            Pages.CaptionEditor.Instance.Dispatcher.Invoke(()=> { Pages.CaptionEditor.Instance.SetSource(); });
            return Task.CompletedTask;
        }
    }
}
