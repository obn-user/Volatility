using System.Windows;
using Test.ViewModel;


namespace Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new VM();
        }

    }
}




