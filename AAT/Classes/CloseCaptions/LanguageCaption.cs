using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using HLACaptionReplacer;
using System.Linq;
using System.Diagnostics;

namespace AAT.Classes.CloseCaptions
{
    public class LanguageCaption : IComparable
    {
        public string language { get; set; }
        public ClosedCaptions captionFile;

        private ObservableCollection<CaptionSoundevent> m_structing;
        public bool isCompiling = false;
        public ObservableCollection<CaptionSoundevent> CaptionStruct
        {
            get
            {
                if (m_structing == null || m_structing.Count <= 0 && !isCompiling)
                {
                    FillCombineStruct();
                    isCompiling = true;
                }
                return m_structing;
            }
            set
            {
                m_structing.Clear();
                m_structing = value;
            }
        }

        public struct CaptionSoundevent
        {
            public LanguageCaption LanguageCaption { get; set; }
            public ClosedCaption Caption { get; set; }
            public Soundevents.Soundevent Event { get; set; }

        }

        public LanguageCaption(string Language)
        {
            language = Language;
        }
        public void RefreshCaptions()
        {

        }

        private async void FillCombineStruct()
        {
            List<string> foundHashes = new();
            List<CaptionSoundevent> b = new();
            await Task.Run(() =>
            {

                foreach (var item in captionFile.Captions)
                {
                    var caption_found = false;

                    Soundevents.SoundeventBuilder.Instance.AllBaseSoundEvents.AsParallel().ForAll((e) =>
                    {
                        if (item.SoundEventHash == e.Hash)
                        {
                            b.Add(new CaptionSoundevent()
                            {
                                LanguageCaption = this,
                                Caption = item,
                                Event = e

                            });
                            caption_found = true;
                        }
                    });
                    if (!caption_found)
                        b.Add(new CaptionSoundevent()
                        {
                            LanguageCaption = this,
                            Caption = item,
                            Event = new Soundevents.Soundevent(item.SoundEventHash.ToString())

                        }) ;

                }

                
            });
            m_structing = new ObservableCollection<CaptionSoundevent>(b);
            Pages.CaptionEditor.Instance.SetSource();
            Debug.WriteLine($"Done Switching Language to : {language}");

        }
        public int CompareTo(object obj)
        {
            var lc = obj as LanguageCaption;
            if (lc == null) return 0;
            return language.CompareTo(lc.language);
        }
    }
}
