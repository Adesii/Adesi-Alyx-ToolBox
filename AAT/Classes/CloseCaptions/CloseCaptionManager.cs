using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using HLACaptionReplacer;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AAT.Classes.CloseCaptions
{
    public static class CloseCaptionManager
    {
        public static List<LanguageCaption> AddonSpecificCaptions = new();



        public static string CurrLang = "english";
        public static ObservableCollection<ClosedCaption> LanguageSpecificCaptions
        {
            get
            {
                return new ObservableCollection<ClosedCaption>(AddonSpecificCaptions.Find((e) =>
                {
                    return e.language.Equals(CurrLang);
                }).captionFile.Captions);
            }
        }

        public static async void LoadCaptions()
        {
            Regex reger = new("(_)\\w+");
            foreach (var item in Directory.GetFiles(Path.Combine(Addons.AddonFolderWatcher.GetInstallPath(), "game/hlvr/resource/subtitles")))
            {
                LanguageCaption lc = new(reger.Match(item).Value[1..]);
                lc.captionFile = new ClosedCaptions();
                await Task.Run(()=> { lc.captionFile.Read(File.OpenRead(item)); });
                AddonSpecificCaptions.Add(lc);
                //Debug.WriteLine(lc.language);
            }

        }

    }
}
