﻿using AAT.Addons;
using AAT.Soundevents;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using static AAT.Soundevents.SoundeventsPropertyDefinitions;
using System.Collections;

namespace AAT.Pages
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Page
    {
        private static Editor m_instance;
        public static Editor Instance
        {
            get
            {
                if (m_instance == null) m_instance = new Editor();
                return m_instance;
            }
        }
        SoundeventBuilder builder;
        ErrorCodes code = ErrorCodes.OK;

        public ObservableCollection<SoundeventProperty> properties = new();

        public ObservableCollection<Soundevent> SoundeventList => builder.AllSoundEvents;

        public ObservableCollection<string> ValueTypes { get => vt; }
        private ObservableCollection<string> vt = new();

        public static SortedDictionary<string, EventTypeStruct> TypeDictionary => SoundeventsPropertyDefinitions.TypeDictionary;

        public IEnumerable Stuff => ComboBoxAddItem.ItemsSource;


        public Editor()
        {
            InitializeComponent();
            Loaded += Editor_Loaded;
            Unloaded += Editor_Unloaded;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            builder = SoundeventBuilder.Instance;
            onlyBase.Toggled += OnlyBase_Toggled;
            onlyAddon.Toggled += OnlyAddon_Toggled;
            soundeventEditorView.ItemsSource = builder.properties;
            properties = builder.properties;
            vt = new ObservableCollection<string>
            {
                "Float",
                "Array",
                "Eventpicker",
                "Comment"
            };


            AddonManager.AddonChanged += addonChanged;
            MainWindow.ChangeTheme(Instance);

            addonChanged();
        }
        private void Editor_Unloaded(object sender, RoutedEventArgs e)
        {
            AddonManager.AddonChanged -= addonChanged;
        }
        private void addonChanged()
        {
            SoundeventName.ItemsSource = AddonManager.CurrentAddon.AllSoundevents;
        }

        private void OnlyAddon_Toggled(object sender, RoutedEventArgs e)
        {
            fixSwitches((ToggleSwitch)sender);

            builder.SwitchOnlyAddons(((ToggleSwitch)sender).IsOn);
        }

        private void OnlyBase_Toggled(object sender, RoutedEventArgs e)
        {
            fixSwitches((ToggleSwitch)sender);
            builder.SwitchOnlyBase(((ToggleSwitch)sender).IsOn);
        }

        private void fixSwitches(ToggleSwitch ob)
        {
            if (ob.IsOn)
            {


                foreach (ToggleSwitch item in BaseEventSelectionOptions.Items)
                {
                    if (item != ob)
                    {
                        item.IsOn = false;
                    }
                }
            }
        }

        public void SetFiltered(Func<Soundevent, bool> predicate)
        {
            ObservableCollection<Soundevent> b = new(SoundeventBuilder.Instance.AllBaseSoundEvents.Concat(SoundeventBuilder.Instance.AddonBasedEvents));
            BaseSoundeventName.ItemsSource = new ObservableCollection<Soundevent>(b.Where(predicate));
        }

        private async void CreateMessageDialog(ErrorCodes code = ErrorCodes.OK, string text = "", string body = "")
        {
            Debug.Print("MessageDialog Received");
            if (text != "")
            {
                await MainWindow.Instance.ShowMessageAsync(text, body);
            }
            else
            {
                switch (code)
                {
                    case ErrorCodes.UNKOWN:
                        await MainWindow.Instance.ShowMessageAsync("Unknow Error occured....\nDidn't know this could happen \n :(", code.ToString());
                        break;
                    case ErrorCodes.OK:
                        break;
                    case ErrorCodes.INVALID:
                        await MainWindow.Instance.ShowMessageAsync("Invalid.", "Happens to the best of us\n" + code.ToString());
                        break;
                    case ErrorCodes.DUPLICATE:
                        await MainWindow.Instance.ShowMessageAsync("Duplicate found.", "Please make sure the Item doesn't already Exist\n" + code.ToString());
                        break;
                    case ErrorCodes.NOTFOUND:
                        await MainWindow.Instance.ShowMessageAsync("Item not Found.", "Where has it gone ?\n" + code.ToString());
                        break;
                    case ErrorCodes.EMPTY:
                        await MainWindow.Instance.ShowMessageAsync("Input can't be Empty.", "Type something in and Try again :)\n" + code.ToString());
                        break;
                    default:
                        break;
                }
            }


        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BaseSoundeventName.Text.Equals(""))
            {

                code = builder.CreateNewEvent(SoundeventName.Text);
            }
            else
            {
                code = builder.CreateNewEvent(SoundeventName.Text, BaseSoundeventName.Text);
            }

            CreateMessageDialog(code, "", "");
        }

        private void SoundeventName_Selected(object sender, RoutedEventArgs e)
        {
            Soundevent se = (Soundevent)SoundeventName.SelectedItem;

            if (se != null)
            {
                builder.ShowPropertiesOfSoundevent(se);
            }
            else
            {
                builder.properties.Clear();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ((Soundevent)SoundeventName.SelectedItem).RemoveProperty(((SoundeventProperty)soundeventEditorView.SelectedItem).TypeName);
            builder.ShowPropertiesOfSoundevent((Soundevent)SoundeventName.SelectedItem);

        }

        private void ComboBoxAddItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ComboBox cb = (ComboBox)sender;
                Soundevent se = (Soundevent)SoundeventName.SelectedItem;
                if (se != null && cb.Text != "")
                {
                    if (cb.SelectedItem != null)
                    {
                        KeyValuePair<string, EventTypeStruct> kvp = (KeyValuePair<string, EventTypeStruct>)cb.SelectedItem;
                        CreateMessageDialog(builder.AddCustomPropertyToEvent(se, kvp.Key, kvp.Value.Type));
                    }
                    else
                    {
                        if (SoundeventsPropertyDefinitions.TypeDictionary.TryGetValue(cb.Text, out EventTypeStruct v))

                            CreateMessageDialog(builder.AddCustomPropertyToEvent(se, v));
                        else
                            CreateMessageDialog(builder.AddCustomPropertyToEvent(se, cb.Text));
                    }


                    builder.ShowPropertiesOfSoundevent(se);
                }

            }
        }


        private void ComboBoxAddItem_Loaded(object sender, EventArgs e)
        {
            ComboBoxAddItem.ItemsSource = SoundeventsPropertyDefinitions.TypeDictionary;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ComboBoxAddItem.ItemsSource);
            PropertyGroupDescription descriptor = new("Value.Group");
            view.GroupDescriptions.Add(descriptor);
        }

        private void EventPicker_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox s = sender as ComboBox;
            var name = s.TryFindParent<DataGridRow>()?.Item as SoundeventProperty;
            //Debug.WriteLine(name?.Value);
            try
            {
                s.SelectedItem = builder.AllSoundEvents.Single((e) => { return e.EventName.Equals(name.Value); });
            }
            catch (Exception)
            {

            }


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddonManager.CurrentAddon.ApplyChanges();
        }

        private void EventPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox s = sender as ComboBox;
            var name = s.TryFindParent<DataGridRow>()?.Item as SoundeventProperty;
            //Debug.WriteLine(name?.Value);
            name.Value = s.SelectedItem;
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox s = sender as ComboBox;
            var name = s.TryFindParent<DataGridRow>()?.Item as SoundeventProperty;
            //Debug.WriteLine(name?.Value);
            if(s.SelectedItem != null)
            {
                Debug.WriteLine(s.SelectedItem);
                name.Type = ((KeyValuePair<string, EventTypeStruct>)s.SelectedItem).Value.Type;
            }
            else
            {
                name.Type = typeof(string);
            }
            
        }
    }
    public class DataTemplateBasedOnValue : DataTemplateSelector
    {

        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            
            if (container is FrameworkElement element && item != null)
            {

                SoundeventProperty soundeventProperty = item as SoundeventProperty;
                soundeventProperty.ValueContainer = container;
                DataTemplate Template;
                //Debug.WriteLine($"Soundevent Property:{soundeventProperty.Value?.GetType()} wants to display as {soundeventProperty.DisAs}");
                switch (soundeventProperty.DisAs)
                {
                    case EventDisplays.FloatValue:
                        Template = element.FindResource("FloatTemplate") as DataTemplate;
                        break;
                    case EventDisplays.SoundeventPicker:
                        Template = element.FindResource("EventPicker") as DataTemplate;
                        break;
                    case EventDisplays.ArrayValue:
                        Template = element.FindResource("ArrayTemplate") as DataTemplate;
                        break;
                    case EventDisplays.StringValue:
                        Template = element.FindResource("TextTemplate") as DataTemplate;
                        break;
                    case EventDisplays.FilePicker:
                        Template = element.FindResource("FilePicker") as DataTemplate;
                        break;
                    default:
                        Template = element.FindResource("TextTemplate") as DataTemplate;
                        break;
                }
                return Template;
            }
            return base.SelectTemplate(item, container);
        }

        private void SoundeventProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
