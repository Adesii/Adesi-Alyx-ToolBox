using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.Serialization.KeyValues;

namespace AAT.Soundevents
{
    public class Soundevent
    {
        public Soundevent(string SoundeventName, Addons.Addon ownAddon = null, string baseEvent = null, string FileName = "base")
        {
            EventName = SoundeventName.Trim().ToLower().Replace(" ","");
            Hash = ValveResourceFormat.Crc32.Compute(System.Text.Encoding.UTF8.GetBytes(EventName));
            BaseEvent = baseEvent;
            Addon = ownAddon;
            this.FileName = FileName;
            if (ownAddon != null)
            {
                ownAddon.AddNewSoundEvent(this);
            }
        }

        public uint Hash { get; set; }
        public string EventName { get; }

        private string meta = "";
        public string GetMeta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(meta)) {
                    var b = (KVObject)GetProperty("metadata")?.Value;
                    meta = b==null? "":b.Properties["1"].Value.ToString();
                }
                return meta ?? "";
            }
        }

        public string LineText
        {
            get
            {

                var t = GetProperty("line_text");
                if (t != null)
                    return t.Value.ToString();
                else return Caption.Definition;
            }
        }
        public string FileName { get; private set; }
        public string BaseEvent { get; }
        public HLACaptionReplacer.ClosedCaption Caption { get; set; }
        public Addons.Addon Addon { get; }
        public List<SoundeventProperty> Properties { get; set; } = new List<SoundeventProperty>();

        public SoundeventProperty GetProperty(SoundeventProperty property)
        {
            foreach (var item in Properties)
            {
                if (item.TypeName.Equals(property.TypeName))
                {
                    return item;
                }
            }
            return null;
        }
        public SoundeventProperty GetProperty(string propertyName)
        {
            foreach (var item in Properties)
            {
                if (item.TypeName.Equals(propertyName))
                {
                    return item;
                }
            }
            return null;
        }

        public ErrorCodes AddProperty(SoundeventProperty property)
        {
            foreach (var item in Properties)
            {
                if (item.TypeName.Equals(property.TypeName))
                {
                    return ErrorCodes.DUPLICATE;
                }
            }
            Properties.Add(property);
            return ErrorCodes.OK;
        }
        public ErrorCodes RemoveProperty(string type)
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                SoundeventProperty item = Properties[i];
                if (item.TypeName == type)
                {
                    Properties.RemoveAt(i);
                    return ErrorCodes.OK;
                }
            }
            return ErrorCodes.NOTFOUND;
        }

        public override string ToString()
        {
            return EventName.ToString();
        }
    }
}
