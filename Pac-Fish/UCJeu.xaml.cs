using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;


namespace Pac_Fish
{
    /// <summary>
    /// Logique d'interaction pour UCJeu.xaml
    /// </summary>
    public partial class UCJeu : UserControl
    {
        // Config / constantes
        private const int DefaultTilePixelSize = 30;

        private const int PelletPoints = 10;
        private const int EnemyEatPoints = 200;

        private const double SpeedIncreaseFactor = 0.7; // 0.7 => +43% de vitesse (intervalle réduit)
        private const double MinimumMoveIntervalInMilliseconds = 30;

        private static readonly TimeSpan PowerModeDuration = TimeSpan.FromSeconds(8);

        // Etat du labyrinthe et des collectibles
        // clé = (X=colonne, Y=ligne)
        private readonly Dictionary<(int Column, int Row), Ellipse> pelletDictionary = new();
        private readonly List<(int Column, int Row)> initialPelletCoordinates = new();

        // Stéroïdes (power-pellets)
        private readonly Dictionary<(int Column, int Row), Shape> steroidDictionary = new();
        private readonly List<(int Column, int Row)> initialSteroidCoordinates = new();

        // Emplacements par défaut des stéroïdes (choisis sur des cases chemin)
        private readonly (int Column, int Row)[] defaultSteroidCoordinates =
        {
            (1, 1),                // proche coin haut-gauche
            (26, 1),               // proche coin haut-droit
            (1, 28),               // proche coin bas-gauche
            (26, 28)               // proche coin bas-droit
        };

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

        // Taille d'une case
        private int tilePixelSize = DefaultTilePixelSize;

        // Déplacements / timer jeu
        private enum Direction { None, Up, Down, Left, Right }
        private Direction currentMovementDirection = Direction.None;
        private DispatcherTimer gameLoopTimer;

        // Power mode (stéroïdes)
        private bool isPowerModeActive;
        private DispatcherTimer powerModeTimer;

        // Ennemis (chats)
        private readonly Image[] enemyImages;
        private readonly Direction[] enemyLastDirections;
        private readonly (int Column, int Row)[] enemySpawnCells;
        private readonly Random randomGenerator = new Random();

        // Score
        private int currentScore = 0;
        private int bestScore = 0;
        private TextBlock scoreDisplay;
        private TextBlock bestScoreDisplay;

        // Brushes / effets réutilisables
        private readonly LinearGradientBrush wallFillBrush;
        private readonly Brush wallStrokeBrush = new SolidColorBrush(Color.FromRgb(220, 100, 80));
        private readonly DropShadowEffect wallDropShadow;

        private readonly RadialGradientBrush pelletFillBrush;
        private readonly DropShadowEffect pelletDropShadow;

        private readonly RadialGradientBrush steroidFillBrush;
        private readonly DropShadowEffect steroidDropShadow;

        private readonly DropShadowEffect vulnerableEnemyGlow;

        private MediaPlayer mediaPlayer = new MediaPlayer(); // Initialisation d'un MediaPlayer pour charger la musique de jeu en .mp3

        private bool MusiqueDeFondLoaded = false; // Indicateur de chargement de la musique
        private void InitPlayerMusiqueDeFond()
        {
            string musicPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "musics", "music.mp3");
            if (File.Exists(musicPath))
            {
                mediaPlayer.Open(new Uri(musicPath, UriKind.Absolute));
            }

            mediaPlayer.Volume = 0.4;

            // Signaler le chargement est terminé
            mediaPlayer.MediaOpened += (s, e) =>
            {
                MusiqueDeFondLoaded = true;
                mediaPlayer.Play();
            };

            mediaPlayer.MediaFailed += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Erreur chargement musique: {e.ErrorException}");
            };

            // Relancer la musique quand elle termine
            mediaPlayer.MediaEnded += (s, e) =>
            {
                mediaPlayer.Stop();
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
            };
        }

        private void LancerMusiqueDeFond()
        {
            mediaPlayer.Stop(); // Retour au début de la boucle si déjà en cours
            mediaPlayer.Play(); // Lancer la lecture
        }

        private void StopperMusiqueDeFond()
        {
            mediaPlayer.Stop(); // Arrêter la lecture
        }


        public UCJeu()
        {
            InitializeComponent();

            // Initialisation du lecteur audio pour la musique de fond
            InitPlayerMusiqueDeFond();
            // LancerMusiqueDeFond(); // La lecture démarre automatiquement via MediaOpened

            // Prépare brushes/effets une fois
            wallFillBrush = CreateWallGradientBrush(); wallFillBrush.Freeze();
            wallDropShadow = new DropShadowEffect { Color = Color.FromRgb(255, 200, 180), BlurRadius = 6, ShadowDepth = 0, Opacity = 0.25 };

            pelletFillBrush = CreatePelletGradientBrush(); pelletFillBrush.Freeze();
            pelletDropShadow = new DropShadowEffect { Color = Colors.LightSkyBlue, BlurRadius = 6, ShadowDepth = 0, Opacity = 0.5 };

            steroidFillBrush = CreateSteroidGradientBrush(); steroidFillBrush.Freeze();
            steroidDropShadow = new DropShadowEffect { Color = Color.FromRgb(180, 0, 255), BlurRadius = 10, ShadowDepth = 0, Opacity = 0.7 };

            vulnerableEnemyGlow = new DropShadowEffect { Color = Color.FromRgb(50, 180, 255), BlurRadius = 15, ShadowDepth = 0, Opacity = 0.85 };

            // Collections ennemis + spawns
            enemyImages = new[] { popcat1, popcat2, popcat3, popcat4, popcat5, popcat6 };
            enemyLastDirections = new Direction[enemyImages.Length];
            for (int i = 0; i < enemyLastDirections.Length; i++) enemyLastDirections[i] = Direction.None;

            // Spawns des chats (colonnes/lignes alignées à SetupCharacters)
            enemySpawnCells = new[]
            {
                (11, 13),
                (12, 15),
                (13, 14),
                (14, 14),
                (15, 15),
                (16, 13)
            };

            // Initialisation
            RenderMazeGrid();
            SetupCharacters();

            bestScore = BestScoreManager.LoadBestScore();

            InitializeScoreDisplay();

            // Timer jeu principal
            gameLoopTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(MainWindow.MoveIntervalMs) };
            gameLoopTimer.Tick += GameLoopTimer_Tick;
            gameLoopTimer.Start();

            // Timer power mode (one-shot)
            powerModeTimer = new DispatcherTimer { Interval = PowerModeDuration };
            powerModeTimer.Tick += (_, __) => DeactivatePowerMode();
        }

        // prépare et ajoute le TextBlock de score
        // Score UI
        private void InitializeScoreDisplay()
        {
            if (scoreDisplay != null) return;

            scoreDisplay = new TextBlock { Foreground = Brushes.White, FontSize = 16, Text = "Score: 0" };
            Canvas.SetLeft(scoreDisplay, 6);
            Canvas.SetTop(scoreDisplay, 6);
            MazeCanvas.Children.Add(scoreDisplay);

            bestScoreDisplay = new TextBlock
            {
                Foreground = Brushes.Gold,
                FontSize = 16,
                Text = $"Meilleur: {bestScore}"
            };
            Canvas.SetLeft(bestScoreDisplay, 6);
            Canvas.SetTop(bestScoreDisplay, 26); // Positionné 20px plus bas
            MazeCanvas.Children.Add(bestScoreDisplay);
        }

        // Dessin labyrinthe + collectibles
        private void RenderMazeGrid()
        {
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

            // Place les stéroïdes (si la case n'est pas un mur)
            foreach (var (col, row) in defaultSteroidCoordinates)
            {
                if (row >= 0 && row < mazeGrid.GetLength(0) &&
                    col >= 0 && col < mazeGrid.GetLength(1) &&
                    mazeGrid[row, col] != 1)
                {
                    var steroid = CreateSteroidVisual(col, row);
                    MazeCanvas.Children.Add(steroid);
                    steroidDictionary[(col, row)] = steroid;
                    initialSteroidCoordinates.Add((col, row));
                }
            }
        }

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

        private Shape CreateSteroidVisual(int columnIndex, int rowIndex)
        {
            // Bulle plus grande et violette pour se distinguer
            double size = Math.Max(12, tilePixelSize * 0.6);

            var visual = new Ellipse
            {
                Width = size,
                Height = size,
                Stroke = new SolidColorBrush(Color.FromRgb(200, 0, 255)),
                StrokeThickness = 2,
                Opacity = 0.98,
                Fill = steroidFillBrush,
                Effect = steroidDropShadow
            };

            double left = columnIndex * tilePixelSize + (tilePixelSize - size) / 2.0;
            double top = rowIndex * tilePixelSize + (tilePixelSize - size) / 2.0;
            Canvas.SetLeft(visual, left);
            Canvas.SetTop(visual, top);

            return visual;
        }

        private void SetupCharacters()
        {
            imgPoisson.Width = tilePixelSize;
            imgPoisson.Height = tilePixelSize;

            // Taille chats
            foreach (var enemy in enemyImages)
            {
                enemy.Width = tilePixelSize;
                enemy.Height = tilePixelSize;
            }

            // Spawn du joueur (cellules valides connues)
            int startingPositionX = 8;
            int startingPositionY = 14;

            Canvas.SetLeft(imgPoisson, startingPositionX * tilePixelSize);
            Canvas.SetTop(imgPoisson, startingPositionY * tilePixelSize);

            // Spawns des chats
            for (int i = 0; i < enemyImages.Length && i < enemySpawnCells.Length; i++)
            {
                var (c, r) = enemySpawnCells[i];
                Canvas.SetLeft(enemyImages[i], c * tilePixelSize);
                Canvas.SetTop(enemyImages[i], r * tilePixelSize);
                enemyLastDirections[i] = Direction.None;
                enemyImages[i].Effect = null;
                enemyImages[i].Opacity = 1.0;
            }

            try { MainWindow.PasPoisson = tilePixelSize; } catch { /* ignore */ }
        }

        /*
        Timer de déplacement : déplace l'image du poisson selon la direction courante,
        vérifie si la nouvelle position correspond à un collectible (pellet/stéroïde),
        gère la consommation, la suppression graphique et la mise à jour du score.

        Optimisation : dictionnaires pour éviter les recherches LINQ dans MazeCanvas.Children.
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
            double playerLeft = Canvas.GetLeft(imgPoisson);
            double playerTop = Canvas.GetTop(imgPoisson);
            int playerColumnIndex = (int)Math.Round(playerLeft / tilePixelSize);
            int playerRowIndex = (int)Math.Round(playerTop / tilePixelSize);

            for (int i = 0; i < enemyImages.Length; i++)
            {
                var enemyImage = enemyImages[i];
                double enemyLeft = Canvas.GetLeft(enemyImage);
                double enemyTop = Canvas.GetTop(enemyImage);
                int enemyColumnIndex = (int)Math.Round(enemyLeft / tilePixelSize);
                int enemyRowIndex = (int)Math.Round(enemyTop / tilePixelSize);

                if (playerColumnIndex == enemyColumnIndex && playerRowIndex == enemyRowIndex)
                {
                    if (isPowerModeActive)
                    {
                        // Mange le chat: +200 pts, renvoi au spawn
                        currentScore += EnemyEatPoints;
                        UpdateScoreUI();
                        SendEnemyToSpawn(i);
                        // On continue la boucle, pas de fin de partie
                    }
                    else
                    {
                        // Fin de partie
                        gameLoopTimer.Stop();
                        powerModeTimer.Stop();
                        StopperMusiqueDeFond();

                        // Sauvegarde le meilleur score si nécessaire
                        if (currentScore > bestScore)
                        {
                            BestScoreManager.SaveBestScore(currentScore);
                        }

                        // Affiche l'écran de fin
                        if (Application.Current.MainWindow is MainWindow mainWindow)
                        {
                            mainWindow.AfficheFinPartie();
                        }
                    }
                    // Une seule collision par tick est suffisante
                    break;
                }
            }
        }

        private void SendEnemyToSpawn(int index)
        {
            if (index < 0 || index >= enemyImages.Length || index >= enemySpawnCells.Length) return;
            var (c, r) = enemySpawnCells[index];
            Canvas.SetLeft(enemyImages[index], c * tilePixelSize);
            Canvas.SetTop(enemyImages[index], r * tilePixelSize);
            enemyLastDirections[index] = Direction.None;
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
                case Direction.Up: targetRowIndex = currentRowIndex - 1; break;
                case Direction.Down: targetRowIndex = currentRowIndex + 1; break;
                case Direction.Left: targetColumnIndex = currentColumnIndex - 1; break;
                case Direction.Right: targetColumnIndex = currentColumnIndex + 1; break;
            }

            // Wrap horizontal si possible
            if ((targetColumnIndex < 0 || targetColumnIndex >= columnCount) &&
                (currentMovementDirection == Direction.Left || currentMovementDirection == Direction.Right))
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

            // Consommer items éventuels (pellet / stéroïde)
            TryConsumeItems(targetColumnIndex, targetRowIndex);
        }

        // Déplacement aléatoire des chats, sans demi-tour immédiat
        private void MoveEnemiesRandomly()
        {
            int rowCount = mazeGrid.GetLength(0);
            int columnCount = mazeGrid.GetLength(1);

            for (int i = 0; i < enemyImages.Length; i++)
            {
                var enemyImage = enemyImages[i];
                double positionLeft = Canvas.GetLeft(enemyImage);
                double positionTop = Canvas.GetTop(enemyImage);

                int columnIndex = (int)Math.Round(positionLeft / tilePixelSize);
                int rowIndex = (int)Math.Round(positionTop / tilePixelSize);

                var lastDirection = enemyLastDirections[i];

                var possibleDirections = new List<Direction>(4);
                if (rowIndex - 1 >= 0 && mazeGrid[rowIndex - 1, columnIndex] != 1) possibleDirections.Add(Direction.Up);
                if (rowIndex + 1 < rowCount && mazeGrid[rowIndex + 1, columnIndex] != 1) possibleDirections.Add(Direction.Down);
                if (columnIndex - 1 >= 0 && mazeGrid[rowIndex, columnIndex - 1] != 1) possibleDirections.Add(Direction.Left);
                if (columnIndex + 1 < columnCount && mazeGrid[rowIndex, columnIndex + 1] != 1) possibleDirections.Add(Direction.Right);

                Direction oppositeDirection = GetOppositeDirection(lastDirection);
                if (lastDirection != Direction.None)
                {
                    possibleDirections.Remove(oppositeDirection);
                }

                if (possibleDirections.Count == 0)
                {
                    possibleDirections.Add(oppositeDirection);
                }

                var chosenDirection = possibleDirections[randomGenerator.Next(possibleDirections.Count)];
                int newColumnIndex = columnIndex, newRowIndex = rowIndex;
                switch (chosenDirection)
                {
                    case Direction.Up: newRowIndex = rowIndex - 1; break;
                    case Direction.Down: newRowIndex = rowIndex + 1; break;
                    case Direction.Left: newColumnIndex = columnIndex - 1; break;
                    case Direction.Right: newColumnIndex = columnIndex + 1; break;
                }

                Canvas.SetLeft(enemyImage, newColumnIndex * tilePixelSize);
                Canvas.SetTop(enemyImage, newRowIndex * tilePixelSize);
                enemyLastDirections[i] = chosenDirection;
            }
        }

        private static Direction GetOppositeDirection(Direction lastDirection)
        {
            return lastDirection switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.None,
            };
        }

        private void TryConsumeItems(int cellColumnIndex, int cellRowIndex)
        {
            // Pellet standard
            if (mazeGrid[cellRowIndex, cellColumnIndex] == 2)
            {
                mazeGrid[cellRowIndex, cellColumnIndex] = 0;
                currentScore += PelletPoints;

                if (pelletDictionary.TryGetValue((cellColumnIndex, cellRowIndex), out var pelletVisual))
                {
                    MazeCanvas.Children.Remove(pelletVisual);
                    pelletDictionary.Remove((cellColumnIndex, cellRowIndex));
                }

                UpdateScoreUI();

                if (pelletDictionary.Count == 0)
                {
                    ResetLevel();
                }
            }

            // Stéroïde
            if (steroidDictionary.TryGetValue((cellColumnIndex, cellRowIndex), out var steroidVisual))
            {
                MazeCanvas.Children.Remove(steroidVisual);
                steroidDictionary.Remove((cellColumnIndex, cellRowIndex));
                ActivatePowerMode();
            }
        }

        private void ActivatePowerMode()
        {
            isPowerModeActive = true;

            // Apparence "vulnérable" des chats
            foreach (var enemy in enemyImages)
            {
                enemy.Opacity = 0.7;
                enemy.Effect = vulnerableEnemyGlow;
            }

            // (Ré)initialise le compte à rebours
            powerModeTimer.Stop();
            powerModeTimer.Interval = PowerModeDuration;
            powerModeTimer.Start();
        }

        private void DeactivatePowerMode()
        {
            powerModeTimer.Stop();
            isPowerModeActive = false;

            foreach (var enemy in enemyImages)
            {
                enemy.Opacity = 1.0;
                enemy.Effect = null;
            }
        }

        private void UpdateScoreUI()
        {
            // Met à jour le score courant
            if (scoreDisplay != null)
            {
                scoreDisplay.Text = $"Score: {currentScore}";
            }

            // Gestion du meilleur score (high score)
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                if (bestScoreDisplay != null)
                {
                    bestScoreDisplay.Text = $"Meilleur: {bestScore}";
                }
            }
        }

        // Reset des pellets + stéroïdes et augmentation de vitesse, on garde la position du joueur
        private void ResetLevel()
        {
            // Pellets
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

            // Stéroïdes
            foreach (var (col, row) in initialSteroidCoordinates)
            {
                if (!steroidDictionary.ContainsKey((col, row)) && mazeGrid[row, col] != 1)
                {
                    var steroidVisual = CreateSteroidVisual(col, row);
                    MazeCanvas.Children.Add(steroidVisual);
                    steroidDictionary[(col, row)] = steroidVisual;
                }
            }

            // Augmente la vitesse
            if (gameLoopTimer != null)
            {
                double currentIntervalMs = gameLoopTimer.Interval.TotalMilliseconds;
                double newIntervalMs = Math.Max(MinimumMoveIntervalInMilliseconds, currentIntervalMs * SpeedIncreaseFactor);
                gameLoopTimer.Interval = TimeSpan.FromMilliseconds(newIntervalMs);

                try { MainWindow.MoveIntervalMs = (int)newIntervalMs; } catch { /* ignore */ }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
                Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
            }
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
                case Key.Up: currentMovementDirection = Direction.Up; break;
                case Key.Down: currentMovementDirection = Direction.Down; break;
                case Key.Left: currentMovementDirection = Direction.Left; break;
                case Key.Right: currentMovementDirection = Direction.Right; break;
            }
        }

        // Brushes/Effets
        private static LinearGradientBrush CreateWallGradientBrush()
        {
            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 1) };
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 127, 80), 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 160, 122), 0.45));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(255, 99, 71), 0.8));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(200, 70, 60), 1.0));
            return brush;
        }

        private static RadialGradientBrush CreatePelletGradientBrush()
        {
            var brush = new RadialGradientBrush { GradientOrigin = new Point(0.35, 0.35), Center = new Point(0.35, 0.35), RadiusX = 0.8, RadiusY = 0.8 };
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(220, 255, 255, 255), 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(160, 180, 220, 255), 0.6));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(110, 120, 180, 255), 1.0));
            return brush;
        }

        private static RadialGradientBrush CreateSteroidGradientBrush()
        {
            var brush = new RadialGradientBrush { GradientOrigin = new Point(0.35, 0.35), Center = new Point(0.35, 0.35), RadiusX = 0.85, RadiusY = 0.85 };
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 230, 255), 0.0));   // centre très clair
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(230, 200, 120, 255), 0.45));  // liseré doré/rose
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 160, 0, 255), 0.7));     // magenta/orangé
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(220, 120, 0, 200), 1.0));     // bord violet/rose foncé
            return brush;
        }
    }
}