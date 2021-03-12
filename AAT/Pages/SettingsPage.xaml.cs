using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using ValveKeyValue;

namespace AAT.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public static SettingsPage Instance = new SettingsPage();
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

            Path.Text = GetInstallPath();
            AutoCompile.IsChecked = Properties.Settings.Default.AutoCompile;
            string markdown = Properties.Resources.aboutText;
            //var xaml = Markdown.ToFlowDocument(markdown);

            AboutPage.Markdown = markdown;
            MainWindow.changeTheme(Properties.Settings.Default.DarkMode);

            
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
        private string GetInstallPath() 
        {
            var c = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", "InstallPath", null);
            if (c == null) return Properties.Settings.Default.InstallPath;
            var path = GetAlyxPath(c.ToString());
            Properties.Settings.Default.InstallPath = path;
            return path;
        }

        private string GetAlyxPath(string SteamPath)
        {
            List<string> paths = new List<string>();
            if (Directory.Exists(SteamPath))
            {
                paths.Add(SteamPath);
                if (File.Exists(SteamPath + "\\steamapps\\libraryfolders.vdf"))
                {
                    var allLibraries=File.ReadAllText(SteamPath + "\\steamapps\\libraryfolders.vdf");
                    var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
                    var kvalues =kv.Deserialize(File.OpenRead(SteamPath + "\\steamapps\\libraryfolders.vdf"));

                    foreach (var item in kvalues)
                    {
                        if (!int.TryParse(item.Name, out int num)) continue;
                        Debug.Print("--------Item---------");
                        Debug.Print(item.Name);
                        paths.Add(item.Value.ToString());
                    }
                }
            }
            foreach (var item in paths)
            {
                if (Directory.Exists(item + "\\steamapps\\common\\Half-Life Alyx\\game\\hlvr"))
                return item.Replace("\\","/",System.StringComparison.Ordinal)+ "/steamapps/common/Half-Life Alyx";
            }

            return null;
        }
        private void AutoCompile_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoCompile = (bool)AutoCompile.IsChecked;
        }
    }
}
