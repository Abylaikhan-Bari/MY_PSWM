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


namespace PSWMNGUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Login Page
            MainContentFrame.Navigate(new Uri("LoginPage.xaml", UriKind.Relative));
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Registration Page
            MainContentFrame.Navigate(new Uri("RegisterPage.xaml", UriKind.Relative));
        }

        private void MainPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Main Page
            MainContentFrame.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
        }
    }

}
