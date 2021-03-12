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

namespace AAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public const string Theme = "Steel";

        public static MetroWindow Instance;

        public static Brush BackgroundLightMode;
        public static Brush BackgroundDarkMode;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            MainFrame.Content = Pages.SettingsPage.Instance;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            transitionThing.Content = null;
            transitionThing.Content = MainFrame;
            BackgroundDarkMode = (Brush)Instance.FindResource("MahApps.Brushes.MenuItem.Background");
            BackgroundLightMode = (Brush)Instance.FindResource("MahApps.Brushes.Gray");
            changeTheme(Properties.Settings.Default.DarkMode);

        }
        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Pages.SettingsPage.Instance;
        }

        private void EditorMenu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Pages.Editor.Instance;
        }
        public static void changeTheme(bool darkmode)
        {
            if (darkmode)
            {
                ThemeManager.Current.ChangeTheme(Instance, "Dark." + Theme);
                ThemeManager.Current.ChangeTheme(Editor.Instance, "Dark." + Theme);
                ThemeManager.Current.ChangeTheme(SettingsPage.Instance, "Dark." + Theme);

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
                ThemeManager.Current.ChangeTheme(Editor.Instance, "Light." + Theme);
                ThemeManager.Current.ChangeTheme(SettingsPage.Instance, "Light." + Theme);

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
    }
}
