using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pac_Fish
{
    /// <summary>
    /// Logique d'interaction pour UCJeu.xaml
    /// </summary>
    public partial class UCJeu : UserControl
    {
        // Configuration / constantes
        private const int DefaultTileSize = 30;
        private const double IntervalReductionFactor = 0.9; // 0.9 = +10% de vitesse
        private const double MinIntervalMs = 30;

        // Etat du labyrinthe et des pellets
        // clé = (X=colonne, Y=ligne)
        private readonly Dictionary<(int X, int Y), Ellipse> pelletMap = new();
        private readonly List<(int X, int Y)> initialPelletPositions = new();

        // Représentation du labyrinthe (0=vide,1=mur,2=pellet)
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

        // Taille d'une case (modifiable si besoin)
        private int tileSize = DefaultTileSize;

        // Déplacements et timer
        private enum Direction { None, Up, Down, Left, Right }
        private Direction currentDirection = Direction.None;
        private DispatcherTimer moveTimer;

        // Score
        private int score = 0;
        private TextBlock scoreText;

        // Indicateur pour ne dessiner les murs/pellets qu'une seule fois
        private bool mazeDrawn = false;

        // Brushes et effets réutilisables (évite réallocation tous les ticks)
        private readonly LinearGradientBrush coralFillBrush;
        private readonly Brush coralStrokeBrush = new SolidColorBrush(Color.FromRgb(220, 100, 80));
        private readonly DropShadowEffect coralEffect;
        private readonly RadialGradientBrush bubbleFillBrush;
        private readonly DropShadowEffect bubbleEffect;

        public UCJeu()
        {
            InitializeComponent();

            // Prépare les brushes/effets une seule fois
            coralFillBrush = CreateCoralBrush();
            coralFillBrush.Freeze();

            coralEffect = new DropShadowEffect
            {
                Color = Color.FromRgb(255, 200, 180),
                BlurRadius = 6,
                ShadowDepth = 0,
                Opacity = 0.25
            };

            bubbleFillBrush = CreateBubbleBrush();
            bubbleFillBrush.Freeze();

            bubbleEffect = new DropShadowEffect
            {
                Color = Colors.LightSkyBlue,
                BlurRadius = 6,
                ShadowDepth = 0,
                Opacity = 0.5
            };

            // Gestion initiale
            DrawMaze(); // ne dessinera qu'une fois grâce à mazeDrawn
            InitializePlayerSizeAndPosition();
            CreateScoreText();

            // configure le timer de déplacement selon la variable globale
            moveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(MainWindow.MoveIntervalMs)
            };
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        // Prépare et ajoute le TextBlock de score
        private void CreateScoreText()
        {
            if (scoreText != null) return;

            scoreText = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 16,
                Text = "Score: 0"
            };
            Canvas.SetLeft(scoreText, 6);
            Canvas.SetTop(scoreText, 6);
            MazeCanvas.Children.Add(scoreText);
        }

        // Dessine murs et pellets (exécuté une seule fois)
        void DrawMaze()
        {
            if (MazeCanvas == null || mazeDrawn) return;

            for (int y = 0; y < maze.GetLength(0); y++)
            {
                for (int x = 0; x < maze.GetLength(1); x++)
                {
                    if (maze[y, x] == 1)
                    {
                        var coral = CreateCoral(x, y);
                        MazeCanvas.Children.Add(coral);
                    }
                    else if (maze[y, x] == 2)
                    {
                        var pellet = CreateBubble(x, y);
                        MazeCanvas.Children.Add(pellet);
                        pelletMap[(x, y)] = pellet;
                        initialPelletPositions.Add((x, y));
                    }
                }
            }

            mazeDrawn = true;
        }

        // Crée un rectangle "corail" (utilise les brushes/effets partagés)
        private Rectangle CreateCoral(int x, int y)
        {
            var coral = new Rectangle
            {
                Width = tileSize,
                Height = tileSize,
                RadiusX = Math.Max(4, tileSize * 0.12),
                RadiusY = Math.Max(4, tileSize * 0.12),
                Stroke = coralStrokeBrush,
                StrokeThickness = 1.0,
                Fill = coralFillBrush,
                Effect = coralEffect
            };

            Canvas.SetLeft(coral, x * tileSize);
            Canvas.SetTop(coral, y * tileSize);
            return coral;
        }

        // Crée la bulle visuelle (pellet) — réutilise la même Brush/Effect
        private Ellipse CreateBubble(int x, int y)
        {
            double size = Math.Max(8, tileSize * 0.4);

            var bubble = new Ellipse
            {
                Width = size,
                Height = size,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Opacity = 0.95,
                Fill = bubbleFillBrush,
                Effect = bubbleEffect
            };

            double left = x * tileSize + (tileSize - size) / 2.0;
            double top = y * tileSize + (tileSize - size) / 2.0;
            Canvas.SetLeft(bubble, left);
            Canvas.SetTop(bubble, top);

            return bubble;
        }

        // Initialise la taille et la position du joueur sans ré-allocation inutile
        private void InitializePlayerSizeAndPosition()
        {
            imgPoisson.Width = tileSize;
            imgPoisson.Height = tileSize;
            popcat1.Width = tileSize;
            popcat1.Height = tileSize;
            popcat2.Width = tileSize;
            popcat2.Height = tileSize;
            popcat3.Width = tileSize;
            popcat3.Height = tileSize;
            popcat4.Width = tileSize;
            popcat4.Height = tileSize;
            popcat5.Width = tileSize;
            popcat5.Height = tileSize;
            popcat6.Width = tileSize;
            popcat6.Height = tileSize;

            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);

            int startingPositionX = 8;
            int startingPositionY = 14;


            Canvas.SetLeft(imgPoisson, startingPositionX * tileSize);
            Canvas.SetTop(imgPoisson, startingPositionY * tileSize);

            Canvas.SetLeft(popcat1, 11 * tileSize);
            Canvas.SetTop(popcat1, 13 * tileSize);

            Canvas.SetLeft(popcat2, 12 * tileSize);
            Canvas.SetTop(popcat2, 15 * tileSize);

            Canvas.SetLeft(popcat3, 13 * tileSize);
            Canvas.SetTop(popcat3, 14 * tileSize);

            Canvas.SetLeft(popcat4, 14 * tileSize);
            Canvas.SetTop(popcat4, 14 * tileSize);

            Canvas.SetLeft(popcat5, 15 * tileSize);
            Canvas.SetTop(popcat5, 15 * tileSize);

            Canvas.SetLeft(popcat6, 16 * tileSize);
            Canvas.SetTop(popcat6, 13 * tileSize);


            //FAIRE DE MÊME POUR POSITIONNER LES CHATS








            try
            {
                MainWindow.PasPoisson = tileSize;
            }
            catch
            {
                // silencieux si propriété non accessible
            }
        }

        /*
        Timer de déplacement : déplace l'image du poisson selon la direction courante,
        vérifie si la nouvelle position correspond à une cellule contenant un pellet (2),
        gère la consommation du pellet via EatPelletAt (mise à 0 dans le tableau), la suppression graphique
        de l'ellipse correspondante et la mise à jour du score.

        Optimisation : on utilise pelletMap pour éviter les recherches LINQ dans MazeCanvas.Children.
        */
        private void MoveTimer_Tick(object? sender, EventArgs e)
        {
            if (currentDirection == Direction.None) return;

            double left = Canvas.GetLeft(imgPoisson);
            double top = Canvas.GetTop(imgPoisson);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            int currCellX = (int)Math.Round(left / tileSize);
            int currCellY = (int)Math.Round(top / tileSize);

            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);

            int targetCellX = currCellX;
            int targetCellY = currCellY;
            switch (currentDirection)
            {
                case Direction.Up:
                    targetCellY = currCellY - 1;
                    break;
                case Direction.Down:
                    targetCellY = currCellY + 1;
                    break;
                case Direction.Left:
                    targetCellX = currCellX - 1;
                    break;
                case Direction.Right:
                    targetCellX = currCellX + 1;
                    break;
            }

            // Wrap horizontal si possible
            if ((targetCellX < 0 || targetCellX >= cols) && (currentDirection == Direction.Left || currentDirection == Direction.Right))
            {
                int wrapX = (targetCellX + cols) % cols;
                if (maze[currCellY, currCellX] != 1 && maze[currCellY, wrapX] != 1)
                {
                    targetCellX = wrapX;
                    targetCellY = currCellY;
                }
                else
                {
                    return;
                }
            }

            if (targetCellY < 0 || targetCellY >= rows ||
                targetCellX < 0 || targetCellX >= cols)
            {
                return;
            }

            if (maze[targetCellY, targetCellX] == 1) return;

            Canvas.SetLeft(imgPoisson, targetCellX * tileSize);
            Canvas.SetTop(imgPoisson, targetCellY * tileSize);

            EatPelletAt(targetCellX, targetCellY);
        }

        private void EatPelletAt(int cellX, int cellY)
        {
            if (maze[cellY, cellX] != 2) return;

            maze[cellY, cellX] = 0;
            score += 10;

            if (pelletMap.TryGetValue((cellX, cellY), out var pellet))
            {
                MazeCanvas.Children.Remove(pellet);
                pelletMap.Remove((cellX, cellY));
            }

            if (scoreText != null)
            {
                scoreText.Text = $"Score: {score}";
            }

            if (pelletMap.Count == 0)
            {
                ResetPelletsAndIncreaseSpeed();
            }
        }

        // Réinitialise les pellets à leurs positions initiales sans toucher à la position du joueur
        private void ResetPelletsAndIncreaseSpeed()
        {
            foreach (var (x, y) in initialPelletPositions)
            {
                if (maze[y, x] != 1)
                {
                    maze[y, x] = 2;
                    if (!pelletMap.ContainsKey((x, y)))
                    {
                        var pellet = CreateBubble(x, y);
                        MazeCanvas.Children.Add(pellet);
                        pelletMap[(x, y)] = pellet;
                    }
                }
            }

            if (moveTimer != null)
            {
                double currentMs = moveTimer.Interval.TotalMilliseconds;
                double newMs = Math.Max(MinIntervalMs, currentMs * IntervalReductionFactor);
                moveTimer.Interval = TimeSpan.FromMilliseconds(newMs);

                try
                {
                    MainWindow.MoveIntervalMs = (int)newMs;
                }
                catch
                {
                    // ignore
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

        // Factories pour Brushes/Effets (séparées pour lisibilité)
        private static LinearGradientBrush CreateCoralBrush()
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 127, 80), 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 160, 122), 0.45));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 99, 71), 0.8));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(200, 70, 60), 1.0));
            return brush;
        }

        private static RadialGradientBrush CreateBubbleBrush()
        {
            var brush = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.35, 0.35),
                Center = new Point(0.35, 0.35),
                RadiusX = 0.8,
                RadiusY = 0.8
            };
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(220, 255, 255, 255), 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(160, 180, 220, 255), 0.6));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(110, 120, 180, 255), 1.0));
            return brush;
        }
    }
}