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

namespace PegSolitaireGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int BoardSize = 7;

        public MainWindow()
        {
            InitializeComponent();
            MyBoard.InitBoard(BoardSize);
        }

        private void OnStepBack(object sender, RoutedEventArgs e)
        {
            MyBoard.StepBack();
        }
    }
}
