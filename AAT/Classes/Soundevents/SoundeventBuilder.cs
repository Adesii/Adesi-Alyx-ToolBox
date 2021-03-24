using AAT.Addons;
using AAT.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ObservableCollection<Soundevent> filtered = new ObservableCollection<Soundevent>();
        public ObservableCollection<Soundevent> AddonBasedEvents { get => AddonManager.CurrentAddon.AllSoundevents; }
        public ObservableCollection<SoundeventProperty> properties = new ObservableCollection<SoundeventProperty>();


        private Dictionary<options, bool> lastButtonState = new Dictionary<options, bool>();
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
            }
        }
        public void SwitchOnlyAddons(bool state)
        {
            lastButtonState[options.onlyBase] = state;

            if (state)
            {
                filtered = GetAllSounds();
                IEnumerable<Soundevent> query = filtered.Where((a) => a.Addon == AddonManager.CurrentAddon);
                filtered.Clear();
                foreach (var item in query)
                {
                    filtered.Add(item);
                }
                Pages.Editor.Instance.BaseSoundeventName.ItemsSource = filtered;
                lastButtonState[options.onlyBase] = state;
            }
            else
            {
                if (noFilters()) Pages.Editor.Instance.BaseSoundeventName.ItemsSource = GetAllSounds();
            }

        }
        public void SwitchOnlyBase(bool state)
        {
            lastButtonState[options.onlyBase] = state;
            if (state)
            {
                filtered = GetAllSounds();
                IEnumerable<Soundevent> query = filtered.Where((a) => a.BaseEvent != "" || a.BaseEvent != null);
                filtered.Clear();
                foreach (var item in query)
                {
                    filtered.Add(item);
                }

                Pages.Editor.Instance.BaseSoundeventName.ItemsSource = filtered;
            }
            else
            {
                if (noFilters()) Pages.Editor.Instance.BaseSoundeventName.ItemsSource = GetAllSounds();
            }
        }
        private bool noFilters()
        {
            int i = 0;
            foreach (var item in lastButtonState)
            {
                if (item.Value) i++;
            }
            return i <= 0;
        }
        private ObservableCollection<Soundevent> GetAllSounds()
        {
            ObservableCollection<Soundevent> onion = new ObservableCollection<Soundevent>();
            var res = new ObservableCollection<Soundevent>(AllSoundEvents.Concat(AddonBasedEvents));
            foreach (var item in res)
            {
                onion.Add(item);
            }

            return onion;
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
                se.AddProperty(new SoundeventProperty("base", EventDisplays.SoundeventPicker));
                se.AddProperty(new SoundeventProperty(PropertyNames.volume.ToString(), EventDisplays.FloatValue, "1"));
            }
            else
            {
                se = new Soundevent(name, AddonManager.CurrentAddon);
                se.AddProperty(new SoundeventProperty(PropertyNames.type.ToString(), EventDisplays.TypePicker));
                se.AddProperty(new SoundeventProperty(PropertyNames.volume.ToString(), EventDisplays.FloatValue, "1"));
            }
            AddonBasedEvents.Add(se);
            Editor.Instance.SoundeventName.SelectedItem = se;
            filtered = GetAllSounds();
            return ErrorCodes.OK;
        }
        public ErrorCodes AddNewPropertyToEvent(Soundevent Event, PropertyNames propertyName)
        {
            if (SoundeventsPropertyDefinitions.typeDictionary.TryGetValue(propertyName.ToString(), out Type t))
            {
                return Event.AddProperty(new SoundeventProperty(propertyName, t));

            }
            return Event.AddProperty(new SoundeventProperty(propertyName.ToString()));

        }
        public ErrorCodes AddCustomPropertyToEvent(Soundevent Event, Type types, string typeName)
        {
            return Event.AddProperty(new SoundeventProperty(typeName, types));
        }
        public ErrorCodes AddCustomPropertyToEvent(Soundevent Event, string type = "", string Val = "")
        {

            return Event.AddProperty(new SoundeventProperty(type.Trim(), Val.Trim()));
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