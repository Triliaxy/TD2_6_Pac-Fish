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
        //taille d'une case, permet d'aligner le déplacement sur la matrice
        private const int tileSize = 47;
        private DispatcherTimer moveTimer;

        // Direction courante du déplacement. None = pas de déplacement
        private Direction currentDirection = Direction.None;

        //Enum décrivant les directions possibles
        private enum Direction { None, Up, Down, Left, Right }

        // Matrice du labyrinthe: 0 = chemin vide, 1 = mur, 2 = pièce (pellet)
        // Chaque entrée correspond à une case de taille tileSize
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
            {1,1,1,1,1,1,2,1,1,0,1,1,1,0,0,1,1,1,0,1,1,2,1,1,1,1,1,1},
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

        public UCJeu()
        {
            InitializeComponent(); // Initialise les composants et charge le XAML associé

            DrawMaze(); // Construit visuellement le labyrinthe sur le Canvas

            // Création d'un timer déclenchant un tick toutes les 0,5 secondes pour déplacer le personnage
            moveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500) // Intervalle de 0,5 seconde
            };
            moveTimer.Tick += MoveTick; // Abonnement à l'événement déclenché à chaque tick
            moveTimer.Start(); // Lance le timer
        }

        void DrawMaze()
        {
            int pelletSize = 8; // Diamètre des pièces (pellets) en pixels.
            int pelletOffset = (tileSize - pelletSize) / 2; // Décalage pour centrer la pièce dans la tuile

            // Calcul et affectation des dimensions du Canvas du labyrinthe d'après la matrice et la taille des tuiles
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);
            MazeCanvas.Width = cols * tileSize;
            MazeCanvas.Height = rows * tileSize;

            // Demande d'alignement centré si le parent le supporte (Grid/StackPanel...)
            MazeCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            MazeCanvas.VerticalAlignment = VerticalAlignment.Center;

            //parcourt chaque case de la matrice et dessine soit un mur, soit une pièce, soit rien
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
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
                        //dessine une pièce (pellet) sous forme de ellipse dorée centrée dans la case
                        var pellet = new Ellipse
                        {
                            Width = pelletSize,
                            Height = pelletSize,
                            Fill = Brushes.Gold
                        };
                        //positionne la pièce au centre de la tuile de coordonnées (x, y)
                        Canvas.SetLeft(pellet, x * tileSize + pelletOffset);
                        Canvas.SetTop(pellet, y * tileSize + pelletOffset);
                        MazeCanvas.Children.Add(pellet);
                    }
                }
            }
        }

        private void MoveTick(object? sender, EventArgs e)
        {
            if (currentDirection == Direction.None) return;

            double x = Canvas.GetLeft(imgPoisson); //Position X actuelle du personnage
            double y = Canvas.GetTop(imgPoisson);  //Position Y actuelle du personnage

            switch (currentDirection)
            {
                case Direction.Up:
                    //déplacement vers le haut si on ne sort pas du Canvas
                    if (y - tileSize >= 0)
                    {
                        Canvas.SetTop(imgPoisson, y - tileSize);
                    }
                        break;
                    
                case Direction.Down:
                    //déplacement vers le bas (limite basse contrôlée)
                    if (y + tileSize + imgPoisson.ActualHeight <= MazeCanvas.Height)
                    {
                        Canvas.SetTop(imgPoisson, y + tileSize);
                    }
                        break;
                case Direction.Left:
                    if (x - tileSize < 0)
                    {
                        Canvas.SetLeft(imgPoisson, Math.Max(0, MazeCanvas.Width - imgPoisson.ActualWidth));//help ( revoir ce bout de code plus tard )
                    }
                    else
                    {
                        Canvas.SetLeft(imgPoisson, x - tileSize);
                    }
                    break;
                case Direction.Right:
                    if (x + tileSize + imgPoisson.ActualWidth > MazeCanvas.Width)//help ( revoir ce bout de code plus tard )
                    {
                        Canvas.SetLeft(imgPoisson, 0);
                    }
                    else
                    {
                        Canvas.SetLeft(imgPoisson, x + tileSize);
                    }
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //bout de code pour gérer les events clavier
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
            Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
        }

        private void canvasJeu_KeyUp(object sender, KeyEventArgs e)
        {
            //si la touche relachée alors arreter le déplacement
            currentDirection = Direction.None;
        }

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            //mettre à jour la direction en cas de pression sur une touche
            switch(e.Key)
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
