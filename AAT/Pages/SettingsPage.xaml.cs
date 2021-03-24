using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ControlzEx.Theming;
using Microsoft.Win32;
using ValveKeyValue;

namespace AAT.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private static SettingsPage m_instance;
        public static SettingsPage Instance
        {
            get
            {
                if (m_instance == null) m_instance = new SettingsPage();
                return m_instance;
            }
        }
        public SettingsPage()
        {
            InitializeComponent();
            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            NavigationService.Navigating += NavigationService_Navigating;

            if (Properties.Settings.Default.DarkMode)
                ThemeSelector.Content = "Dark";
            else
                ThemeSelector.Content = "Light";

            Path.Text = Addons.AddonFolderWatcher.GetInstallPath();
            AutoCompile.IsChecked = Properties.Settings.Default.AutoCompile;
            string markdown = Properties.Resources.aboutText;
            //var xaml = Markdown.ToFlowDocument(markdown);

            AboutPage.Markdown = markdown;
            MainWindow.changeTheme(Properties.Settings.Default.DarkMode);
            MainWindow.ChangeTheme(Instance);
              
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
                e.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(e.Parameter.ToString());
        }




        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.changeTheme(true);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow.changeTheme(false);
        }

        private void fileSelection_Click(object sender, RoutedEventArgs e)
        {
            /*
            var fileContent = string.Empty;
            var filePath = string.Empty;
            var ppath = string.Empty;
            FileInfo file = null;

            using (var openFileDialog = new CommonOpenFileDialog() { IsFolderPicker = true })
            {
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Select the Half life Alyx Directory";

                if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Path.Text = openFileDialog.FileName;
                    Properties.Settings.Default.InstallPath = Path.Text;
                }

            }
            */
        }
        private void AutoCompile_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoCompile = (bool)AutoCompile.IsChecked;
        }
    }
}
