using AAT.Classes;
using AAT.Classes.Addons;
using AAT.Classes.SoundEventPropertyClasses;
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

namespace AAT.Pages
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Page
    {
        public static Editor Instance = new Editor();
        SoundeventBuilder builder;
        ErrorCodes code = ErrorCodes.OK;

        public ObservableCollection<SoundeventProperty> properties = new ObservableCollection<SoundeventProperty>();

        public ObservableCollection<string> ValueTypes { get => vt; }
        private ObservableCollection<string> vt = new ObservableCollection<string>();
        public ObservableCollection<Addon> Addons { get => addons;}
        private ObservableCollection<Addon> addons = new ObservableCollection<Addon>();


        public Editor()
        {
            InitializeComponent();
            Loaded += Editor_Loaded;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            builder = SoundeventBuilder.Instance;
            onlyBase.Toggled += OnlyBase_Toggled;
            onlyAddon.Toggled += OnlyAddon_Toggled;
            soundeventEditorView.ItemsSource = builder.properties;
            properties = builder.properties;
            ComboBoxAddItem.ItemsSource = Enum.GetNames(typeof(PropertyNames));
            vt = new ObservableCollection<string>
            {
                "Float",
                "Array",
                "Eventpicker",
                "Comment"
            };
            addons = new ObservableCollection<Addon>(AddonManager.GetAddons());
            if(Properties.Settings.Default.LastSelectedAddon == "None")
            {
                AddonSelectionBox.SelectedIndex = 0;
            }
            else
            {
                AddonSelectionBox.SelectedIndex = addons.IndexOf(addons.Where((e) => { return e.AddonName == Properties.Settings.Default.LastSelectedAddon; }).First());

            }
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

        private void BaseEvent_TouchDown(object sender, TouchEventArgs e)
        {

        }

        private void SoundEvenName_TouchDown(object sender, TouchEventArgs e)
        {

        }
        private async void CreateMessageDialog(ErrorCodes code = ErrorCodes.OK, string text = "", string body = "")
        {
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
                        await MainWindow.Instance.ShowMessageAsync("Invalid.\nHappens to the best of us", code.ToString());
                        break;
                    case ErrorCodes.DUPLICATE:
                        await MainWindow.Instance.ShowMessageAsync("Duplicate found.\nPlease make sure the Item doesn't already Exist", code.ToString());
                        break;
                    case ErrorCodes.NOTFOUND:
                        await MainWindow.Instance.ShowMessageAsync("Item not Found.\nWhere has it gone?", code.ToString());
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

        private void ComboBoxAddItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            Soundevent se = (Soundevent)SoundeventName.SelectedItem;
            if (se != null && cb.SelectedItem != null)
            {
                CreateMessageDialog(builder.AddCustomPropertyToEvent(se, cb.SelectedItem.ToString()));
                builder.ShowPropertiesOfSoundevent(se);
            }
        }

        private void ComboBoxAddItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ComboBox cb = (ComboBox)sender;
                Soundevent se = (Soundevent)SoundeventName.SelectedItem;
                if (se != null && cb.Text != "")
                {
                    CreateMessageDialog(builder.AddCustomPropertyToEvent(se, cb.Text));
                    builder.ShowPropertiesOfSoundevent(se);
                }

            }
        }

        private void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox b = (ComboBox)sender;
            Debug.Print(b.Text);
        }

        private void AddonSelectionBox_Initialized(object sender, EventArgs e)
        {
            AddonSelectionBox.ItemsSource = Addons;
        }
    }
    public class DataTemplateBasedOnValue : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item != null)
            {

                SoundeventProperty soundeventProperty = item as SoundeventProperty;
                switch (soundeventProperty.DisAs)
                {
                    case EventDisplays.FloatValue:
                        return element.FindResource("FloatTemplate") as DataTemplate;
                    case EventDisplays.SoundeventPicker:
                        return element.FindResource("EventPicker") as DataTemplate;
                    case EventDisplays.ArrayValue:
                        return element.FindResource("TextTemplate") as DataTemplate;

                    case EventDisplays.StringValue:
                        return element.FindResource("TextTemplate") as DataTemplate;
                    default:
                        return element.FindResource("TextTemplate") as DataTemplate;

                }

            }
            return base.SelectTemplate(item, container);
        }
    }
    
}
