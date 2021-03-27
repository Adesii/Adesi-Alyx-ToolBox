using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLACaptionReplacer;
using AAT.Addons;
using AAT.Soundevents;

namespace AAT.CloseCaptions
{
    public class ClosedCaptionWrapperClass
    {
        private ClosedCaption Parent;

        public ClosedCaptionWrapperClass(ClosedCaption c)
        {
            Parent = c;
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
                    return v.LineText+" Original: "+Parent.Definition;
                return Parent.Definition;
            }
        }
    }
}
