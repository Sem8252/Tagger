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
using System.IO;
//using System.Drawing;

namespace Tagger
{
    /// <summary>
    /// Логика взаимодействия для PictureExplorer.xaml
    /// </summary>
    public partial class PictureExplorer : Window
    {
        public PictureExplorer(string dirPath)
        {
            InitializeComponent();
            path = dirPath;
            files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
            foreach (var cur in files)
                sortedFiles.Add(cur);
            //var tagLibrary = TagLib.ReadFromFile(path);
            //var tags = TagLib.GetAllTags(tagLibrary).OrderBy(x => x);
        }
        public PictureExplorer()
        {
            InitializeComponent();
            files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
            foreach (var cur in files)
                sortedFiles.Add(cur);
        }

        int picturesInRow = 7;

        List<FileInfo> files = new List<FileInfo>();
        public List<FileInfo> sortedFiles = new List<FileInfo>();
        string path;
        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            //files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
            //List<string> tagsToSearch = new List<string>();
            files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
            sortedFiles.Clear();
            foreach (var cur in files)
                sortedFiles.Add(cur);      
            var tagsToSearch = SearchBox.Text.Split(' ');
            //if(SearchBox.Text=="")
            foreach(var tag in tagsToSearch)
            {
                if (tag.Length > 1)
                    if (tag[0] == '-')
                    {
                        var badFiles = sortedFiles.FindAll(x => x.FullName.Contains(tag.Substring(1)));
                        foreach (var file in badFiles)
                            sortedFiles.Remove(file);
                    }
                    else
                        sortedFiles = sortedFiles.FindAll(x => x.FullName.Contains(tag));
            }
            if (checkMarks.Count == 0)
            {
                grid.Children.Clear();
                redraw();
            }
        }

        protected List<string> checkedImages = new List<string>();
        protected List<Image> checkMarks = new List<Image>();
        protected void check(object sender, RoutedEventArgs e)
        {
            string name = (string)((Image)sender).Tag;
            //MessageBox.Show(name);
            if(checkedImages.Contains(name))
            {
                checkedImages.Remove(name);
                grid.Children.Remove(checkMarks.Find(x => x.Margin.Left == ((Image)sender).Margin.Left && x.Margin.Top == ((Image)sender).Margin.Top));
                checkMarks.Remove(checkMarks.Find(x => x.Margin.Left == ((Image)sender).Margin.Left && x.Margin.Top == ((Image)sender).Margin.Top));
            }
            else 
            {
                checkedImages.Add(name);
                Image image = new Image();
                image.Stretch = Stretch.Fill;
                image.Height = 10;
                image.Width = 10;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;
                image.Margin = new Thickness(((Image)sender).Margin.Left, ((Image)sender).Margin.Top, 0, 0);
                image.Source = checkSymbol.Source;
                checkMarks.Add(image);
                grid.Children.Add(checkMarks.Last());
            }
            Transfer.PutSearchedFiles(checkedImages);
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            picturesInRow = (int)(window.Width-50) / 100;
            if (checkMarks.Count == 0)
            {
                grid.Children.Clear();
                redraw();
            }
        }
        private void redraw()
        {
            //files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
            int lastLeftGrid = 0;
            int lastTopGrid = 0;
            int ImageNum = 0;
            foreach (var cur in sortedFiles)
            {
                try
                {
                    ImageNum++;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(cur.FullName, UriKind.Relative);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    var image = new Image();
                    image.Stretch = Stretch.Fill;
                    image.Height = 100;
                    image.Width = 100;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.Margin = new Thickness(lastLeftGrid, lastTopGrid, 0, 0);
                    image.MouseDown += check;
                    lastLeftGrid += 100;
                    if (ImageNum % picturesInRow == 0)
                    {
                        lastTopGrid += 100;
                        lastLeftGrid = 0;
                    }
                    image.Source = bitmapImage;
                    image.Tag = cur.FullName;

                    grid.Children.Add(image);
                }
                catch { }
            }
            foreach(var cur in checkMarks)
            {
                grid.Children.Add(cur);
            }
            
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (checkMarks.Count == 0)
                {
                    grid.Children.Clear();
                    Scan_Click(this, e);
                    redraw();
                }
            }
        }
        private void IsInnerDirectories_Checked(object sender, RoutedEventArgs e)
        {
            if (checkMarks.Count == 0)
            {
                grid.Children.Clear();
                files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
                sortedFiles.Clear();
                foreach (var cur in files)
                    sortedFiles.Add(cur);
                Scan_Click(this, e);
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Transfer.Clear();
        }
    }
}
