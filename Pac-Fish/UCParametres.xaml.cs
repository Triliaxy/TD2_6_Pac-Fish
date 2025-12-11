using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logique d'interaction pour UCParametres.xaml
    /// </summary>
    public partial class UCParametres : UserControl
    {
        public UCParametres()
        {
            InitializeComponent();
        }



        private void Textbox_codeTriche_GotFocus(object sender, RoutedEventArgs e) //fonction qui efface le contenu par défaut du textbox en cas de clique
        {
            if (Textbox_codeTriche.Text == "Entrez votre code") 
            {
                Textbox_codeTriche.Text = "";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) //faire les cheats codes ici
        {
            
            
            string codeEntre = Textbox_codeTriche.Text;
            if (codeEntre == "steve")
            {
                MainWindow.steve = true;
            }
        }
    
    }
}
