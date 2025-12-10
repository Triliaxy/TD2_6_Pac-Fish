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

namespace Jeu_Poisson
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarrage();
        }

        public void AfficheDemarrage() //affiche la page de démarrage
        {
            UCDemarrage uc = new UCDemarrage();
            ZoneJeu.Content = uc;
            uc.butStart.Click += (sender, e) => AfficheAcceuil(); //quand on clique sur le bouton start, on affiche la page d'acceuil, comprendre la syntaxe de cette ligne
        }

        private void AfficheAcceuil()
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
        }
        private void AfficheChoixDifficulte(object sender, RoutedEventArgs e) //affiche la page de choix des difficultés
        {
            UCDifficultés uc = new UCDifficultés();
            ZoneJeu.Content = uc;
        }
        private void AfficheBoutique(object sender, RoutedEventArgs e) //affiche la page de la boutique
        {
            UCBoutique uc = new UCBoutique();
            ZoneJeu.Content = uc;
        }
        private void AfficheParametres(object sender, RoutedEventArgs e) //affiche la page des paramètres
        {
            UCParamètres uc = new UCParamètres();
            ZoneJeu.Content = uc;
        }
        
        private RoutedEventHandler Fermer() //ferme l'application
        {
            return (sender, e) => { Application.Current.Shutdown(); }; //trouver la signification de la ligne de pourquoi ça se ferme avec ça
        }


    }
}