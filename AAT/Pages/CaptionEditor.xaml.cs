using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AAT.Addons;
using AAT.Classes.CloseCaptions;
using AAT.Windows;
using AAT.Soundevents;

namespace AAT.Pages
{
    /// <summary>
    /// Interaction logic for CaptionEditor.xaml
    /// </summary>
    public partial class CaptionEditor : Page
    {
        private static CaptionEditor m_instance;
        public static CaptionEditor Instance
        {
            get
            {
                if (m_instance == null) m_instance = new CaptionEditor();
                return m_instance;
            }
        }
        private static Cheatsheet cheatsheet;
        public CaptionEditor()
        {
            InitializeComponent();
            Loaded += CaptionEditor_Loaded;
            CloseCaptionManager.LanguageChanged += languageChanged;
        }


        private void languageChanged()
        {
            SetSource();
        }
        private void CaptionEditor_Loaded(object sender, RoutedEventArgs e)
        {
            AddonManager.AddonChanged += addonChanged;
            CloseCaptionManager.LoadCaptions();
            MainWindow.ChangeTheme(Instance);
        }
        public void SetSource()
        {
            CaptionEditorView.ItemsSource = CloseCaptionManager.LanguageSpecificCaptions.CaptionStruct;
            CustomCaptions.ItemsSource = CloseCaptionManager.AddonSpecificCaptions.Values;
            CustomCaptions.SelectedItem = CloseCaptionManager.AddonSpecificCaptions[CloseCaptionManager.CurrLang];
            CaptionCount.Text = $"Count: {CaptionEditorView.Items.Count}";


        }
        private void addonChanged()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cheatsheet_Click(object sender, RoutedEventArgs e)
        {
            cheatsheet = Windows.Cheatsheet.Instance;
            if (cheatsheet != null)
            {
                cheatsheet.Show();
                cheatsheet.Focus();
            } 
        }

        private void CustomCaptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageCaption LC = (LanguageCaption)CustomCaptions.SelectedItem;
            if (LC == null) return;

            CloseCaptionManager.CurrLang = LC.language;
            
        }
    }
}
