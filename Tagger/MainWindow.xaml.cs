﻿using System;
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
            CheckBD(bdPath);
        }

        public void CheckBD(string path)
        {
            tagLibrary = TagLib.ReadFromFile(path);
            var tags = TagLib.GetAllTags(tagLibrary).OrderBy(x=>x);
            foreach (var tag in tags)
                BDBox.Items.Add(tag);
        }
        

        PictureExplorer pictureExplorer;
        
        protected string path;
        protected string bdPath = "BDTags.txt";
        protected List<FileInfo> files = new List<FileInfo>();
        protected List<string> tags = new List<string>();
        protected FolderBrowserDialog browser = new FolderBrowserDialog();
        protected static Random rand = new Random();
        protected const string characters = "qwertyuiopasdfghjklzxcvbnm0123456789";
        protected List<string[]> tagLibrary = new List<string[]>();
        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {
            browser.ShowDialog();
            path = browser.SelectedPath;
            PathLabel.Text = path;
            Rescan();
        }
        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkedPictures == 0)
            {
                Transfer.Clear();
                Transfer.PutSearchedFiles(files);
                addTagKey(files);
                Transfer.Clear();
            }
            else
                addTagKey(checkedImages);
        }
        private void DeleteTagButton_Click(object sender, RoutedEventArgs e)
        {
            //Rescan();
            if (checkedPictures == 0)
                FileProcessor.RemoveTag(files, TagListComboBox.SelectionBoxItem.ToString());
            else
                FileProcessor.RemoveTag(checkedImages, TagListComboBox.SelectionBoxItem.ToString());
            Rescan();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Rescan();
        }

        int checkedPictures = 0;
        protected void Rescan()
        {
            try
            {
                checkedImages = Transfer.GetSearchedFiles();
                TagListComboBox.Items.Clear();
                files = FileProcessor.ScanDirectories(path, CheckBoxSubDirectory.IsChecked.Value);
                List<FileInfo> filesToFind = new List<FileInfo>(checkedImages);
                checkedImages.Clear();
                foreach(var file in filesToFind)
                    checkedImages.Add(files.Find(x => x.FullName.Split('%')[0].Contains(file.FullName.Split('.')[0].Split('%')[0])));
                if (checkedImages == null)
                    System.Windows.MessageBox.Show("АТАТА!");
                Transfer.PutSearchedFiles(checkedImages);
                tags = FileProcessor.GetTagsFromDirectory(files);
                foreach (var tag in tags)
                    TagListComboBox.Items.Add(tag);
                checkedPictures = checkedImages.Count;
                CheckedCount.Content = checkedPictures;
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
                if (checkedPictures == 0)
                    addTagKey(files);
                else
                    addTagKey(checkedImages);
            }
        }
        private void addTagKey(List<FileInfo> filesToTag)
        {
            Rescan();
            if (!(TextBoxTag.Text.Contains('%') || (TextBoxTag.Text == "")))
                if(tagLibrary.Find(x=>x[1].Equals(TextBoxTag.Text))!=null)
                {
                    AddAll(TextBoxTag.Text);
                }
            else
                FileProcessor.AddTag(filesToTag, TextBoxTag.Text);
            Rescan();
        }
        private void PathLabel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                path = PathLabel.Text;
                browser.SelectedPath = path;
                try { Rescan(); }
                catch { }
            }
        }
        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            Rescan();
            var selectedItem = BDBox.SelectedItem.ToString();
            if (checkedPictures == 0)
            {
                Transfer.Clear();
                Transfer.PutSearchedFiles(files);
                AddAll(selectedItem);
                Transfer.Clear();
            }
            else
                AddAll(selectedItem);
        }
        private void AddToBDBut_Click(object sender, RoutedEventArgs e)
        {
            if (BDBox.SelectedIndex != -1)
            {
                var parent = BDBox.SelectedItem.ToString();
                tagLibrary.Add(new string[] { parent, ChildBox.Text });
                TagLib.WriteToFile(tagLibrary, bdPath);
                CheckBD(bdPath);
            }
            else System.Windows.MessageBox.Show("Выберите предка!");
        }
        private void AddAll(string tag)
        {
            var toUse = TagLib.TagsToWrite(tagLibrary, tag);
            Rescan();
            List<FileInfo> toTag = new List<FileInfo>(checkedImages);
            foreach (var curTag in toUse)
            {
                FileProcessor.AddTag(toTag, curTag);
                Rescan();
                toTag.Clear();
                toTag = new List<FileInfo>(checkedImages);
            }
        }

        private void OpenExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (PathLabel.Text != "")
            {
                try
                {
                    pictureExplorer = new PictureExplorer(path);
                    pictureExplorer.Show();
                }
                catch { }
            }
        }

        List<FileInfo> checkedImages = new List<FileInfo>();
        private void testbutton_Click(object sender, RoutedEventArgs e)
        {
            //checkedImages = Transfer.GetSearchedFiles();
            if(checkedImages.Count>0)
            {
                addTagKey(checkedImages);   
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Rescan();
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Rescan();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Transfer.Clear();
            Rescan();
        }

        TagTree treeWindow;
        private void ShowTree_Click(object sender, RoutedEventArgs e)
        {
            treeWindow = new TagTree(tags);
            treeWindow.Show();
        }

        private void TagListComboBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            
            foreach (var tag in tags)
                TagListComboBox.Items.Add(tag);
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            Rescan();
            if (checkedPictures == 0)
            {
                Transfer.Clear();
                Transfer.PutSearchedFiles(files);
                RandomName(files);
                Transfer.Clear();
            }
            else
                RandomName(checkedImages);
            Rescan();
        }

        private void RandomName(List<FileInfo> files)
        {
            foreach(var file in files)
            {
                var splittedName = file.FullName.Split('.');
                var res = splittedName[splittedName.Length-1];
                StringBuilder newname = new StringBuilder();
                string chars = "1234567890qwertyuiopasdfghjklzxcvbnm";
                newname.Append(file.DirectoryName+"\\");
                for (int i = 0; i <= 10; i++)
                {
                    newname.Append(chars[rand.Next(0, 35)]);
                }
                newname.Append("."+res);
                if (!File.Exists(newname.ToString()))
                {
                    file.CopyTo(newname.ToString());
                    file.Delete();
                }
                else
                {
                    newname.Append("-1");
                    file.CopyTo(newname.ToString());
                    System.Windows.MessageBox.Show("УПС!");
                }
            }
        }
    }
}
