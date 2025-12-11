using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour UCParamètres.xaml
    /// </summary>
    public partial class UCParamètres : UserControl
    {
        public UCParamètres()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string codeEntre = Textbox_codeTriche.Text;
            if (codeEntre == "steve")
            {
                Console.WriteLine("oui"); // à modifier avec la page debug
            }
        }

        private void Textbox_codeTriche_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Textbox_codeTriche.Text == "Entrez votre code")
            {
                Textbox_codeTriche.Text = "";
            }
        }
    }
}
