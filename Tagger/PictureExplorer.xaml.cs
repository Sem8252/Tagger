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

        List<FileInfo> files = new List<FileInfo>(); // Информация о файлах
        public List<FileInfo> sortedFiles = new List<FileInfo>(); // Отсортированная информация о файлах
        string path;    // Путь до сканируемой папки



        public PictureExplorer(string dirPath)  // Конструктор класса
        {
            InitializeComponent();  // Базовая инициализация компонентов
            path = dirPath;     // Установка поля пути
            files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value); // Сканирование директории на наличие необходимых файлов
            foreach (var cur in files)
                sortedFiles.Add(cur);   // Добавление файлов в отсортированный список
        }



        public PictureExplorer() : this("") // Конструктор по умолчанию
        {

        }



        private void Scan_Click(object sender, RoutedEventArgs e)   // Обработчик кнопки, отвечающей за сканирование
        {

            files = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value); // Сканирование директории на наличие необходимых файлов

            sortedFiles.Clear();
            foreach (var cur in files)
                sortedFiles.Add(cur);   // Добавление файлов в отсортированный список

            var tagsToSearch = SearchBox.Text.Split(' '); // Массив строк с необходимыми тегами

            foreach(var tag in tagsToSearch) // Для каждого тега в списке
            {
                if (tag.Length > 1)
                    if (tag[0] == '-') // Если перед тегом стоит "минус", значит надо из отсортированных файлов исключить файлы с данным тегом
                    {
                        var badFiles = sortedFiles.FindAll(x => x.FullName.Contains(tag.Substring(1)));
                        foreach (var file in badFiles)
                            sortedFiles.Remove(file);
                    }
                    else //Если нет "минуса", то просто добавить лишь файлы с тегом
                        sortedFiles = sortedFiles.FindAll(x => x.FullName.Contains(tag));
            }
            if (checkMarks.Count == 0) // Если нет отметок, то очистить грид
            {
                grid.Children.Clear();
                redraw();
            }
        }



        protected List<string> checkedImages = new List<string>(); // Список помеченных картинок (строки)
        protected List<Image> checkMarks = new List<Image>();   // Список отметок (изображения)
        protected void check(object sender, RoutedEventArgs e) // Обработчик нажатия на картинку (отметить картинку)
        {
            string name = (string)((Image)sender).Tag; // Получаем

            if(checkedImages.Contains(name)) // Проверяем была ли картинка уже отмечена
            {
                checkedImages.Remove(name); // Если да, удалим ее название из списка отмеченных картинок
                grid.Children.Remove(checkMarks.Find(x => x.Margin.Left == ((Image)sender).Margin.Left && x.Margin.Top == ((Image)sender).Margin.Top));
                checkMarks.Remove(checkMarks.Find(x => x.Margin.Left == ((Image)sender).Margin.Left && x.Margin.Top == ((Image)sender).Margin.Top));
            }
            else 
            {
                checkedImages.Add(name); // Если нет, то добавим ее название в список отмеченных картинок 
                Image image = new Image(); // Создаем новую картинку
                image.Stretch = Stretch.Fill; // Должна заполнить область
                image.Height = 10; // Высота
                image.Width = 10; // Ширина
                image.HorizontalAlignment = HorizontalAlignment.Left; // Расположена слева
                image.VerticalAlignment = VerticalAlignment.Top;      // и сверху
                image.Margin = new Thickness(((Image)sender).Margin.Left, ((Image)sender).Margin.Top, 0, 0); // Расположение в верхнем левом углу картинки метки
                image.Source = checkSymbol.Source;
                checkMarks.Add(image);  // Добавление отметки в список отметок
                grid.Children.Add(checkMarks.Last());   // Добавление метки на грид
            }
            Transfer.PutSearchedFiles(checkedImages); // Добавление отмеченных файлов в список трансфера
        }



        int picturesInRow = 7;
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) // Перерисовка при смене размера окна
        {
            picturesInRow = (int)(window.Width-50) / 100; // Определение количества изобра
            if (checkMarks.Count == 0) // Удаление элементов грида при отстутствии отметок (картинок) (вроде спец костыль)
            {
                grid.Children.Clear(); // (внести в redraw)
                redraw(); // Перерисовка
            }
        }



        private void redraw()
        {
            int lastLeftGrid = 0;
            int lastTopGrid = 0;
            int ImageNum = 0;
            foreach (var cur in sortedFiles) // Для каждого файла в отсортированном списке
            {
                try
                {
                    ImageNum++; // Увеличим счетчик файлов на 1
                    BitmapImage bitmapImage = new BitmapImage(); // Создадим bitmap и инициализируем его указанной картинкой
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(cur.FullName, UriKind.Relative);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    var image = new Image(); // Создадим картинку и инициализируем ее
                    image.Stretch = Stretch.Fill;
                    image.Height = 100;
                    image.Width = 100;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.VerticalAlignment = VerticalAlignment.Top;
                    image.Margin = new Thickness(lastLeftGrid, lastTopGrid, 0, 0);
                    image.MouseDown += check;
                    image.Source = bitmapImage;
                    image.Tag = cur.FullName;

                    lastLeftGrid += 100;
                    if (ImageNum % picturesInRow == 0) // Если максимум картинок в ряду достигнут перейдем на нижнюю строку
                    {
                        lastTopGrid += 100;
                        lastLeftGrid = 0;
                    }

                    grid.Children.Add(image); // Добавление картинки на грид
                }
                catch { }
            }
            foreach(var cur in checkMarks)
            {
                grid.Children.Add(cur); // Добавление меток на грид
            }
            
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e) // Обработчик нажатия кнопки Enter в блоке задания тегов
        {
            if (e.Key == Key.Enter)
            {
                if (checkMarks.Count == 0) // Тот же костыль
                {
                    grid.Children.Clear();
                    Scan_Click(this, e);
                    redraw();
                }
            }
        }
        private void IsInnerDirectories_Checked(object sender, RoutedEventArgs e) // Обработка галочки на сканирование подпапок
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

        private void window_MouseEnter(object sender, MouseEventArgs e)
        {
            //var testScan = FileProcessor.ScanDirectories(path, IsInnerDirectories.IsChecked.Value);
            //if (!testScan.SequenceEqual(files))
            //{
            //    Scan_Click(this, e);
            //    redraw();
            //}
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Transfer.Clear();
            grid.Children.Clear();
            checkedImages.Clear();
            checkMarks.Clear();
            Scan_Click(this, e);
            redraw();
        }
    }
}
