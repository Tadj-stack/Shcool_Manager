using System.Collections.Generic;
using System.Windows;

namespace CampusManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<prgram> mesProg = new List<prgram>();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            


        }


         class prgram
        {
            public string nomDuProg { get; set; }
            public string numDuprog { get; set; }
            public string dureeDuprog { get; set; }

        }
    }
}












