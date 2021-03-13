using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAT.Soundevents
{
    public class Soundevent
    {
        public Soundevent(string SoundeventName, Addons.Addon ownAddon = null, string baseEvent = null,string FileName = "base")
        {
            EventName = SoundeventName;
            BaseEvent = baseEvent;
            Addon = ownAddon;
            this.FileName = FileName;
            if(ownAddon != null)
            {
                ownAddon.AddNewSoundEvent(this);
            }
        }

        public string EventName { get; }
        public string FileName { get; private set; }
        public string BaseEvent { get; }
        public Addons.Addon Addon { get; }
        public List<SoundeventProperty> Properties { get; } = new List<SoundeventProperty>();

        public SoundeventProperty GetProperty(SoundeventProperty property)
        {
            foreach (var item in Properties)
            {
                if (item.TypeName == property.TypeName)
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
