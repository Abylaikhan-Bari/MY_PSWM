using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using PasswordManagerApp;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace DatabseInsertApp
{
    public partial class MainPage : Form
    {
        private MySqlConnection con;
        private string currentUsername; // Store the current logged-in username

        public MainPage(string username)
        {
            InitializeComponent();
            this.currentUsername = username;
            con = new MySqlConnection("server=localhost;user=root;database=PasswordManagerDB;password=root;");
            DisplayData();
            lblCurrentUser.Text = "Current User: " + username;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Ensure full row selection
        }

        private bool IsEntryExists(string website, string login)
        {
            using (var connection = new MySqlConnection(con.ConnectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM PasswordEntries WHERE UserId = @userId AND Website = @website AND Login = @login";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", GetCurrentUserId());
                    command.Parameters.AddWithValue("@website", website);
                    command.Parameters.AddWithValue("@login", login);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }

        }
        private void btnAddEntry_Click(object sender, EventArgs e)
        {
            string website = txtWebsite.Text;
            string login = txtLogin.Text;
            string password = EncryptDecrypt(txtPassword.Text);

            if (string.IsNullOrWhiteSpace(website) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (IsEntryExists(website, login))
            {
                var confirmResult = MessageBox.Show("An entry with the same login already exists for this website. Do you still want to add it?", "Confirm Add", MessageBoxButtons.YesNo);
                if (confirmResult != DialogResult.Yes)
                {
                    return; // User chose not to add the entry
                }
            }

            InsertEntry(website, login, password);
            DisplayData();
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
                using (var connection = new MySqlConnection(con.ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO PasswordEntries (UserId, Website, Login, Password) VALUES (@userId, @website, @login, @password)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", GetCurrentUserId());
                        command.Parameters.AddWithValue("@website", website);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void DisplayData()
        {
            if (con != null)
            {
                using (var connection = new MySqlConnection(con.ConnectionString))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    string query = "SELECT entryid, website, login, password FROM passwordmanagerdb.passwordentries WHERE UserId = @UserId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                        using (var adapt = new MySqlDataAdapter(cmd))
                        {
                            adapt.Fill(dt);
                            dataGridView1.DataSource = dt;

                            // Hide the EntryId and Password columns
                            if (dataGridView1.Columns["entryid"] != null)
                            {
                                dataGridView1.Columns["entryid"].Visible = false;
                            }
                            if (dataGridView1.Columns["password"] != null)
                            {
                                dataGridView1.Columns["password"].Visible = false;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("MySQL Connection is null.");
            }
        }



        private int GetCurrentUserId()
        {
            int userId = -1;
            using (var connection = new MySqlConnection(con.ConnectionString))
            {
                connection.Open();
                using (var cmd = new MySqlCommand("SELECT UserId FROM passwordmanagerdb.useraccounts WHERE Username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", currentUsername);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader.GetInt32(0);
                        }
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

        

        private void btnCloseApp_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnSignOut_Click(object sender, EventArgs e)
        {
            LoginPage loginForm = new LoginPage();
            loginForm.Show();
            this.Close(); // Close the current MainPage form
        }

        // Implement the logic for these event handlers as per your application requirements
        private void btnSearchEntry_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                DisplayData(); // Reload all data if the search term is empty
                return;
            }

            using (var connection = new MySqlConnection(con.ConnectionString))
            {
                connection.Open();
                DataTable dt = new DataTable();
                // Modify the query to search in the website or login columns
                string query = "SELECT entryid, website, login FROM passwordmanagerdb.passwordentries WHERE UserId = @UserId AND (LOWER(website) LIKE @searchTerm OR LOWER(login) LIKE @searchTerm)";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                    using (var adapt = new MySqlDataAdapter(cmd))
                    {
                        adapt.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }

            // Hide the EntryId column
            if (dataGridView1.Columns["entryid"] != null)
            {
                dataGridView1.Columns["entryid"].Visible = false;
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void btnChangeMasterPassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtChangeMasterPassword.Text;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Please enter a new password.");
                return;
            }

            string hashedNewPassword = HashPassword(newPassword);

            try
            {
                using (var connection = new MySqlConnection(con.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE UserAccounts SET HashedPassword = @hashedPassword WHERE UserId = @userId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@hashedPassword", hashedNewPassword);
                        command.Parameters.AddWithValue("@userId", GetCurrentUserId());

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Master password changed successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to change master password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void btnViewPassword_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an entry from the list to view its password.");
                return;
            }

            // Assuming the password is in the third column. Adjust the index as necessary.
            string encryptedPassword = dataGridView1.SelectedRows[0].Cells["Password"].Value.ToString();
            string decryptedPassword = EncryptDecrypt(encryptedPassword);

            MessageBox.Show("The password for the selected entry is: " + decryptedPassword, "Password");
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows[e.RowIndex].Selected = true;

            txtWebsite.Text = dataGridView1.Rows[e.RowIndex].Cells["Website"].Value.ToString(); // Adjust column name if different
            txtLogin.Text = dataGridView1.Rows[e.RowIndex].Cells["Login"].Value.ToString(); // Adjust column name if different
            txtPassword.Text = EncryptDecrypt(dataGridView1.Rows[e.RowIndex].Cells["Password"].Value.ToString()); // Adjust column name if different
        }

        private void btnUpdateEntry_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an entry from the list to update.");
                return;
            }

            int entryId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["EntryId"].Value); // Adjust column name if different
            string newWebsite = txtWebsite.Text;
            string newLogin = txtLogin.Text;
            string newPassword = txtPassword.Text;

            UpdateEntry(entryId, newWebsite, newLogin, newPassword);
            DisplayData();
        }

        private void UpdateEntry(int entryId, string newWebsite, string newLogin, string newPassword)
        {
            try
            {
                using (var connection = new MySqlConnection(con.ConnectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("", connection);
                    StringBuilder query = new StringBuilder("UPDATE PasswordEntries SET ");
                    bool needComma = false;

                    if (!string.IsNullOrWhiteSpace(newWebsite))
                    {
                        query.Append("Website = @website");
                        command.Parameters.AddWithValue("@website", newWebsite);
                        needComma = true;
                    }

                    if (!string.IsNullOrWhiteSpace(newLogin))
                    {
                        if (needComma) query.Append(", ");
                        query.Append("Login = @login");
                        command.Parameters.AddWithValue("@login", newLogin);
                        needComma = true;
                    }

                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        if (needComma) query.Append(", ");
                        query.Append("Password = @password");
                        command.Parameters.AddWithValue("@password", EncryptDecrypt(newPassword));
                    }

                    query.Append(" WHERE EntryId = @entryId AND UserId = @userId");
                    command.CommandText = query.ToString();
                    command.Parameters.AddWithValue("@entryId", entryId);
                    command.Parameters.AddWithValue("@userId", GetCurrentUserId());

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Entry updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No entry found with the specified ID for the current user.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the entry: {ex.Message}");
            }
        }

        private static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();

            while (0 < length--)
            {
                res.Append(validChars[rnd.Next(validChars.Length)]);
            }

            return res.ToString();
        }

        private void btnGenerateRandom_Click(object sender, EventArgs e)
        {
            int passwordLength = 12; // You can set this to your desired length
            string randomPassword = GenerateRandomPassword(passwordLength);
            txtPassword.Text = randomPassword; // Display the generated password in the password text box
        }

        private void btnCopyPassword_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an entry from the list to copy its password.");
                return;
            }

            // Assuming the password is in a column named "password"
            string encryptedPassword = dataGridView1.SelectedRows[0].Cells["password"].Value.ToString();
            string decryptedPassword = EncryptDecrypt(encryptedPassword);

            Clipboard.SetText(decryptedPassword);
            MessageBox.Show("Password copied to clipboard.");
        }


        private void btnDeleteEntry_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an entry from the list to delete.");
                return;
            }

            // Assuming the first cell in each row is the EntryId
            int entryId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["entryid"].Value);

            var confirmResult = MessageBox.Show("Are you sure you want to delete this entry?",
                                                 "Confirm Delete",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                DeleteEntry(entryId);
                DisplayData();
            }
        }

        private void DeleteEntry(int entryId)
        {
            try
            {
                using (var connection = new MySqlConnection(con.ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM PasswordEntries WHERE EntryId = @entryId AND UserId = @userId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@entryId", entryId);
                        command.Parameters.AddWithValue("@userId", GetCurrentUserId());

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Entry deleted successfully.");
                        }
                        else
                        {
                            MessageBox.Show("No entry found with the specified ID for the current user.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the entry: {ex.Message}");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clearing all the textboxes
            txtWebsite.Text = "";
            txtLogin.Text = "";
            txtPassword.Text = "";
            txtSearch.Text = "";


        }

        private void btnExportDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Query the database to retrieve data for the current user's entries
                List<PasswordEntry> entriesToExport = RetrieveEntriesForCurrentUser();

                // Serialize the retrieved data to JSON format
                string jsonData = SerializeToJson(entriesToExport);

                // Specify the file path where you want to save the JSON data
                string filePath = "C:\\Users\\Abylaikhan Bari\\source\\repos\\PasswordManagerApp\\PasswordManagerApp\\ExportedData\\exported_data.json";

                // Save the JSON data to the specified file
                File.WriteAllText(filePath, jsonData);

                MessageBox.Show("Data exported successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}");
            }
        }


        private List<PasswordEntry> RetrieveEntriesForCurrentUser()
        {
            List<PasswordEntry> entries = new List<PasswordEntry>();

            using (var connection = new MySqlConnection(con.ConnectionString))
            {
                connection.Open();
                string query = "SELECT Website, Login, Password FROM PasswordEntries WHERE UserId = @userId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", GetCurrentUserId());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string website = reader.GetString(0);
                            string login = reader.GetString(1);
                            string password = reader.GetString(2);

                            // Create a PasswordEntry object and add it to the list
                            entries.Add(new PasswordEntry { Website = website, Login = login, Password = password });
                        }
                    }
                }
            }

            return entries;
        }

        private string SerializeToJson(List<PasswordEntry> entries)
        {
            // Serialize the list of PasswordEntry objects to JSON format
            string jsonData = JsonConvert.SerializeObject(entries, Formatting.Indented);
            return jsonData;
        }

        //private string EncryptDecrypt(string text, char xorKey)
        //{
        //    // Implement XOR encryption/decryption logic
        //    var encryptedText = new string(text.ToCharArray().Select(c => (char)(c ^ xorKey)).ToArray());
        //    return encryptedText;
        //}

        //private void SaveEncryptedDataToFile(string encryptedData)
        //{
        //    // Specify the file path where you want to save the encrypted data (e.g., "exported_data.json")
        //    string filePath = "C:\\Users\\Abylaikhan Bari\\source\\repos\\PasswordManagerApp\\PasswordManagerApp\\ExportedData\\exported_data.json";

        //    // Save the encrypted data to the specified file
        //    File.WriteAllText(filePath, encryptedData);
        //}

        // Define the PasswordEntry class to represent the data structure
        public class PasswordEntry
        {
            public string Website { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
        }

    }
}
