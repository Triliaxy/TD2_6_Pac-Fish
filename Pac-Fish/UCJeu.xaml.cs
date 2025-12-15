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
using System.Windows.Threading;

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
            {1,2,2,2,2,2,2,2,2,2,2,2,2,1,1,2,2,2,2,2,2,2,2,2,2,2,2,1},
            {1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1},
            {1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1},
            {1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1},
            {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
            {1,2,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,2,1},
            {1,2,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,2,1},
            {1,2,2,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,2,2,1},
            {1,1,1,1,1,1,2,1,1,1,1,1,0,1,1,0,1,1,1,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,1,1,1,0,1,1,0,1,1,1,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,0,0,0,0,0,0,0,0,0,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,1,1,1,1,1,1,1,1,0,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,1,0,0,0,0,0,0,1,0,1,1,2,1,1,1,1,1,1},
            {0,0,0,0,0,0,2,0,0,0,1,0,0,0,0,0,0,1,0,0,0,2,0,0,0,0,0,0},
            {1,1,1,1,1,1,2,1,1,0,1,0,0,0,0,0,0,1,0,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,1,1,1,1,1,1,1,1,0,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,0,0,0,0,0,0,0,0,0,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,1,1,1,1,1,1,1,1,0,1,1,2,1,1,1,1,1,1},
            {1,1,1,1,1,1,2,1,1,0,1,1,1,1,1,1,1,1,0,1,1,2,1,1,1,1,1,1},
            {1,2,2,2,2,2,2,2,2,2,2,2,2,1,1,2,2,2,2,2,2,2,2,2,2,2,2,1},
            {1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1},
            {1,2,1,1,1,1,2,1,1,1,1,1,2,1,1,2,1,1,1,1,1,2,1,1,1,1,2,1},
            {1,2,2,2,1,1,2,2,2,2,2,2,2,0,0,2,2,2,2,2,2,2,1,1,2,2,2,1},
            {1,1,1,2,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,2,1,1,1},
            {1,1,1,2,1,1,2,1,1,2,1,1,1,1,1,1,1,1,2,1,1,2,1,1,2,1,1,1},
            {1,2,2,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,1,1,2,2,2,2,2,2,1},
            {1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,2,1},
            {1,2,1,1,1,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,2,1},
            {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };
        //todo : logique de jeu + génération logique

        private int tileSize = 30;

        private enum Direction { None, Up, Down, Left, Right } //ajout de l'option None pour ne pas bouger
        private Direction currentDirection = Direction.None; //direction actuelle du poisson
        private DispatcherTimer moveTimer; //timer pour gérer le déplacement

        public UCJeu()
        {
            InitializeComponent();

            //string nomFichierImage = $"pack://application:,,,/sprites/{MainWindow.Perso}.gif"; //plus utilisé 
            //imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

            DrawMaze();

            // redimensionne et aligne le poisson sur la grille
            InitializePlayerSizeAndPosition();

            // configure le timer de déplacement selon la variable globale
            moveTimer = new DispatcherTimer();
            moveTimer.Interval = TimeSpan.FromMilliseconds(MainWindow.MoveIntervalMs);
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        //commenter la section ci-dessous

        void DrawMaze()
        {
            // utilisation du champ tileSize
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

        private void InitializePlayerSizeAndPosition()
        {
            // définit la taille du poisson à une case
            imgPoisson.Width = tileSize;
            imgPoisson.Height = tileSize;

            // récupère la position actuelle (peut être NaN si non définie)
            double left = Canvas.GetLeft(imgPoisson);
            double top = Canvas.GetTop(imgPoisson);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            // calcule la cellule la plus proche et aligne exactement
            int cellX = (int)Math.Round(left / tileSize);
            int cellY = (int)Math.Round(top / tileSize);
            Canvas.SetLeft(imgPoisson, cellX * tileSize);
            Canvas.SetTop(imgPoisson, cellY * tileSize);

            // aligne le pas de déplacement sur tileSize si souhaité
            try
            {
                MainWindow.PasPoisson = tileSize;
            }
            catch
            {
                // si la propriété n'existe pas ou est inaccessible, on ignore silencieusement
            }
        }

        private void MoveTimer_Tick(object? sender, EventArgs e)
        {
            // déplace une seule fois par tick si une direction est active
            if (currentDirection == Direction.None) return;

            double left = Canvas.GetLeft(imgPoisson);
            double top = Canvas.GetTop(imgPoisson);
            if (double.IsNaN(left)) left = 0; //remplace par 0 dans le cas où la position n'est pas encore définie
            if (double.IsNaN(top)) top = 0;

            switch (currentDirection)
            {
                case Direction.Up:
                    Canvas.SetTop(imgPoisson, top - MainWindow.PasPoisson);
                    break;
                case Direction.Down:
                    Canvas.SetTop(imgPoisson, top + MainWindow.PasPoisson);
                    break;
                case Direction.Left:
                    Canvas.SetLeft(imgPoisson, left - MainWindow.PasPoisson);
                    break;
                case Direction.Right:
                    Canvas.SetLeft(imgPoisson, left + MainWindow.PasPoisson);
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
            Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
        }

        private void canvasJeu_KeyUp(object sender, KeyEventArgs e)
        {
            // si la touche relâchée correspond à la direction active, on stoppe
           switch (e.Key)
            {
                case Key.Up when currentDirection == Direction.Up:
                case Key.Down when currentDirection == Direction.Down:
                case Key.Left when currentDirection == Direction.Left:
                case Key.Right when currentDirection == Direction.Right:
                    currentDirection = Direction.None;
                    break;
            }
        }

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            // on mémorise la direction uniquement, le déplacement effectif se fait dans le timer
            switch (e.Key)
            {
                case Key.Up:
                    currentDirection = Direction.Up;
                    break;
                case Key.Down:
                    currentDirection = Direction.Down;
                    break;
                case Key.Left:
                    currentDirection = Direction.Left;
                    break;
                case Key.Right:
                    currentDirection = Direction.Right;
                    break;
            }
        }
    }
}