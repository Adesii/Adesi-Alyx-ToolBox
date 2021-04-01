using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AAT.Pages;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps;
using AAT.Addons;

namespace AAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public const string Theme = "Steel";

        public static MainWindow Instance;

        public static Brush BackgroundLightMode;
        public static Brush BackgroundDarkMode;
        static List<object> classesToUpdateTheme = new();
        public MainWindow()
        {
            InitializeComponent();
            
            MainFrame.Content = SettingsPage.Instance;
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.LastSelectedAddon = AddonManager.CurrentAddon.AddonName;
            Properties.Settings.Default.Save();
            //Windows.Cheatsheet.Instance?.Close();
            //App.Current.Shutdown(0);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            transitionThing.Content = null;
            transitionThing.Content = MainFrame;
            BackgroundDarkMode = (Brush)Instance.FindResource("MahApps.Brushes.MenuItem.Background");
            BackgroundLightMode = (Brush)Instance.FindResource("MahApps.Brushes.Gray");
            changeTheme(Properties.Settings.Default.DarkMode);

            this.OverlayFadeIn = (System.Windows.Media.Animation.Storyboard)this.FindResource("fadeIn");
            this.OverlayFadeOut = (System.Windows.Media.Animation.Storyboard)this.FindResource("fadeOut");

            if (Properties.Settings.Default.LastSelectedAddon == "None")
            {
                AddonSelectionBox.SelectedIndex = 0;
            }
            else
            {
                AddonSelectionBox.SelectedIndex = AddonManager.Addons.IndexOf(AddonManager.Addons.Where((e) => { return e.AddonName == Properties.Settings.Default.LastSelectedAddon; }).First());

            }
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            _ = Soundevents.SoundeventBuilder.Instance;
            
        }

        

        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Pages.SettingsPage.Instance;
        }

        private void EditorMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Pages.Editor.Instance;
        }
        private void CaptionClick(object sender,RoutedEventArgs e)
        {
            MainFrame.Content = Pages.CaptionEditor.Instance;
        }

        public static void changeTheme(bool darkmode)
        {
            if (darkmode)
            {
                ThemeManager.Current.ChangeTheme(Instance, "Dark." + Theme);

                BackgroundDarkMode = (Brush)Instance.FindResource("MahApps.Brushes.MenuItem.Background");
                BackgroundLightMode = (Brush)Instance.FindResource("MahApps.Brushes.Gray");

                Instance.Background = BackgroundDarkMode;
                Instance.WindowTitleBrush = BackgroundDarkMode;
                Instance.GlowBrush = BackgroundDarkMode;
                Instance.NonActiveWindowTitleBrush = BackgroundDarkMode;
                Instance.NonActiveGlowBrush = BackgroundDarkMode;

                Properties.Settings.Default.DarkMode = true;
                SettingsPage.Instance.ThemeSelector.Content = "Dark";
                Properties.Settings.Default.Save();
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Instance, "Light." + Theme);

                BackgroundDarkMode = (Brush)Instance.FindResource("MahApps.Brushes.MenuItem.Background");
                BackgroundLightMode = (Brush)Instance.FindResource("MahApps.Brushes.Gray");

                Instance.Background = BackgroundDarkMode;
                Instance.WindowTitleBrush = BackgroundLightMode;
                Instance.GlowBrush = BackgroundLightMode;
                Instance.NonActiveWindowTitleBrush = BackgroundLightMode;
                Instance.NonActiveGlowBrush = BackgroundLightMode;

                Properties.Settings.Default.DarkMode = false;
                SettingsPage.Instance.ThemeSelector.Content = "Light";
                Properties.Settings.Default.Save();
            }
            ChangeTheme(SettingsPage.Instance);

        }
        public static void ChangeTheme(FrameworkElement self)
        {
            

            ThemeManager.Current.ChangeTheme(self, Properties.Settings.Default.DarkMode ? "Dark." + Theme : "Light." + Theme);
            foreach (var item in classesToUpdateTheme)
            {
                if(self != item)
                {
                    ThemeManager.Current.ChangeTheme(self, Properties.Settings.Default.DarkMode ? "Dark." + Theme : "Light." + Theme);
                }
            }
            foreach (var item in classesToUpdateTheme)
            {
                if (item == self)
                    return;
            }
            classesToUpdateTheme.Add(self);
        }
        private void LaunchGitHubSite(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Adesii");
        }

        private void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            transitionThing.Content = null;
            transitionThing.Content = MainFrame;
        }



        private void AddonSelectionBox_Initialized(object sender, EventArgs e)
        {
            AddonSelectionBox.ItemsSource = AddonManager.Addons;
        }
        private void AddonSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddonManager.ChangeAddon(AddonManager.Addons[AddonSelectionBox.SelectedIndex]);
        }
    }
}
