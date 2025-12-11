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

namespace Pac_Fish
{
    /// <summary>
    /// Logique d'interaction pour UCJeu.xaml
    /// </summary>
    public partial class UCJeu : UserControl
    {
        
        
        public UCJeu()
        {
            InitializeComponent();
            string nomFichierImage = $"pack://application:,,,/PlaceHolders/{MainWindow.Perso}.gif";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
            Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
        }

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - MainWindow.PasPoisson);
                    break;
                case Key.Down:
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + MainWindow.PasPoisson);
                    break;
                case Key.Left:
                    Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) - MainWindow.PasPoisson);
                    break;
                case Key.Right:
                    Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) + MainWindow.PasPoisson);
                    break;
            }
        }

        private void canvasJeu_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        
    }
}
