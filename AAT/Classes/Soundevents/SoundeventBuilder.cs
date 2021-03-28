using AAT.Addons;
using AAT.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLACaptionReplacer;
using AAT.CloseCaptions;

namespace AAT.Soundevents
{
    public enum ErrorCodes
    {
        UNKOWN,
        OK,
        INVALID,
        DUPLICATE,
        NOTFOUND,
        EMPTY
    }
    public enum PropertyNames
    {
        type,
        volume,
        priority,
        mixgroup,
        volume_fade_out,
        volume_fade_in,
        vsnd_files,
        use_hrtf
    }
    class SoundeventBuilder
    {

        private static SoundeventBuilder m_instance;
        public static SoundeventBuilder Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new SoundeventBuilder();
                return m_instance;
            }
            set { m_instance = value; }
        }

        public ObservableCollection<Soundevent> AllSoundEvents = new ObservableCollection<Soundevent>();
        public ObservableCollection<Soundevent> AllBaseSoundEvents = new ObservableCollection<Soundevent>();
        public ObservableCollection<Soundevent> AddonBasedEvents { get => AddonManager.CurrentAddon.AllSoundevents; }
        public ObservableCollection<SoundeventProperty> properties = new();


        public static Dictionary<uint, Soundevent> SoundeventDictionary = new();
        public static Dictionary<string, LanguageCaption> CaptionDictionary = new();


        enum options
        {
            onlyAddon,
            onlyBase
        }

        public SoundeventBuilder()
        {
            FillBaseGameEventsList();

            //AddonBasedEvents = AllSoundEvents;
            Editor.Instance.BaseSoundeventName.ItemsSource = AllBaseSoundEvents;
            Editor.Instance.SoundeventName.ItemsSource = AddonBasedEvents;
        }

        private async void FillBaseGameEventsList()
        {
            await alyxJsonParser.GetAlyxSoundeventFromGame();
            foreach (var item in alyxJsonParser.allsoundeventsfromGame)
            {
                AllSoundEvents.Add(item);
                AllBaseSoundEvents.Add(item);
                SoundeventDictionary[item.Hash] = item;
            }
        }
        public void SwitchOnlyAddons(bool state)
        {
            if(state)
            Editor.Instance.SetFiltered((e) => { return (e.Addon != null && e.Addon.AddonName.Equals(AddonManager.CurrentAddon.AddonName)); });
            else
            {
                Editor.Instance.SetFiltered((e)=> { return true; });
            }

        }
        public void SwitchOnlyBase(bool state)
        {
            if (state)
                Editor.Instance.SetFiltered((e) => { return e.Addon == null; });
            else
            {
                Editor.Instance.SetFiltered((e) => { return true; });
            }
        }


        public ErrorCodes CreateNewEvent(string name, string basevent = null)
        {
            Soundevent se = null;
            if (string.IsNullOrEmpty(name)) return ErrorCodes.EMPTY;
            if (string.IsNullOrWhiteSpace(name)) return ErrorCodes.EMPTY;
            foreach (var item in AddonBasedEvents)
            {
                if (item.EventName.Equals(name))
                {
                    return ErrorCodes.DUPLICATE;
                }
            }
            if (basevent != null)
            {
                se = new Soundevent(name,AddonManager.CurrentAddon, basevent);
                se.AddProperty(new SoundeventProperty("base", EventDisplays.SoundeventPicker,basevent));
                se.AddProperty(new SoundeventProperty(PropertyNames.volume.ToString(), EventDisplays.FloatValue, "1"));
            }
            else
            {
                se = new Soundevent(name, AddonManager.CurrentAddon);
                se.AddProperty(new SoundeventProperty(PropertyNames.type.ToString(), EventDisplays.TypePicker));
                se.AddProperty(new SoundeventProperty(PropertyNames.volume.ToString(), EventDisplays.FloatValue, "1"));
            }
            AddonBasedEvents.Add(se);
            AddonManager.CurrentAddon.Manifest.AddonSoundeventDictionary[se.Hash] = se;
            Editor.Instance.SoundeventName.SelectedItem = se;
            return ErrorCodes.OK;
        }
        public ErrorCodes AddNewPropertyToEvent(Soundevent Event, PropertyNames propertyName)
        {
            if (SoundeventsPropertyDefinitions.TypeDictionary.TryGetValue(propertyName.ToString(),out SoundeventsPropertyDefinitions.EventTypeStruct t))
            {
                return Event.AddProperty(new SoundeventProperty(propertyName, t.Type));

            }
            return Event.AddProperty(new SoundeventProperty(propertyName.ToString()));

        }
        public ErrorCodes AddCustomPropertyToEvent(Soundevent Event,  string typeName, Type types)
        {

            return Event.AddProperty(new SoundeventProperty(typeName, types));
        }
        public ErrorCodes AddCustomPropertyToEvent(Soundevent Event, string type = "", object Val = null)
        {

            return Event.AddProperty(new SoundeventProperty(type.Trim(), Val));
        }
        public ErrorCodes AddCustomPropertyToEvent(Soundevent Event, SoundeventsPropertyDefinitions.EventTypeStruct TS)
        {
            return Event.AddProperty(new SoundeventProperty(TS));
        }


        public void ShowPropertiesOfSoundevent(Soundevent se)
        {
            properties.Clear();
            foreach (var item in se.Properties)
            {
                properties.Add(item);
            }
        }
    }
}