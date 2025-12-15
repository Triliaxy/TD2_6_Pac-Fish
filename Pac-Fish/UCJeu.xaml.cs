using System;
using System.Collections.Generic;
using System.Linq;
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
        // Map des pellets pour accès O(1) lors de la consommation
        private readonly Dictionary<(int X, int Y), Ellipse> pelletMap = new();

        // Positions initiales des pellets (capturées au premier DrawMaze)
        private readonly List<(int X, int Y)> initialPelletPositions = new();

        // facteur de réduction de l'intervalle (ex: 0.9 = +10% de vitesse)
        private const double IntervalReductionFactor = 0.9;
        private const double MinIntervalMs = 30;

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

        private int tileSize = 30;

        private enum Direction { None, Up, Down, Left, Right } //ajout de l'option None pour ne pas bouger
        private Direction currentDirection = Direction.None; //direction actuelle du poisson
        private DispatcherTimer moveTimer; //timer pour gérer le déplacement

        // Nouveau : compteur de score et TextBlock pour l'afficher
        private int score = 0;
        private TextBlock scoreText;

        public UCJeu()
        {
            InitializeComponent();

            DrawMaze();

            // redimensionne et aligne le poisson sur la grille
            InitializePlayerSizeAndPosition();

            // configure le TextBlock de score
            scoreText = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 16,
                Text = "Score: 0"
            };
            // positionne le score en haut à gauche (ajustez si nécessaire)
            Canvas.SetLeft(scoreText, 6);
            Canvas.SetTop(scoreText, 6);
            MazeCanvas.Children.Add(scoreText);

            // configure le timer de déplacement selon la variable globale
            moveTimer = new DispatcherTimer();
            moveTimer.Interval = TimeSpan.FromMilliseconds(MainWindow.MoveIntervalMs);
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        //commenter la section ci-dessous

        void DrawMaze()
        {
            // sécurité : si le canvas n'est pas encore initialisé on quitte
            if (MazeCanvas == null) return;

            // Conserver les éléments déjà présents (imgPoisson, scoreText) : on n'efface rien globalement.
            // Dessine murs et pellets ; n'ajoute les positions initiales qu'une seule fois.
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
                        var pellet = CreateBubble(x, y);
                        MazeCanvas.Children.Add(pellet);

                        // Enregistre l'ellipse dans la map pour accès rapide
                        pelletMap[(x, y)] = pellet;

                        // Enregistre la position initiale si pas déjà connue
                        if (!initialPelletPositions.Contains((x, y)))
                        {
                            initialPelletPositions.Add((x, y));
                        }
                    }
                }
            }
        }

        private Ellipse CreateBubble(int x, int y)
        {
            // taille proportionnelle à la case
            double size = Math.Max(8, tileSize * 0.4);

            var bubble = new Ellipse
            {
                Width = size,
                Height = size,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Opacity = 0.95
            };

            // Remplissage radial pour effet "bulle"
            var brush = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.35, 0.35),
                Center = new Point(0.35, 0.35),
                RadiusX = 0.8,
                RadiusY = 0.8
            };
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(220, 255, 255, 255), 0.0));      // centre lumineux
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(160, 180, 220, 255), 0.6));      // léger bleu
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(110, 120, 180, 255), 1.0));      // bord bleuté
            bubble.Fill = brush;

            // Ombre légère pour donner du volume
            bubble.Effect = new DropShadowEffect
            {
                Color = Colors.LightSkyBlue,
                BlurRadius = 6,
                ShadowDepth = 0,
                Opacity = 0.5
            };

            // Centre la bulle dans la case
            double left = x * tileSize + (tileSize - size) / 2.0;
            double top = y * tileSize + (tileSize - size) / 2.0;
            Canvas.SetLeft(bubble, left);
            Canvas.SetTop(bubble, top);

            return bubble;
        }

        private void InitializePlayerSizeAndPosition()
        {
            // définit la taille du poisson à une case
            imgPoisson.Width = tileSize;
            imgPoisson.Height = tileSize;

            // Détermine une cellule de départ "en bas" (juste au-dessus de la dernière ligne de murs),
            // préférentiellement au centre ; si centre = mur, on recherche la cellule non-mur la plus proche.
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);

            int startRow = Math.Max(0, rows - 2); // ligne juste au-dessus du dernier mur
            int centerCol = cols / 2;

            int cellX = centerCol;
            int cellY = startRow;
            bool found = false;

            // Cherche sur startRow puis remonte si nécessaire
            for (int y = startRow; y >= 0 && !found; y--)
            {
                for (int offset = 0; offset <= cols / 2 && !found; offset++)
                {
                    int left = centerCol - offset;
                    int right = centerCol + offset;

                    if (left >= 0 && maze[y, left] != 1)
                    {
                        cellX = left;
                        cellY = y;
                        found = true;
                        break;
                    }

                    if (right < cols && maze[y, right] != 1)
                    {
                        cellX = right;
                        cellY = y;
                        found = true;
                        break;
                    }
                }
            }

            // Positionne le poisson exactement sur la cellule trouvée
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

        /*
        Timer de déplacement : déplace l'image du poisson selon la direction courante,
        vérifie si la nouvelle position correspond à une cellule contenant un pellet (2),
        gère la consommation du pellet via EatPelletAt (mise à 0 dans le tableau), la suppression graphique
        de l'ellipse correspondante et la mise à jour du score.

        Optimisation : on utilise pelletMap pour éviter les recherches LINQ dans MazeCanvas.Children.
        */
        private void MoveTimer_Tick(object? sender, EventArgs e)
        {
            // déplace une seule fois par tick si une direction est active
            if (currentDirection == Direction.None) return;

            // Récupère la position actuelle (en pixels). Si NaN, on considère 0.
            double left = Canvas.GetLeft(imgPoisson);
            double top = Canvas.GetTop(imgPoisson);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;

            // Convertit la position en coordonnées de cellule (colonnes / lignes)
            int currCellX = (int)Math.Round(left / tileSize);
            int currCellY = (int)Math.Round(top / tileSize);

            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);

            // Calcule la cellule cible selon la direction demandée
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

            // Gère le passage de tunnel horizontal : si on sort à gauche ou à droite,
            // autorise le wrap vers l'autre bord si la cellule côté opposé (même Y) n'est pas un mur.
            if ((targetCellX < 0 || targetCellX >= cols) && (currentDirection == Direction.Left || currentDirection == Direction.Right))
            {
                int wrapX = (targetCellX + cols) % cols;

                // n'autorise le wrap que si la cellule courante n'est pas un mur et la cellule wrap n'est pas un mur
                if (maze[currCellY, currCellX] != 1 && maze[currCellY, wrapX] != 1)
                {
                    targetCellX = wrapX;
                    targetCellY = currCellY;
                }
                else
                {
                    // sinon on ne bouge pas
                    return;
                }
            }

            // Vérifie bornes (après tentative de wrap). Si hors limites verticales ou autres cas, on ne bouge pas.
            if (targetCellY < 0 || targetCellY >= rows ||
                targetCellX < 0 || targetCellX >= cols)
            {
                // hors du labyrinthe : on ne bouge pas
                return;
            }

            // Collision : si la cellule cible est un mur (1), Steve ne bouge pas
            if (maze[targetCellY, targetCellX] == 1)
            {
                return;
            }

            // Autorisé : positionne Steve exactement sur la cellule cible
            Canvas.SetLeft(imgPoisson, targetCellX * tileSize);
            Canvas.SetTop(imgPoisson, targetCellY * tileSize);

            // Après déplacement : vérifie et mange un pellet éventuel
            EatPelletAt(targetCellX, targetCellY);
        }

        private void EatPelletAt(int cellX, int cellY)
        {
            if (maze[cellY, cellX] != 2) return;

            // Met à jour la donnée logique : plus de pellet dans la cellule
            maze[cellY, cellX] = 0;

            // Incrémente le score (valeur choisie : 10 par pellet)
            score += 10;

            // Retire l'ellipse du Canvas si elle existe dans la map
            if (pelletMap.TryGetValue((cellX, cellY), out var pellet))
            {
                MazeCanvas.Children.Remove(pellet);
                pelletMap.Remove((cellX, cellY));
            }

            // Mise à jour de l'affichage du score si le TextBlock existe
            if (scoreText != null)
            {
                scoreText.Text = $"Score: {score}";
            }

            // Si plus aucun pellet, on réinitialise la map de pellets et on augmente la vitesse
            if (pelletMap.Count == 0)
            {
                ResetPelletsAndIncreaseSpeed();
            }
        }

        private void ResetPelletsAndIncreaseSpeed()
        {
            // Remet la donnée logique et re-crée visuellement les pellets
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

            // Augmente la vitesse : réduit l'intervalle du timer (cap min)
            if (moveTimer != null)
            {
                double currentMs = moveTimer.Interval.TotalMilliseconds;
                double newMs = Math.Max(MinIntervalMs, currentMs * IntervalReductionFactor);
                moveTimer.Interval = TimeSpan.FromMilliseconds(newMs);

                // Optionnel : mettre à jour MainWindow.MoveIntervalMs si persisté globalement
                try
                {
                    MainWindow.MoveIntervalMs = (int)newMs;
                }
                catch
                {
                    // ignore si la propriété n'existe pas ou n'est pas accessible
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