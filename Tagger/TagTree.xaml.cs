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

namespace Tagger
{
    /// <summary>
    /// Логика взаимодействия для TagTree.xaml
    /// </summary>
    public partial class TagTree : Window
    {
        public TagTree()
        {
            InitializeComponent();
            //object Item = null;
            //Tree.Items.Add(Item);
            ////Tree.ItemsSource = 
            ////Tree
        }
        public TagTree(List<string> tags)
        {
            InitializeComponent();
            //Tree.ItemsSource = tags;
            //ItemsControl itcon = new ItemsControl();
            //itcon.ItemsSource = tags;
            //TreeViewItem viewItem = new TreeViewItem();
            //viewItem.Header = "123";
            //Tree.Items.Add(viewItem);
            //Tree.be
            //Tree.Items.Add(new TreeViewItem().Header = "234");
        }
    }
}
