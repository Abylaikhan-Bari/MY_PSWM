﻿using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManagerApp
{
    public partial class RegisterPage : Form
    {
        private string connectionString = "server=localhost;user=root;database=PasswordManagerDB;password=root;";

        public RegisterPage()
        {
            InitializeComponent();

            // Attach a click event handler to the "Register" button
            btnRegister.Click += BtnRegister_Click;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (Register(username, password))
            {
                MessageBox.Show("User registered successfully.");

                // Close the current RegisterPage form
                this.Close();

                // Create an instance of the LoginPage form and show it
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
            }
            else
            {
                MessageBox.Show("Registration failed. Please choose a different username.");
            }
        }

        private bool Register(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var command = new MySqlCommand("INSERT INTO UserAccounts (Username, HashedPassword) VALUES (@username, @hashedPassword)", connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                        command.ExecuteNonQuery();
                        return true; // Registration successful
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062) // Duplicate entry
                    {
                        MessageBox.Show("Registration failed. Username already exists.");
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Please try again later.");
                    }
                    return false; // Registration failed
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
}
