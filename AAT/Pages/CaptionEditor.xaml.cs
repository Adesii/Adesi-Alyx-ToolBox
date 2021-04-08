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
using AAT.CloseCaptions;
using AAT.Windows;
using AAT.Soundevents;
using System.Linq;
using System.Diagnostics;
using HLACaptionCompiler;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

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

        bool CurrentlyViewable = false;

        private static Cheatsheet cheatsheet;
        private static CaptionTypeEditor captionEditing;
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
                
            MainWindow.ChangeTheme(Instance);
            CurrentlyViewable = true;

            //List<string> compiled = new();
            //ITraceWriter traceWriter = new MemoryTraceWriter();


            //Debug.WriteLine(traceWriter.ToString());
            //System.IO.File.WriteAllTextAsync("C:/Users/kopie/Desktop/New folder/allCaption.Json", JsonConvert.SerializeObject(SoundeventBuilder.CaptionDictionary, new JsonSerializerSettings
            //{
            //    TraceWriter = traceWriter,
            //    MaxDepth = 1,
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    Error = delegate (object sender, ErrorEventArgs args)
            //    {
            //        args.ErrorContext.Handled = true;
            //    }
            //}
            //        ));
        }
        public void SetSource()
        {
            CaptionEditorView.ItemsSource = CloseCaptionManager.LanguageSpecificCaptions.GetCaptions;
            CustomCaptions.ItemsSource = Addon.AvailableCloseCaptions.Values;
            CustomCaptions.SelectedItem = Addon.AvailableCloseCaptions[CloseCaptionManager.CurrLang];
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!CurrentlyViewable) return;
            captionEditing = Windows.CaptionTypeEditor.Instance;
            captionEditing.Owner = MainWindow.Instance;
            Debug.WriteLine(sender.GetType());
            if (sender.GetType() == typeof(DataGrid))
            {
                var grid = sender as DataGrid;
                if (grid.SelectedItem == null)
                {
                    return;
                }
                else
                {
                    CaptionTypeEditor.Instance.CurrentCaption = grid.SelectedItem as ClosedCaptionWrapperClass;
                }
            }
            if (captionEditing != null)
            {
                captionEditing.Show();
                captionEditing.Focus();
            }
        }

        private void CaptionEditorView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!CurrentlyViewable) return;
            captionEditing = Windows.CaptionTypeEditor.Instance;
            captionEditing.Owner = MainWindow.Instance;
            Debug.WriteLine(sender.GetType());
            if (sender.GetType() == typeof(DataGrid))
            {
                var grid = sender as DataGrid;
                if (grid.SelectedItem == null)
                {
                    return;
                }
                else
                {
                    CaptionTypeEditor.Instance.CurrentCaption = grid.SelectedItem as ClosedCaptionWrapperClass;
                }
            }
            if (captionEditing != null)
            {
                captionEditing.Show();
                captionEditing.Focus();
            }
        }
    }
}
