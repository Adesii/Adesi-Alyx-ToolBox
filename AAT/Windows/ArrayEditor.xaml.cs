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

namespace AAT.Windows
{
    /// <summary>
    /// Interaction logic for ArrayEditor.xaml
    /// </summary>
    public partial class ArrayEditor : MetroWindow
    {

        private static ArrayEditor m_instance;

        public event PropertyChangedEventHandler PropertyChanged;
        public SoundeventProperty CurrentArrayProperty;

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
            InitializeComponent();
            Loaded += CaptionTypeEditor_Loaded;
            Closed += MetroWindow_Closed;
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
    }
}
