using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using HLACaptionCompiler;
using AAT.CloseCaptions;
using System.ComponentModel;

namespace AAT.Windows
{
    /// <summary>
    /// Interaction logic for CaptionTypeEditor.xaml
    /// </summary>
    public partial class CaptionTypeEditor : MetroWindow,INotifyPropertyChanged
    {
        private ClosedCaptionWrapperClass m_currCaption;
        public ClosedCaptionWrapperClass CurrentCaption
        {
            get => m_currCaption;
            set 
            {

                m_currCaption = value;
                //m_currCaption.PropertyChanged += M_currCaption_PropertyChanged;
                NotifyPropertyChanged(nameof(CurrentCaption));
                EditingTextBox.Text = m_currCaption.GetText;
                
            }
        }

        private void M_currCaption_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Property Was changed");
            NotifyPropertyChanged(nameof(CurrentCaption));
        }

        private static CaptionTypeEditor m_instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static CaptionTypeEditor Instance
        {
            get
            {
                if (m_instance == null) m_instance = new CaptionTypeEditor();
                return m_instance;
            }
        }

        public CaptionTypeEditor()
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
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox ts = sender as TextBox;
            CurrentCaption.GetText = ts.Text;
        }
    }
}
