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
    /// Logique d'interaction pour UCDebug.xaml
    /// </summary>
    public partial class UCDebug : UserControl
    {
        public UCDebug()
        {
            InitializeComponent();
        }

        //private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    // Remet la lecture à zéro quand elle est finie
        //    ((MediaElement)sender).Position = TimeSpan.FromMilliseconds(1);
        //    ((MediaElement)sender).Play();
        //}

    }
}
