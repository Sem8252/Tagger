using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tagger.ViewModels
{
    class PictureExplorerViewModel : INotifyPropertyChanged // Лишнее, пока не используется(наследование)
    {
        public string path;

        bool _isInnerDirectoriesChecked = false;
        public bool isInnerDirectoriesChecked {
            get
            {
                return _isInnerDirectoriesChecked;
            }
            set 
            {
                _isInnerDirectoriesChecked = value;
            } 
        }

        string _tagsString = "";
        public string tagsString
        {
            get
            {
                return _tagsString;
            }
            set
            {
                _tagsString = value;
            }
        }

        List<FileInfo> files = new List<FileInfo>();

        ObservableCollection<Grid> _allImages = new ObservableCollection<Grid>();
        
        public ObservableCollection<Grid> allImagesHandlers
        {
            get
            {
                return _allImages;
            }
            set
            {
                _allImages = value;
            }
        }

        List<string> checkedImages = new List<string>(); // Список выбранных картинок

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public PictureExplorerViewModel(string path)
        {

            this.path = path;
            files = FileProcessor.ScanDirectories(path, isInnerDirectoriesChecked);

        }

        public void UpdateImagesData()
        {
            files = FileProcessor.ScanDirectories(path, isInnerDirectoriesChecked);

            string[] tagsForSearch = tagsString.Split(' ');

            foreach(var tag in tagsForSearch)
            {
                if (tag.Length > 1)
                {
                    if (tag[0] == '-')
                        files.RemoveAll(x => x.FullName.Contains(tag.Substring(1)));
                    else
                        files = files.FindAll(x => x.FullName.Contains(tag));
                }
            }

        }

        public void UpdateImagesList()
        {
            _allImages.Clear();
            foreach (var cur in files) // Для каждого файла в отсортированном списке
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();

                    bitmapImage.UriSource = new Uri(cur.FullName, UriKind.Relative);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                    bitmapImage.DecodePixelHeight = 100;
                    bitmapImage.DecodePixelWidth = 100;

                    bitmapImage.EndInit();

                    Image image = new Image(); // Создадим картинку и инициализируем ее
                    image.Stretch = Stretch.Fill;
                    image.Height = 100;
                    image.Width = 100;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.Source = bitmapImage;
                    image.Tag = cur.FullName;

                    Grid imageHandler = new Grid();
                    imageHandler.Children.Add(image);

                    allImagesHandlers.Add(imageHandler);

                }
                catch { }
            }


        }


        public void ChangeImageSelection(Image image, Image mark)
        {
            string name = (string)image.Tag;
            bool isRequired = false;    // Флаг, проверяющий входит ли картинка в список требуемых (не просто ли это пометка)

            foreach (var file in files)
            {
                if (file.FullName == name)
                    isRequired = true;
            }

            if (!isRequired)    // Если это просто какая-либо метка, выходим из метода
                return;

            Grid imageHandler = image.Parent as Grid;

            if (!checkedImages.Contains(name))
            {
                checkedImages.Add(name);
                AddCheckedMark(imageHandler, mark);
            }
            else
            {
                checkedImages.Remove(name);
                DeleteCheckedMark(imageHandler);
            }

            Transfer.PutSearchedFiles(checkedImages);
        }

        private void AddCheckedMark(Grid imageHandler, Image mark)
        {
            Image checkedMark = new Image();
            checkedMark.Stretch = Stretch.Fill;
            checkedMark.Height = 10;
            checkedMark.Width = 10;
            checkedMark.HorizontalAlignment = HorizontalAlignment.Left;
            checkedMark.VerticalAlignment = VerticalAlignment.Top;
            checkedMark.Source = mark.Source;
            checkedMark.Tag = "mark";

            imageHandler.Children.Add(checkedMark);
        }

        private void DeleteCheckedMark(Grid imageHandler)
        {
            var images = imageHandler.Children;
            for (int i = 0; i < imageHandler.Children.Count; i++)
            {
                if ((string)(images[i] as Image).Tag == "mark")
                    imageHandler.Children.RemoveAt(i);
            }
        }



        public void Window_Closed()
        {
            Transfer.Clear();
        }



        public void ClearButton_Clicked()
        {
            Transfer.Clear();
            checkedImages.Clear();

            for (int i = 0; i < allImagesHandlers.Count; i++)
                DeleteCheckedMark(allImagesHandlers[i]);
        }
    }
}
