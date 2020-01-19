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

        ObservableCollection<Image> _allImages = new ObservableCollection<Image>();
        public ObservableCollection<Image> allImages
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

        //public List<Image> allImages
        //{
        //    get
        //    {
        //        return _allImages;
        //    }
        //    set
        //    {
        //        _allImages = value;
        //        OnPropertyChanged("allImages");
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public PictureExplorerViewModel(string path)
        {
            //allImages.CollectionChanged += CollectionChangedHandler;

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

            // Сформировать набор данных для привязки
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
                    bitmapImage.EndInit();

                    Image image = new Image(); // Создадим картинку и инициализируем ее
                    image.Stretch = Stretch.Fill;
                    image.Height = 100;
                    image.Width = 100;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.Source = bitmapImage;
                    image.Tag = cur.FullName;

                    allImages.Add(image);

                }
                catch { }
            }
        }
    }
}
