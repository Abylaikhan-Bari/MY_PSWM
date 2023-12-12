using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

public class PasswordManager
{
    private string connectionString = "server=localhost;user=root;database=PasswordManagerDB;password=root;";
    private int currentUserId = -1;

    public bool Login(string username, string password)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
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
                    }
                }
            }
        }
        return false;
    }

    public bool Register(string username, string password)
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string hashedPassword = HashPassword(password);
                using (var command = new MySqlCommand("INSERT INTO UserAccounts (Username, HashedPassword) VALUES (@username, @hashedPassword)", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;
                }
            }
        }
        catch (MySqlException ex)
        {
            MessageBox.Show("Registration failed: " + ex.Message);
            return false;
        }
    }

    public void AddEntry(string website, string login, string password)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            string encryptedPassword = EncryptDecrypt(password);
            string query = "INSERT INTO PasswordEntries (UserId, Website, Login, Password) VALUES (@userId, @website, @login, @password)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", encryptedPassword);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public List<string> ViewEntries()
    {
        List<string> entries = new List<string>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Website, Login FROM PasswordEntries WHERE UserId = @userId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entries.Add($"Website: {reader["Website"]}, Login: {reader["Login"]}");
                    }
                }
            }
        }
        return entries;
    }

    public string ViewPassword(string website)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Login, Password FROM PasswordEntries WHERE UserId = @userId AND Website = @website";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string login = reader["Login"].ToString();
                        string encryptedPassword = reader["Password"].ToString();
                        return $"Login: {login}, Password: {EncryptDecrypt(encryptedPassword)}";
                    }
                }
            }
        }
        return "Entry not found.";
    }

    public void ChangeMasterPassword(string currentPassword, string newPassword)
    {
        string hashedCurrentPassword = HashPassword(currentPassword);
        string hashedNewPassword = HashPassword(newPassword);
        using (var connection = new MySqlConnection(connectionString))
        {
            string verifyQuery = "SELECT HashedPassword FROM UserAccounts WHERE UserId = @userId";
            using (var verifyCommand = new MySqlCommand(verifyQuery, connection))
            {
                verifyCommand.Parameters.AddWithValue("@userId", currentUserId);
                connection.Open();
                string storedHash = verifyCommand.ExecuteScalar()?.ToString();
                if (storedHash == hashedCurrentPassword)
                {
                    string updateQuery = "UPDATE UserAccounts SET HashedPassword = @hashedNewPassword WHERE UserId = @userId";
                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@hashedNewPassword", hashedNewPassword);
                        updateCommand.Parameters.AddWithValue("@userId", currentUserId);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    // Utility methods
    private string EncryptDecrypt(string text)
    {
        var key = 'K'; // Simple key for XOR (Not Secure)
        return new string(text.ToCharArray().Select(c => (char)(c ^ key)).ToArray());
    }

    public static string GeneratePassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        StringBuilder res = new StringBuilder();
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] uintBuffer = new byte[sizeof(uint)];
            while (length-- > 0)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(valid[(int)(num % (uint)valid.Length)]);
            }
        }
        return res.ToString();
    }
    public List<string> SearchEntry(string searchTerm)
    {
        List<string> foundEntries = new List<string>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Website, Login FROM PasswordEntries WHERE UserId = @userId AND (LOWER(Website) LIKE @searchTerm OR LOWER(Login) LIKE @searchTerm)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm.ToLower()}%");
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foundEntries.Add($"Website: {reader["Website"]}, Login: {reader["Login"]}");
                    }
                }
            }
        }
        return foundEntries;
    }

    public void CopyPasswordToClipboard(string website, string login)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Password FROM PasswordEntries WHERE UserId = @userId AND Website = @website AND Login = @login";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);
                command.Parameters.AddWithValue("@login", login);
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    string decryptedPassword = EncryptDecrypt(result.ToString());
                    Clipboard.SetText(decryptedPassword); // Copy to clipboard
                }
                else
                {
                    MessageBox.Show("Entry not found.");
                }
            }
        }
    }
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
