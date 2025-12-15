using Pac_Fish;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public static bool steve { get; set; } = false; //var globale, get et set autorisent la modification et lecture partout
        public static string Perso { get; set; }
        public static int PasPoisson { get; set; } = 2;

        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = false;

            AfficheDemarrage();
        }

        private void AfficheDemarrage() //affiche la page de démarrage
        {
            UCDemarrage uc = new UCDemarrage();
            ZoneJeu.Content = uc;
            uc.butStart.Click += AfficheAcceuil; //quand on clique sur le bouton start, on affiche la page d'acceuil
        }

        private void AfficheAcceuil(object sender, RoutedEventArgs e) //affiche la page d'acceuil
        {
            UCacceuil uc = new UCacceuil();
            ZoneJeu.Content = uc;

            uc.butJouer.Click += AfficheChoixSkin; //quand on clique sur le bouton jouer, on affiche le choix des skins

            uc.butDiff.Click += AfficheChoixDifficulte; //quand on clique sur le bouton difficulté, on affiche le choix des difficultés

            uc.butBoutique.Click += AfficheBoutique; //quand on clique sur le bouton boutique, on affiche la boutique

            uc.butPara.Click += AfficheParametres; //quand on clique sur le bouton paramètres, on affiche les paramètres

            uc.butQuit.Click += Fermer(); //quand on clique sur le bouton quitter, on ferme l'application
        }

        private void AfficheChoixSkin(object sender, RoutedEventArgs e) //affiche la page de choix des skins
        {
            UCChoixSkin uc = new UCChoixSkin();
            ZoneJeu.Content = uc;

            uc.butRetourSkin.Click += AfficheAcceuil; //quand on clique sur le bouton retour, on affiche la page d'acceuil

            uc.ButLaunch.Click += AfficheJeu; //quand on clique sur le bouton lancer, on affiche le jeu
        }

        private void AfficheJeu(object sender, RoutedEventArgs e) //affiche la page du jeu
        {
            UCJeu uc = new UCJeu();
            ZoneJeu.Content = uc;
        }

        private void AfficheChoixDifficulte(object sender, RoutedEventArgs e) //affiche la page de choix des difficultés
        {
            UCDifficultes uc = new UCDifficultes();
            ZoneJeu.Content = uc;

            uc.butRetourDiff.Click += AfficheAcceuil; //quand on clique sur le bouton retour, on affiche la page d'acceuil
            uc.butValiderDiff.Click += AfficheAcceuil; //quand on clique sur le bouton valider, on affiche la page d'acceuil
        }
        private void AfficheBoutique(object sender, RoutedEventArgs e) //affiche la page de la boutique
        {
            UCBoutique uc = new UCBoutique();
            ZoneJeu.Content = uc;

            uc.butRetourBoutique.Click += AfficheAcceuil; //quand on clique sur le bouton retour, on affiche la page d'acceuil
        }
        private void AfficheParametres(object sender, RoutedEventArgs e) //affiche la page des paramètres
        {
            UCParametres uc = new UCParametres();
            ZoneJeu.Content = uc;

            uc.butRetourPara.Click += AfficheAcceuil; //quand on clique sur le bouton retour, on affiche la page d'acceuil
            uc.butValiderPara.Click += AfficheAcceuil; //quand on clique sur le bouton valider, on affiche la page d'acceuil
            uc.Btn_utiliser.Click += AfficheDebug; //quand on clique sur le bouton utiliser, on affiche la page de debug
        }
        
        private void AfficheDebug(object sender, RoutedEventArgs e) //affiche la page de debug
        {
            //faire la vérification du code de triche ici avant d'afficher la page de debug
            if (MainWindow.steve)
            {
                UCDebug uc = new UCDebug();
                ZoneJeu.Content = uc;
            }
        }

        private RoutedEventHandler Fermer() //ferme l'application
        {
            return (sender, e) => { Application.Current.Shutdown(); }; //trouver la signification de la ligne de pourquoi ça se ferme avec ça
        }

        
    }
}