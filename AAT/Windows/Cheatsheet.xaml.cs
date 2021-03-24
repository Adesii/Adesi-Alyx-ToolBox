using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AAT.Windows
{
    /// <summary>
    /// Interaction logic for Cheatsheet.xaml
    /// </summary>
    public partial class Cheatsheet : MetroWindow
    {
        private static Cheatsheet m_instance;
        public static Cheatsheet Instance
        {
            get
            {
                if (m_instance == null) m_instance = new Cheatsheet();
                return m_instance;
            }
        }
        public Cheatsheet()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            m_instance = null;
        }
    }
}
