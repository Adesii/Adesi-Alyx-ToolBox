using System;
using System.Collections.Generic;
using System.Text;
using HLACaptionReplacer;

namespace AAT.Classes.CloseCaptions
{
    public class LanguageCaption
    {
        public string language = "N/A";
        public ClosedCaptions captionFile;

        public LanguageCaption(string Language)
        {
            language = Language;
        }
        public void RefreshCaptions()
        {
            
        }
    }
}
