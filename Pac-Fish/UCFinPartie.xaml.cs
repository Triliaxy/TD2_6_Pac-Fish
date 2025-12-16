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
    /// Logique d'interaction pour UCFinPartie.xaml
    /// </summary>
    public partial class UCFinPartie : UserControl
    {
        public UCFinPartie()
        {
            InitializeComponent();
        }

        private void BtnRetourMenu_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mw)
            {
                mw.RetourAuMenu();
            }
        }
    }
}
