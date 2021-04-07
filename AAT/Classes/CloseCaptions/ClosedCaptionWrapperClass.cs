using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLACaptionCompiler;
using AAT.Addons;
using AAT.Soundevents;
using System.ComponentModel;

namespace AAT.CloseCaptions
{
    public class ClosedCaptionWrapperClass : INotifyPropertyChanged
    {
        [Newtonsoft.Json.JsonIgnore]
        private ClosedCaption Parent;
        public event PropertyChangedEventHandler PropertyChanged;
        public ClosedCaptionWrapperClass(ClosedCaption c)
        {
            Parent = c;
        }
        [Newtonsoft.Json.JsonIgnore]
        public ClosedCaption GetParent
        {
            get => Parent;
        }
        public uint GetHash
        {
            get => Parent.SoundEventHash;
        }
        public string GetText
        {
            get => Parent.RawDefinition;
            set
            {
                Parent.Definition = value;
                NotifyPropertyChanged(nameof(GetText));
                NotifyPropertyChanged(nameof(GetClearerText));
                Addon.RefreshDictionaries();
            }
        }
        public string GetRealName
        {
            get
            {
                //if (SoundeventBuilder.SoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                if (Addon.AvailableSoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                    return v.EventName;
                return Parent.SoundEventHash.ToString();
            }
        }
        public string GetMetaCharacter
        {
            get
            {
                //if (SoundeventBuilder.SoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                if (Addon.AvailableSoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                    return v.GetMeta;
                return "";
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string GetClearerText
        {
            get
            {
                if (CloseCaptionManager.CurrLang == "english" && Addon.AvailableSoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                {
                    var text = v.LineText;
                    if (!string.IsNullOrWhiteSpace(text))
                        return text;
                }

                return Parent.Definition;
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
