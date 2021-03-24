using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using HLACaptionReplacer;

namespace AAT.Classes.CloseCaptions
{
    public static class CloseCaptionManager
    {
        public static List<LanguageCaption> AddonSpecificCaptions = new List<LanguageCaption>();

        public static string CurrLang = "english";
        public static ObservableCollection<ClosedCaptions> languageSpecifCaptions
        {
            get
            {
                return new ObservableCollection<ClosedCaptions>((IEnumerable<ClosedCaptions>)AddonSpecificCaptions.Find((e) =>
                {
                    return e.language.Equals(CurrLang);
                }).captions);
            }
        }

    }
}
