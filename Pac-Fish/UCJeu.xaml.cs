using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
        // 0 = chemin vide, 1 = mur, 2 = points
        public static int[,] maze =
        {
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,1,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,1,2,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,1,2,2,2,1,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,1,1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
};
        //todo : logique de jeu + génération logique

        public UCJeu()
        {
            InitializeComponent();

            //string nomFichierImage = $"pack://application:,,,/sprites/{MainWindow.Perso}.gif"; //plus utilisé 
            //imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

            DrawMaze();
        }

        //commenter la section ci-dessous

        void DrawMaze()
        {
            int tileSize = 30;
            for (int y = 0; y < maze.GetLength(0); y++)
            {
                for (int x = 0; x < maze.GetLength(1); x++)
                {
                    if (maze[y, x] == 1)
                    {
                        var wall = new Rectangle
                        {
                            Width = tileSize,
                            Height = tileSize,
                            Fill = Brushes.Blue
                        };
                        Canvas.SetLeft(wall, x * tileSize);
                        Canvas.SetTop(wall, y * tileSize);
                        MazeCanvas.Children.Add(wall);
                    }
                    else if (maze[y, x] == 2)
                    {
                        var pellet = new Ellipse
                        {
                            Width = 8,
                            Height = 8,
                            Fill = Brushes.Gold
                        };
                        Canvas.SetLeft(pellet, x * tileSize + 11);
                        Canvas.SetTop(pellet, y * tileSize + 11);
                        MazeCanvas.Children.Add(pellet);
                    }
                }
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
            Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
        }

        private void canvasJeu_KeyUp(object sender, KeyEventArgs e)
        {
        
        }

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    if (Canvas.GetTop(imgPoisson) - MainWindow.PasPoisson <= 0)
                    {
                        break;
                    }
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

       

        
    }
}
