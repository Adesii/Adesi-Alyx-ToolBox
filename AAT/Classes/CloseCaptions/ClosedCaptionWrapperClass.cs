using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLACaptionReplacer;
using AAT.Addons;
using AAT.Soundevents;
using System.ComponentModel;

namespace AAT.CloseCaptions
{
    public class ClosedCaptionWrapperClass : INotifyPropertyChanged
    {
        private ClosedCaption Parent;

        public event PropertyChangedEventHandler PropertyChanged;
        public ClosedCaptionWrapperClass(ClosedCaption c)
        {
            Parent = c;
        }
        public ClosedCaption GetParent
        {
            get => Parent;
        }
        public string GetText
        {
            get => Parent.Definition;
            set
            {
                Parent.Definition = value;
                NotifyPropertyChanged(nameof(GetText));
                NotifyPropertyChanged(nameof(GetClearerText));
            }
        }
        public string GetRealName
        {
            get
            {
                if (Addon.AvailableSoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                    return v.EventName;
                return Parent.SoundEventHash.ToString();
            }
        }

        public string GetMetaCharacter
        {
            get
            {
                if (Addon.AvailableSoundeventDictionary.TryGetValue(Parent.SoundEventHash, out Soundevent v))
                    return v.GetMeta;
                return "";
            }
        }

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
