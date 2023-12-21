using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using DatabseInsertApp;
using System.Configuration;

namespace PasswordManagerApp
{
    public partial class LoginPage : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int currentUserId = -1;

        public LoginPage()
        {
            InitializeComponent();
            passwordTextBox.PasswordChar = '•';
            // Wire up button click event handlers
            button1.Click += button1_Click; // Login button

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = loginTextBox.Text;
            string password = passwordTextBox.Text;

            // Create a MySQL connection and command
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM passwordmanagerdb.useraccounts WHERE username = @username AND HashedPassword = @hashedpassword"; // Updated query
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@hashedpassword", HashPassword(password)); // Provide the hashed password value

                    // Execute the query
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // User found, navigate to MainPage
                            MainPage mainPage = new MainPage(username);
                            mainPage.Show();
                            this.Hide();
                        }
                        else
                        {
                            // User not found, show an error message
                            MessageBox.Show("Invalid username or password. Please try again.");
                        }
                    }
                }
            }
        }



       

        private bool Login(MySqlConnection connection, string username, string password)
        {
            using (var command = new MySqlCommand("SELECT UserId, HashedPassword FROM UserAccounts WHERE Username = @username", connection))
            {
                command.Parameters.AddWithValue("@username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHash = reader["HashedPassword"].ToString();
                        if (HashPassword(password) == storedHash)
                        {
                            currentUserId = Convert.ToInt32(reader["UserId"]);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect password.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Username not found.");
                    }
                }
            }
            return false;
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void lblToRegisterPage_Click(object sender, EventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            registerPage.Show();
            this.Hide();
        }

        private void ChkShowPassword2_CheckedChanged(object sender, EventArgs e)
        {
            passwordTextBox.PasswordChar = ChkShowPassword2.Checked ? '\0' : '•';
        }
    }
}
