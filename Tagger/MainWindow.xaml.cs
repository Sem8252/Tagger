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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace Tagger
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected string path;
        protected List<FileInfo> files = new List<FileInfo>();
        protected List<string> tags = new List<string>();
        protected FolderBrowserDialog browser = new FolderBrowserDialog();
        protected static Random rand = new Random();
        protected const string characters = "qwertyuiopasdfghjklzxcvbnm0123456789";

        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {
            browser.ShowDialog();
            path = browser.SelectedPath;
            PathLabel.Text = path;
            Rescan();
        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            addTagKey();
        }

        private void DeleteTagButton_Click(object sender, RoutedEventArgs e)
        {
            //Rescan();
            FileProcessor.RemoveTag(files, TagListComboBox.SelectionBoxItem.ToString());
            Rescan();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Rescan();
        }

        protected void Rescan()
        {
            try
            {
                TagListComboBox.Items.Clear();
                files = FileProcessor.ScanDirectories(path, CheckBoxSubDirectory.IsChecked.Value);
                tags = FileProcessor.GetTagsFromDirectory(files);
                foreach (var tag in tags)
                    TagListComboBox.Items.Add(tag);
            }
            catch { }
        }

        private void CheckBoxSubDirectory_Checked(object sender, RoutedEventArgs e)
        {
            Rescan();
        }

        private void CheckBoxSubDirectory_Unchecked(object sender, RoutedEventArgs e)
        {
            Rescan();
        }

        private void ButtonFix_Click(object sender, RoutedEventArgs e)
        {
            Rescan();
            var largeFiles = files.FindAll(x => x.FullName.Contains("large"));
            foreach (FileInfo file in largeFiles)
            {
                int indexOfLarge = file.FullName.IndexOf("large");
                string newName = file.FullName.Remove(indexOfLarge - 1, 6);
                file.CopyTo(newName);
                file.Delete();
            }
            Rescan();
            var unusualFiles = files.FindAll(x => x.FullName.Contains("file"));
            foreach (FileInfo file in unusualFiles)
            {
                char[] randomName = new char[10];
                for (int i = 0; i < randomName.Length; i++)
                    randomName[i] = characters[rand.Next(characters.Length)];
                string newName = file.FullName.Replace("file", new string(randomName));
                file.CopyTo(newName);
                file.Delete();
            }
            Rescan();
            var copyes = files.FindAll(x => x.FullName.Contains("("));
            foreach (FileInfo file in copyes)
            {
                string newName = file.FullName;
                int indexOfErr = newName.IndexOf("(");
                newName = newName.Remove(indexOfErr - 1, 4);
                char[] randomName = new char[3];
                for (int i = 0; i < randomName.Length; i++)
                    randomName[i] = characters[rand.Next(characters.Length)];
                newName.Insert(0, new string(randomName));
                file.CopyTo(newName);
                file.Delete();
            }
            Rescan();
        }

        private void TextBoxTag_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                addTagKey();
            }
        }

        private void addTagKey()
        {
            Rescan();
            if (!(TextBoxTag.Text.Contains('%') || (TextBoxTag.Text == "")))
                FileProcessor.AddTag(files, TextBoxTag.Text);
            Rescan();
        }

        private void PathLabel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                path = PathLabel.Text;
                try { Rescan(); }
                catch { }
            }
        }
    }
}
