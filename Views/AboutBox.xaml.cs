/*
*   FILE          : AboutBox.xaml.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Code-behind for AboutBox dialog window.
*/
using System.Windows;

namespace D2ArmorCalc {
    public partial class AboutBox : Window {
        public AboutBox(){
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e){
            Close();
        }
    }
}