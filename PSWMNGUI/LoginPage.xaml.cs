using System.Windows;
using System.Windows.Controls;

namespace PSWMNGUI
{
    public partial class LoginPage : Page
    {
        private PasswordManager _passwordManager = new PasswordManager();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (_passwordManager.Login(username, password))
            {
                MessageBox.Show("Login Successful");
                // Additional logic for successful login
            }
            else
            {
                MessageBox.Show("Login Failed");
            }
        }

    }
}
