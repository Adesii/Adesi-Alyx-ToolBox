using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AAT.Soundevents;
using System.Collections.ObjectModel;
using ValveResourceFormat.Serialization.KeyValues;
using System.Globalization;

namespace AAT.Windows
{
    /// <summary>
    /// Interaction logic for ArrayEditor.xaml
    /// </summary>
    public partial class ArrayEditor : MetroWindow
    {

        private static ArrayEditor m_instance;

        public event PropertyChangedEventHandler PropertyChanged;

        private SoundeventProperty _currentArrayProperty;
        public SoundeventProperty CurrentArrayProperty
        {
            get
            {
                return _currentArrayProperty;
            }
            set
            {
                _currentArrayProperty = value;
                NotifyPropertyChanged(nameof(CurrentArrayProperty));
            }
        }

        private ObservableCollection<AKV.AKValue> kVValues;
        public ObservableCollection<AKV.AKValue> KVCollection
        {
            get
            {
                if (kVValues == null) kVValues = new();
                return kVValues;
            }
        }

        public static ArrayEditor Instance
        {
            get
            {
                if (m_instance == null) m_instance = new ArrayEditor();
                return m_instance;
            }
        }
        public ArrayEditor()
        {
            PropertyChanged += CurrentProperty_PropertyChanged;
            InitializeComponent();
            Loaded += CaptionTypeEditor_Loaded;
            Closed += MetroWindow_Closed;
        }

        private void CurrentProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Property Changed");
            KVCollection.Clear();
            if (CurrentArrayProperty.Value is List<AKV.AKValue> kc)
                foreach (var item in kc)
                {
                    KVCollection.Add(item);
                }
        }

        private void CaptionTypeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeTheme(Instance);

            ThemeManager.Current.ThemeChanged += Current_ThemeChanged;
        }
        private void Current_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            MainWindow.ChangeTheme(Instance);

        }
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            m_instance = null;
        }
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class KVObjectBasedOnValue : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && item != null)
            {
                System.Diagnostics.Debug.WriteLine(item.GetType());
                if (item is AKV.AKValue)
                {
                    return element.FindResource("ArrayTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }

    }

}
