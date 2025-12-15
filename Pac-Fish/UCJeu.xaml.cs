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
        // Configuration / constants
        private const int DefaultTilePixelSize = 30;
        private const double SpeedIncreaseFactor = 0.7; // 0.9 = +30% de vitesse
        private const double MinimumMoveIntervalInMilliseconds = 30;

        // Etat du labyrinthe et des pellets
        // clé = (X=colonne, Y=ligne)
        private readonly Dictionary<(int Column, int Row), Ellipse> pelletDictionary = new();
        private readonly List<(int Column, int Row)> initialPelletCoordinates = new();

        // Représentation du labyrinthe (0=vide,1=mur,2=pellet)
        public static int[,] mazeGrid =
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
        private int tilePixelSize = DefaultTilePixelSize;

        // Déplacements et timer
        private enum Direction { None, Up, Down, Left, Right }
        private Direction currentMovementDirection = Direction.None;
        private DispatcherTimer gameLoopTimer;

        // Etat des chats: dernière direction (sans dictionnaire)
        private readonly Image[] enemyImages;
        private readonly Direction[] enemyLastDirections;
        private readonly Random randomGenerator = new Random();

        // Score
        private int currentScore = 0;
        private TextBlock scoreDisplay;

        // Indicateur pour ne dessiner les murs/pellets qu'une seule fois
        // private bool isMazeAlreadyDrawn = false;

        // Brushes et effets réutilisables (évite réallocation tous les ticks)
        private readonly LinearGradientBrush wallFillBrush;
        private readonly Brush wallStrokeBrush = new SolidColorBrush(Color.FromRgb(220, 100, 80));
        private readonly DropShadowEffect wallDropShadow;
        private readonly RadialGradientBrush pelletFillBrush;
        private readonly DropShadowEffect pelletDropShadow;

        public UCJeu()
        {
            InitializeComponent();

            // Prépare les brushes/effets une seule fois
            wallFillBrush = CreateWallGradientBrush();
            wallFillBrush.Freeze();

            wallDropShadow = new DropShadowEffect
            {
                Color = Color.FromRgb(255, 200, 180),
                BlurRadius = 6,
                ShadowDepth = 0,
                Opacity = 0.25
            };

            pelletFillBrush = CreatePelletGradientBrush();
            pelletFillBrush.Freeze();

            pelletDropShadow = new DropShadowEffect
            {
                Color = Colors.LightSkyBlue,
                BlurRadius = 6,
                ShadowDepth = 0,
                Opacity = 0.5
            };

            // Initialiser la collection de chats et tableaux de directions
            enemyImages = new[] { popcat1, popcat2, popcat3, popcat4, popcat5, popcat6 };
            enemyLastDirections = new Direction[enemyImages.Length];
            for (int i = 0; i < enemyLastDirections.Length; i++) enemyLastDirections[i] = Direction.None;

            // Gestion initiale
            RenderMazeGrid(); // ne dessinera qu'une fois grâce à mazeDrawn
            SetupCharacters();
            InitializeScoreDisplay();

            // configure le timer de déplacement selon la variable globale
            gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(MainWindow.MoveIntervalMs)
            };
            gameLoopTimer.Tick += GameLoopTimer_Tick;
            gameLoopTimer.Start();
        }

        // Prépare et ajoute le TextBlock de score
        private void InitializeScoreDisplay()
        {
            if (scoreDisplay != null) return;

            scoreDisplay = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 16,
                Text = "Score: 0"
            };
            Canvas.SetLeft(scoreDisplay, 6);
            Canvas.SetTop(scoreDisplay, 6);
            MazeCanvas.Children.Add(scoreDisplay);
        }

        // Dessine murs et pellets (exécuté une seule fois)
        void RenderMazeGrid()
        {
            // if (MazeCanvas == null || isMazeAlreadyDrawn) return;

            for (int rowIndex = 0; rowIndex < mazeGrid.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < mazeGrid.GetLength(1); columnIndex++)
                {
                    if (mazeGrid[rowIndex, columnIndex] == 1)
                    {
                        var wallVisual = CreateWallVisual(columnIndex, rowIndex);
                        MazeCanvas.Children.Add(wallVisual);
                    }
                    else if (mazeGrid[rowIndex, columnIndex] == 2)
                    {
                        var pelletVisual = CreatePelletVisual(columnIndex, rowIndex);
                        MazeCanvas.Children.Add(pelletVisual);
                        pelletDictionary[(columnIndex, rowIndex)] = pelletVisual;
                        initialPelletCoordinates.Add((columnIndex, rowIndex));
                    }
                }
            }

            // isMazeAlreadyDrawn = true;
        }

        // Crée un rectangle "corail" (utilise les brushes/effets partagés)
        private Rectangle CreateWallVisual(int columnIndex, int rowIndex)
        {
            var wallVisual = new Rectangle
            {
                Width = tilePixelSize,
                Height = tilePixelSize,
                RadiusX = Math.Max(4, tilePixelSize * 0.12),
                RadiusY = Math.Max(4, tilePixelSize * 0.12),
                Stroke = wallStrokeBrush,
                StrokeThickness = 1.0,
                Fill = wallFillBrush,
                Effect = wallDropShadow
            };

            Canvas.SetLeft(wallVisual, columnIndex * tilePixelSize);
            Canvas.SetTop(wallVisual, rowIndex * tilePixelSize);
            return wallVisual;
        }

        // Crée la bulle visuelle (pellet) — réutilise la même Brush/Effect
        private Ellipse CreatePelletVisual(int columnIndex, int rowIndex)
        {
            double size = Math.Max(8, tilePixelSize * 0.4);

            var pelletVisual = new Ellipse
            {
                Width = size,
                Height = size,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Opacity = 0.95,
                Fill = pelletFillBrush,
                Effect = pelletDropShadow
            };

            double positionLeft = columnIndex * tilePixelSize + (tilePixelSize - size) / 2.0;
            double positionTop = rowIndex * tilePixelSize + (tilePixelSize - size) / 2.0;
            Canvas.SetLeft(pelletVisual, positionLeft);
            Canvas.SetTop(pelletVisual, positionTop);

            return pelletVisual;
        }

        // Initialise la taille et la position du joueur sans ré-allocation inutile
        private void SetupCharacters()
        {
            imgPoisson.Width = tilePixelSize;
            imgPoisson.Height = tilePixelSize;
            popcat1.Width = tilePixelSize;
            popcat1.Height = tilePixelSize;
            popcat2.Width = tilePixelSize;
            popcat2.Height = tilePixelSize;
            popcat3.Width = tilePixelSize;
            popcat3.Height = tilePixelSize;
            popcat4.Width = tilePixelSize;
            popcat4.Height = tilePixelSize;
            popcat5.Width = tilePixelSize;
            popcat5.Height = tilePixelSize;
            popcat6.Width = tilePixelSize;
            popcat6.Height = tilePixelSize;

            int rowCount = mazeGrid.GetLength(0);
            int columnCount = mazeGrid.GetLength(1);

            int startingPositionX = 8;
            int startingPositionY = 14;


            Canvas.SetLeft(imgPoisson, startingPositionX * tilePixelSize);
            Canvas.SetTop(imgPoisson, startingPositionY * tilePixelSize);

            Canvas.SetLeft(popcat1, 11 * tilePixelSize);
            Canvas.SetTop(popcat1, 13 * tilePixelSize);

            Canvas.SetLeft(popcat2, 12 * tilePixelSize);
            Canvas.SetTop(popcat2, 15 * tilePixelSize);

            Canvas.SetLeft(popcat3, 13 * tilePixelSize);
            Canvas.SetTop(popcat3, 14 * tilePixelSize);

            Canvas.SetLeft(popcat4, 14 * tilePixelSize);
            Canvas.SetTop(popcat4, 14 * tilePixelSize);

            Canvas.SetLeft(popcat5, 15 * tilePixelSize);
            Canvas.SetTop(popcat5, 15 * tilePixelSize);

            Canvas.SetLeft(popcat6, 16 * tilePixelSize);
            Canvas.SetTop(popcat6, 13 * tilePixelSize);



            try
            {
                MainWindow.PasPoisson = tilePixelSize;
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
        private void GameLoopTimer_Tick(object? sender, EventArgs e)
        {
            if (currentMovementDirection != Direction.None)
            {
                MovePlayerCharacter();
            }
            MoveEnemiesRandomly();
            CheckForCollisions();
        }

        private void CheckForCollisions()
        {
            //position du joueur
            double playerLeft = Canvas.GetLeft(imgPoisson);
            double playerTop = Canvas.GetTop(imgPoisson);
            int playerColumnIndex = (int)Math.Round(playerLeft / tilePixelSize);
            int playerRowIndex = (int)Math.Round(playerTop / tilePixelSize);

            foreach (var enemyImage in enemyImages)
            {
                double enemyLeft = Canvas.GetLeft(enemyImage);
                double enemyTop = Canvas.GetTop(enemyImage);
                int enemyColumnIndex = (int)Math.Round(enemyLeft / tilePixelSize);
                int enemyRowIndex = (int)Math.Round(enemyTop / tilePixelSize);

                if (playerColumnIndex == enemyColumnIndex && playerRowIndex == enemyRowIndex)
                {
                    gameLoopTimer.Stop();
                    if (Application.Current.MainWindow is MainWindow mw)
                    {
                        mw.AfficheFinPartie();
                    }
                    break;
                }
            }
        }

        private void MovePlayerCharacter()
        {
            double positionLeft = Canvas.GetLeft(imgPoisson);
            double positionTop = Canvas.GetTop(imgPoisson);
            if (double.IsNaN(positionLeft)) positionLeft = 0;
            if (double.IsNaN(positionTop)) positionTop = 0;

            int currentColumnIndex = (int)Math.Round(positionLeft / tilePixelSize);
            int currentRowIndex = (int)Math.Round(positionTop / tilePixelSize);

            int rowCount = mazeGrid.GetLength(0);
            int columnCount = mazeGrid.GetLength(1);

            int targetColumnIndex = currentColumnIndex;
            int targetRowIndex = currentRowIndex;
            switch (currentMovementDirection)
            {
                case Direction.Up:
                    targetRowIndex = currentRowIndex - 1;
                    break;
                case Direction.Down:
                    targetRowIndex = currentRowIndex + 1;
                    break;
                case Direction.Left:
                    targetColumnIndex = currentColumnIndex - 1;
                    break;
                case Direction.Right:
                    targetColumnIndex = currentColumnIndex + 1;
                    break;
            }

            //Wrap horizontal si possible
            if ((targetColumnIndex < 0 || targetColumnIndex >= columnCount) && (currentMovementDirection == Direction.Left || currentMovementDirection == Direction.Right))
            {
                int wrapColumnIndex = (targetColumnIndex + columnCount) % columnCount;
                if (mazeGrid[currentRowIndex, currentColumnIndex] != 1 && mazeGrid[currentRowIndex, wrapColumnIndex] != 1)
                {
                    targetColumnIndex = wrapColumnIndex;
                    targetRowIndex = currentRowIndex;
                }
                else
                {
                    return;
                }
            }

            if (targetRowIndex < 0 || targetRowIndex >= rowCount ||
                targetColumnIndex < 0 || targetColumnIndex >= columnCount)
            {
                return;
            }

            if (mazeGrid[targetRowIndex, targetColumnIndex] == 1) return;

            Canvas.SetLeft(imgPoisson, targetColumnIndex * tilePixelSize);
            Canvas.SetTop(imgPoisson, targetRowIndex * tilePixelSize);

            TryToEatPellet(targetColumnIndex, targetRowIndex);
        }

        //déplacement aléatoire des chats avec contrainte: pas de demi-tour immédiat
        private void MoveEnemiesRandomly()
        {
            int rowCount = mazeGrid.GetLength(0);
            int columnCount = mazeGrid.GetLength(1);

            for (int i = 0; i < enemyImages.Length; i++)
            {
                var enemyImage = enemyImages[i];
                double positionLeft = Canvas.GetLeft(enemyImage);
                double positionTop = Canvas.GetTop(enemyImage);
                //if (double.IsNaN(left)) left = 0;
                //if (double.IsNaN(top)) top = 0;

                int columnIndex = (int)Math.Round(positionLeft / tilePixelSize);
                int rowIndex = (int)Math.Round(positionTop / tilePixelSize);

                var lastDirection = enemyLastDirections[i];

                //liste des directions possibles qui sont pas des murs
                var possibleDirections = new List<Direction>(4);
                if (rowIndex - 1 >= 0 && mazeGrid[rowIndex - 1, columnIndex] != 1) possibleDirections.Add(Direction.Up); //autorisé si y-1 est dans la grille et si la tuile au dessus n’est pas un mur ( 1 )
                if (rowIndex + 1 < rowCount && mazeGrid[rowIndex + 1, columnIndex] != 1) possibleDirections.Add(Direction.Down);
                if (columnIndex - 1 >= 0 && mazeGrid[rowIndex, columnIndex - 1] != 1) possibleDirections.Add(Direction.Left);
                if (columnIndex + 1 < columnCount && mazeGrid[rowIndex, columnIndex + 1] != 1) possibleDirections.Add(Direction.Right);

                // Retire le demi-tour immédiat (inverse de last)
                Direction oppositeDirection = GetOppositeDirection(lastDirection); //renvoie la direction opposée à la derniere
                if (lastDirection != Direction.None)//si le chat s’est déplacé, retirer le demi-tour
                {
                    possibleDirections.Remove(oppositeDirection);
                }

                if (possibleDirections.Count == 0)
                {
                    // si bloqué, autoriser demi-tour
                    possibleDirections = new List<Direction> { oppositeDirection };
                }
                
                var chosenDirection = possibleDirections[randomGenerator.Next(possibleDirections.Count)];//choisit une direction au hasard parmi les options restantes
                int newColumnIndex = columnIndex, newRowIndex = rowIndex;
                switch (chosenDirection)
                {
                    case Direction.Up: newRowIndex = rowIndex - 1; break;
                    case Direction.Down: newRowIndex = rowIndex + 1; break;
                    case Direction.Left: newColumnIndex = columnIndex - 1; break;
                    case Direction.Right: newColumnIndex = columnIndex + 1; break;
                }

                // Applique le déplacement
                Canvas.SetLeft(enemyImage, newColumnIndex * tilePixelSize);
                Canvas.SetTop(enemyImage, newRowIndex * tilePixelSize);
                enemyLastDirections[i] = chosenDirection;//sauvegarde la direction choisie comme dernière direction
            }
        }

        private static Direction GetOppositeDirection(Direction lastDirection)
        {

            switch(lastDirection)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.None;
            }
        }

        private void TryToEatPellet(int cellColumnIndex, int cellRowIndex)
        {
            if (mazeGrid[cellRowIndex, cellColumnIndex] != 2) return;

            mazeGrid[cellRowIndex, cellColumnIndex] = 0;
            currentScore += 10;

            if (pelletDictionary.TryGetValue((cellColumnIndex, cellRowIndex), out var pelletVisual))
            {
                MazeCanvas.Children.Remove(pelletVisual);
                pelletDictionary.Remove((cellColumnIndex, cellRowIndex));
            }

            if (scoreDisplay != null)
            {
                scoreDisplay.Text = $"Score: {currentScore}";
            }

            if (pelletDictionary.Count == 0)
            {
                ResetLevel();
            }
        }

        // Réinitialise les pellets à leurs positions initiales sans toucher à la position du joueur
        private void ResetLevel()
        {
            foreach (var (columnIndex, rowIndex) in initialPelletCoordinates)
            {
                if (mazeGrid[rowIndex, columnIndex] != 1)
                {
                    mazeGrid[rowIndex, columnIndex] = 2;
                    if (!pelletDictionary.ContainsKey((columnIndex, rowIndex)))
                    {
                        var pelletVisual = CreatePelletVisual(columnIndex, rowIndex);
                        MazeCanvas.Children.Add(pelletVisual);
                        pelletDictionary[(columnIndex, rowIndex)] = pelletVisual;
                    }
                }
            }

            if (gameLoopTimer != null)
            {
                double currentIntervalMs = gameLoopTimer.Interval.TotalMilliseconds;
                double newIntervalMs = Math.Max(MinimumMoveIntervalInMilliseconds, currentIntervalMs * SpeedIncreaseFactor);
                gameLoopTimer.Interval = TimeSpan.FromMilliseconds(newIntervalMs);

                try
                {
                    MainWindow.MoveIntervalMs = (int)newIntervalMs;
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
                case Key.Up when currentMovementDirection == Direction.Up:
                case Key.Down when currentMovementDirection == Direction.Down:
                case Key.Left when currentMovementDirection == Direction.Left:
                case Key.Right when currentMovementDirection == Direction.Right:
                    currentMovementDirection = Direction.None;
                    break;
            }
        }

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    currentMovementDirection = Direction.Up;
                    break;
                case Key.Down:
                    currentMovementDirection = Direction.Down;
                    break;
                case Key.Left:
                    currentMovementDirection = Direction.Left;
                    break;
                case Key.Right:
                    currentMovementDirection = Direction.Right;
                    break;
            }
        }

        // Factories pour Brushes/Effets (séparées pour lisibilité)
        private static LinearGradientBrush CreateWallGradientBrush()
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

        private static RadialGradientBrush CreatePelletGradientBrush()
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