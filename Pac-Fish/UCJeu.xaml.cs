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

        // Nouveau : compteur de score et TextBlock pour l'afficher
        private int score = 0;
        private TextBlock scoreText;

        public UCJeu()
        {
            InitializeComponent();

            //string nomFichierImage = $"pack://application:,,,/sprites/{MainWindow.Perso}.gif"; //plus utilisé 
            //imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

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
        gère la consommation du pellet (mise à 0 dans le tableau), la suppression graphique
        de l'ellipse correspondante et la mise à jour du score.
        
        PSEUDOCODE :
        - Si la direction actuelle est None : ne rien faire et sortir.
        - Récupérer la position actuelle en pixels de l'image (left, top).
          - Si NaN, remplacer par 0 pour éviter exceptions.
        - Appliquer un déplacement en pixels selon la direction et MainWindow.PasPoisson.
        - Après déplacement, récupérer la nouvelle position (newLeft, newTop).
        - Convertir la position en coordonnées de cellule :
          - cellX = arrondi(newLeft / tileSize)
          - cellY = arrondi(newTop / tileSize)
        - Vérifier que cellX/cellY sont dans les limites du tableau `maze`.
        - Si la cellule contient un pellet (valeur 2) :
          - Mettre `maze[cellY, cellX] = 0` pour marquer la pellet mangée.
          - Incrémenter `score` (ici +10).
          - Rechercher dans `MazeCanvas.Children` une `Ellipse` positionnée aux coordonnées attendues
            (on compare avec la position calculée utilisée lors du dessin : x * tileSize + 11, y * tileSize + 11).
            - Si trouvée, la supprimer de `MazeCanvas.Children`.
          - Mettre à jour le texte du `scoreText` si non null.
        - Notes d'implémentation :
          - Les comparaisons de position utilisent une tolérance faible (0.1) pour éviter les erreurs
            dues aux conversions en double.
          - On garde la logique de Round pour correspondre au placement par cellule.
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

            // Applique le déplacement selon la direction courante (utilise MainWindow.PasPoisson)
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

            // Récupère la nouvelle position après déplacement
            double newLeft = Canvas.GetLeft(imgPoisson);
            double newTop = Canvas.GetTop(imgPoisson);

            // Convertit la position en coordonnées de cellule (colonnes / lignes)
            int cellX = (int)Math.Round(newLeft / tileSize);
            int cellY = (int)Math.Round(newTop / tileSize);

            // Vérifie que les indices de cellule sont bien dans les bornes du tableau `maze`
            if (cellY >= 0 && cellY < maze.GetLength(0) && cellX >= 0 && cellX < maze.GetLength(1))
            {
                // Si la cellule contient un pellet (2), on le mange
                if (maze[cellY, cellX] == 2)
                {
                    // Met à jour la donnée logique : plus de pellet dans la cellule
                    maze[cellY, cellX] = 0;

                    // Incrémente le score (valeur choisie : 10 par pellet)
                    score += 10;

                    // Calcul des positions attendues de l'ellipse (mêmes offsets que lors du DrawMaze)
                    double expectedLeft = cellX * tileSize + 11;
                    double expectedTop = cellY * tileSize + 11;

                    // Recherche de l'ellipse correspondante dans le canvas.
                    // On utilise une tolérance pour éviter les faux négatifs dus aux conversions numériques.
                    const double tolerance = 0.1;
                    var pelletToRemove = MazeCanvas.Children
                        .OfType<Ellipse>()
                        .FirstOrDefault(el =>
                            Math.Abs(Canvas.GetLeft(el) - expectedLeft) < tolerance &&
                            Math.Abs(Canvas.GetTop(el) - expectedTop) < tolerance);

                    // Si trouvée, suppression graphique du pellet
                    if (pelletToRemove != null)
                    {
                        MazeCanvas.Children.Remove(pelletToRemove);
                    }

                    // Mise à jour de l'affichage du score si le TextBlock existe
                    if (scoreText != null)
                    {
                        scoreText.Text = $"Score: {score}";
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