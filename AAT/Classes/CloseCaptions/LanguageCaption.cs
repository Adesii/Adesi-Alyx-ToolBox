using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using HLACaptionReplacer;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

namespace AAT.CloseCaptions
{
    public class LanguageCaption : IComparable
    {
        [Newtonsoft.Json.JsonIgnore]
        public string language { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ClosedCaptions captionFile;
        [Newtonsoft.Json.JsonIgnore]
        private List<ClosedCaptionWrapperClass> ConvertedList;
        public List<ClosedCaptionWrapperClass> GetCaptions
        {
            get
            {
                if(ConvertedList == null)
                {
                    List<ClosedCaptionWrapperClass> c = new();
                    captionFile.Captions.ForEach((item) =>
                    {
                        c.Add(new ClosedCaptionWrapperClass(item));
                    });
                    ConvertedList = c;
                }
                return ConvertedList;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        private Dictionary<uint, ClosedCaption> m_captionsByHash;
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<uint, ClosedCaption> CaptionsByHash
        {
            get
            {
                if(m_captionsByHash == null)
                {
                    m_captionsByHash = new();
                    foreach (var item in captionFile.Captions)
                    {
                        m_captionsByHash.Add(item.SoundEventHash,item);
                    }
                }
                return m_captionsByHash;
            }
            set
            {
                m_captionsByHash = value;
            }
        }

        public LanguageCaption(string Language)
        {
            language = Language;
        }
        public void RefreshCaptions()
        {

        }
        public int CompareTo(object obj)
        {
            var lc = obj as LanguageCaption;
            if (lc == null) return 0;
            return language.CompareTo(lc.language);
        }
    }
}
