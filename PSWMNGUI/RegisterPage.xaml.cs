using System.Windows;
using System.Windows.Controls;

namespace PSWMNGUI
{
    public partial class RegisterPage : Page
    {
        private PasswordManager _passwordManager = new PasswordManager();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            _passwordManager.Register(username, password);
            MessageBox.Show("Registration Successful");
            // Additional logic after registration
        }

    }
}
