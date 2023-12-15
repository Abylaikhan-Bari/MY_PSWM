using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using PasswordManagerApp;

namespace DatabseInsertApp
{
    public partial class MainPage : Form
    {
        private MySqlConnection con;
        private MySqlCommand cmd;
        private MySqlDataAdapter adapt;
        private string currentUsername; // Store the current logged-in username
        private LoginPage loginForm; // Reference to the login form

        public MainPage(string username)
        {
            InitializeComponent();
            this.currentUsername = username;
            string connectionString = "server=localhost;user=root;database=PasswordManagerDB;password=root;";
            con = new MySqlConnection(connectionString);
            DisplayData();
            lblCurrentUser.Text = "Current User: " + username;
        }


        private string connectionString = "server=localhost;user=root;database=PasswordManagerDB;password=root;";




        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            // Get data from text boxes
            string website = txtWebsite.Text;
            string login = txtLogin.Text;
            string password = EncryptDecrypt(txtPassword.Text);

            // Insert the entry into the database
            InsertEntry(website, login, password);

            // Refresh the data grid view
            DisplayData();

            // Clear the text boxes
            ClearData();
        }
        private string EncryptDecrypt(string text)
        {
            var key = 'K'; // Simple key for XOR (Not Secure)
            return new string(text.ToCharArray().Select(c => (char)(c ^ key)).ToArray());
        }

        private void InsertEntry(string website, string login, string password)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO PasswordEntries (UserId, Website, Login, Password) VALUES (@userId, @website, @login, @password)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", GetCurrentUserId(currentUsername));
                        command.Parameters.AddWithValue("@website", website);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Entry added successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }





        private void btnSearchEntry_Click(object sender, EventArgs e)
        {
            // Your Search Entry code here
        }

        private void btnChangeMasterPassword_Click(object sender, EventArgs e)
        {
            // Your Change Master Password code here
        }

        private void btnSignOut_Click(object sender, EventArgs e)
        {
            LoginPage loginForm = new LoginPage();
            loginForm.Show();
            this.Close(); // Close the current MainPage form
        }



        private void DisplayData()
        {
            if (con != null)
            {
                con.Open();
                DataTable dt = new DataTable();

                // Modify your SQL query to select entries for the current user
                string query = "SELECT website, login FROM passwordmanagerdb.passwordentries WHERE UserId = @UserId";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId(currentUsername));

                    using (MySqlDataAdapter adapt = new MySqlDataAdapter(cmd))
                    {
                        adapt.Fill(dt);

                        if (dataGridView1 != null)
                        {
                            dataGridView1.DataSource = dt;
                        }
                        else
                        {
                            MessageBox.Show("DataGridView is null.");
                        }
                    }
                }

                con.Close();
            }
            else
            {
                MessageBox.Show("MySQL Connection (con) is null.");
            }
        }

        // Helper method to get the UserId for the current username
        private int GetCurrentUserId(string username)
        {
            int userId = -1; // Initialize to an invalid value

            using (MySqlCommand cmd = new MySqlCommand("SELECT UserId FROM passwordmanagerdb.useraccounts WHERE Username = @username", con))
            {
                cmd.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userId = reader.GetInt32(0); // Get the UserId from the result
                    }
                }
            }

            return userId;
        }





        




        private void ClearData()
        {
            txtWebsite.Text = "";
            txtLogin.Text = "";
            txtPassword.Text = "";
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            txtWebsite.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtLogin.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtPassword.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
        }

        private void btnCloseApp_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnViewPassword_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdateEntry_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerateRandom_Click(object sender, EventArgs e)
        {

        }

        private void btnCopyPassword_Click(object sender, EventArgs e)
        {

        }
    }
}
