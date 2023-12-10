﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

class PasswordManager
{
    private int currentUserId = -1;
    private string connectionString = "server=localhost;user=root;database=PasswordManagerDB;password=root;";
    private bool runApp = true;

    public void Run()
    {
        while (runApp)
        {
            if (currentUserId == -1)
            {
                UserAuthentication();
            }
            else
            {
                UserSession();
            }
        }
    }

    private void UserAuthentication()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            bool isAuthenticated = false;

            while (!isAuthenticated && runApp) // Check if runApp is still true
            {
                Console.WriteLine("\n1. Login\n2. Register\n3. Exit Application");
                Console.Write("Choose an option: ");
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        isAuthenticated = Login(connection);
                        break;
                    case "2":
                        Register(connection);
                        break;
                    case "3":
                        runApp = false; // Set runApp to false to exit the application
                        Console.WriteLine("Exiting application.");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
                        break;
                }
            }
        }
    }

    private void UserSession()
    {
        bool exitSession = false;

        while (!exitSession)
        {
            Console.WriteLine("\n1. Add Entry\n2. View Entries\n3. View Password\n4. Search Entry\n5. Generate Random Password\n6. Copy Password to Clipboard\n7. Sign Out\n8. Exit Application");
            Console.Write("Choose an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    AddEntry();
                    break;
                case "2":
                    ViewEntries();
                    break;
                case "3":
                    ViewPassword();
                    break;
                case "4":
                    SearchEntry();
                    break;
                case "5":
                    GenerateRandomPassword();
                    break;
                case "6":
                    CopyPasswordToClipboard();
                    break;
                case "7":
                    exitSession = true;
                    currentUserId = -1;
                    Console.WriteLine("You have been signed out.");
                    break;
                case "8":
                    exitSession = true;
                    runApp = false; // Set runApp to false to exit the application
                    Console.WriteLine("Exiting application.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private bool Login(MySqlConnection connection)
    {
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

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

    private void Register(MySqlConnection connection)
    {
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        string hashedPassword = HashPassword(password);

        using (var command = new MySqlCommand("INSERT INTO UserAccounts (Username, HashedPassword) VALUES (@username, @hashedPassword)", connection))
        {
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@hashedPassword", hashedPassword);

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("User registered successfully.");
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate entry
                {
                    Console.WriteLine("Username already exists. Please choose a different username.");
                }
                else
                {
                    Console.WriteLine("Error occurred during registration.");
                }
            }
        }
    }

    private void AddEntry()
    {
        Console.Write("Enter Website: ");
        string website = Console.ReadLine();
        Console.Write("Enter Login: ");
        string login = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = EncryptDecrypt(Console.ReadLine());

        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "INSERT INTO PasswordEntries (UserId, Website, Login, Password) VALUES (@userId, @website, @login, @password)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    private void ViewEntries()
    {
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
                        Console.WriteLine($"Website: {reader["Website"]}, Login: {reader["Login"]}");
                    }
                }
            }
        }
    }

    private void ViewPassword()
    {
        Console.Write("Enter Website: ");
        string website = Console.ReadLine();

        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Password FROM PasswordEntries WHERE UserId = @userId AND Website = @website";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    Console.WriteLine($"Password: {EncryptDecrypt(result.ToString())}");
                }
                else
                {
                    Console.WriteLine("Entry not found.");
                }
            }
        }
    }

    private void SearchEntry()
    {
        Console.Write("Enter search term (Website or Login): ");
        string searchTerm = Console.ReadLine().ToLower();

        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Website, Login FROM PasswordEntries WHERE UserId = @userId AND (LOWER(Website) LIKE @searchTerm OR LOWER(Login) LIKE @searchTerm)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Website: {reader["Website"]}, Login: {reader["Login"]}");
                    }
                }
            }
        }
    }

    private void GenerateRandomPassword()
    {
        Console.Write("Enter Website: ");
        string website = Console.ReadLine();
        Console.Write("Enter Login: ");
        string login = Console.ReadLine();

        string password = GeneratePassword(12);
        Console.WriteLine($"Generated Password: {password}");

        password = EncryptDecrypt(password);
        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "INSERT INTO PasswordEntries (UserId, Website, Login, Password) VALUES (@userId, @website, @login, @password)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    private void CopyPasswordToClipboard()
    {
        Console.Write("Enter Website: ");
        string website = Console.ReadLine();

        using (var connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT Password FROM PasswordEntries WHERE UserId = @userId AND Website = @website";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@website", website);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null)
                {
                    Clipboard.SetText(EncryptDecrypt(result.ToString()));
                    Console.WriteLine("Password copied to clipboard.");
                }
                else
                {
                    Console.WriteLine("Entry not found.");
                }
            }
        }
    }

    private string EncryptDecrypt(string text)
    {
        var key = 'K'; // Simple key for XOR (Not Secure)
        return new string(text.ToCharArray().Select(c => (char)(c ^ key)).ToArray());
    }

    private static string GeneratePassword(int length)
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

    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        PasswordManager manager = new PasswordManager();
        manager.Run();
    }
}