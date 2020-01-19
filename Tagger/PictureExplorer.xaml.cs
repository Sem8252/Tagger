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
using Tagger.ViewModels;
//using System.Windows.Forms;
//using System.Drawing;

namespace Tagger
{
    /// <summary>
    /// Логика взаимодействия для PictureExplorer.xaml
    /// </summary>
    public partial class PictureExplorer : Window
    {

        PictureExplorerViewModel pictureExplorerView;


        public PictureExplorer(string dirPath)  // Конструктор класса
        {
            InitializeComponent();  // Базовая инициализация компонентов

            pictureExplorerView = new PictureExplorerViewModel(dirPath);

            Loaded += MainWindow_Loaded;

        }

        public PictureExplorer() : this("") // Конструктор по умолчанию
        {

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = pictureExplorerView;
            pictureExplorerView.UpdateImagesData();
            pictureExplorerView.UpdateImagesList();
        }





        private void Scan_Click(object sender, RoutedEventArgs e)   // Обработчик кнопки, отвечающей за сканирование
        {

            pictureExplorerView.UpdateImagesData();
            pictureExplorerView.UpdateImagesList();

        }


        private void SearchBox_KeyDown(object sender, KeyEventArgs e) // Обработчик нажатия кнопки Enter в блоке задания тегов
        {
            if (e.Key == Key.Enter)
            {
                pictureExplorerView.UpdateImagesData();
                pictureExplorerView.UpdateImagesList();
            }

        }


        protected void Check(object sender, RoutedEventArgs e) // Обработчик нажатия на картинку (отметить картинку)
        {

            if (e.OriginalSource is Image image)
                pictureExplorerView.ChangeImageSelection(image, checkSymbol);

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pictureExplorerView.Window_Closed();
        }



        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            pictureExplorerView.ClearButton_Clicked();
        }
    }
}
