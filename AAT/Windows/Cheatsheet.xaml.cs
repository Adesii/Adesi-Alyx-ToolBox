using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Markdig;
using Markdig.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xaml;
using System.Xml;
using XamlReader = System.Windows.Markup.XamlReader;

namespace AAT.Windows
{
    /// <summary>
    /// Interaction logic for Cheatsheet.xaml
    /// </summary>
    public partial class Cheatsheet : MetroWindow
    {
        private MarkdownPipeline BuildPipeline()
        {
            return new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
        }

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
            Loaded += MetroWindow_Loaded;
        }

        private void MetroWindow_Loaded(object sender, EventArgs e)
        {
            viewer.Pipeline = BuildPipeline();
            viewer.Markdown = Properties.Resources.CheatSheet;
            Regex reger = new("(>.+)\\w +");
            Regex numberReger = new(":+?[0-9].+?>", RegexOptions.Singleline);
            System.Collections.IList list1 = viewer.Document.Blocks;
            for (int i1 = 0; i1 < list1.Count; i1++)
            {
                Block item = (Block)list1[i1];
                if (item.GetType() == typeof(List))
                {


                    System.Collections.IList list = (item as List).ListItems;
                    for (int i = 0; i < list.Count; i++)
                    {
                        ListItem items = (ListItem)list[i];
                        TextRange text = new TextRange(items.ElementStart, items.ElementEnd);
                        if (text.Text.Contains("clr"))
                        {
                            var nm = text.Text.Substring(text.Text.IndexOf("clr:")+3, text.Text.IndexOf(">")- text.Text.IndexOf("r:"));
                            var nnna = nm.Split(",");
                            Color c = Color.FromRgb((byte)int.Parse(nnna[0][1..]), (byte)int.Parse(nnna[1]), (byte)int.Parse(nnna[2].Split(">")[0]));
                            
                            text.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(c));
                        }
                    }
                    
                }
                if(item.GetType() == typeof(Table))
                {
                    var t = (item as Table);
                    for (int j = 0; j < t.RowGroups.Count; j++)
                    {
                        TableRowGroup rg = t.RowGroups[j];
                        for (int i = 0; i < rg.Rows.Count; i++)
                        {
                            TableRow r = rg.Rows[i];
                            var lastCell = r.Cells[^1];
                            var text = new TextRange(lastCell.ContentStart, lastCell.ContentEnd);
                            if (lastCell.Blocks.Count <= 0 || text.Text.IndexOf("clr:") == -1) continue;
                            var fsbl=lastCell.Blocks.FirstBlock;
                            
                            

                            if (text.Text.IndexOf("clr:", text.Text.IndexOf("clr:") + 3) != -1)
                            {
                                var p = new TextRange(text.End.Paragraph.Inlines.FirstInline.ContentStart, text.End.Paragraph.Inlines.FirstInline.ContentEnd);
                                var nm = p.Text[(text.Text.IndexOf("clr:") + 3)..];
                                //Debug.WriteLine(nm);
                                if (string.IsNullOrEmpty(nm)) continue;
                                var nnna = nm.Split(",");
                                Color c = Color.FromRgb((byte)int.Parse(nnna[0][1..]), (byte)int.Parse(nnna[1]), (byte)int.Parse(nnna[2].Split(">")[0]));

                                p.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(c));

                                p = new TextRange(text.End.Paragraph.Inlines.LastInline.ContentStart, text.End.Paragraph.Inlines.LastInline.ContentEnd);

                                var nm2 = p.Text[(text.Text.IndexOf("clr:") + 3)..];
                                //Debug.WriteLine(nm);
                                if (string.IsNullOrEmpty(nm)) continue;
                                var nnna2 = nm2.Split(",");
                                Color c2 = Color.FromRgb((byte)int.Parse(nnna2[0][1..]), (byte)int.Parse(nnna2[1]), (byte)int.Parse(nnna2[2].Split(">")[0]));

                                p.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(c2));
                            }
                            else
                            {
                                var nm = text.Text[(text.Text.IndexOf("clr:") + 3)..];
                                //Debug.WriteLine(nm);
                                if (string.IsNullOrEmpty(nm)) continue;
                                var nnna = nm.Split(",");
                                Color c = Color.FromRgb((byte)int.Parse(nnna[0][1..]), (byte)int.Parse(nnna[1]), (byte)int.Parse(nnna[2].Split(">")[0]));
                                text.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush(c));
                            }
                        }
                    }
                }
            }
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
