using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSWMNGUI
{
    public partial class MainPage : Page
    {
        private PasswordManager _passwordManager = new PasswordManager();

        public MainPage()
        {
            InitializeComponent();
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            //// Open the Add Entry dialog
            //var addEntryDialog = new AddEntryDialog();
            //if (addEntryDialog.ShowDialog() == true)
            //{
            //    // Get the user input from the dialog
            //    string website = addEntryDialog.Website;
            //    string login = addEntryDialog.Login;
            //    string password = addEntryDialog.Password;

            //    // Add the entry to the database
            //    _passwordManager.AddEntry(website, login, password);
            //}
        }

        private void ViewEntries_Click(object sender, RoutedEventArgs e)
        {
            // Display entries in the ListBox
            var entries = _passwordManager.ViewEntries();
            EntriesListBox.ItemsSource = entries;
        }

        private void GenerateRandomPassword_Click(object sender, RoutedEventArgs e)
        {
            string randomPassword = PasswordManager.GeneratePassword(12);
            // Show the generated password in a message box
            MessageBox.Show($"Generated Password: {randomPassword}", "Random Password", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewPassword_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected entry from the ListBox
            //string selectedEntry = EntriesListBox.SelectedItem as string;
            //if (selectedEntry != null)
            //{
            //    // Split the selected entry to extract the website
            //    string[] parts = selectedEntry.Split(new[] { ", Login: " }, StringSplitOptions.None);
            //    string website = parts[0].Substring("Website: ".Length);

            //    // Retrieve the password for the selected website
            //    string password = _passwordManager.ViewPassword(website);

            //    // Show the password in a message box
            //    MessageBox.Show(password, "Password", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private void SearchEntry_Click(object sender, RoutedEventArgs e)
        {
            // Implement the search functionality
            // You can open a search dialog and perform a search based on user input
            // Display the search results in the ListBox
        }

        private void CopyPasswordToClipboard_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected entry from the ListBox
            //string selectedEntry = EntriesListBox.SelectedItem as string;
            //if (selectedEntry != null)
            //{
            //    // Split the selected entry to extract the password
            //    string[] parts = selectedEntry.Split(new[] { ", Login: " }, StringSplitOptions.None);
            //    string password = parts[1].Substring(" Password: ".Length);

            //    // Copy the password to the clipboard
            //    Clipboard.SetText(password);

            //    // Show a notification that the password has been copied
            //    MessageBox.Show("Password copied to clipboard.", "Copy Password", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private void ChangeMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            //// Implement a way to change the master password, possibly via a dialog
            //var changePasswordDialog = new ChangePasswordDialog();
            //if (changePasswordDialog.ShowDialog() == true)
            //{
            //    // Get the user input for changing the master password
            //    string currentPassword = changePasswordDialog.CurrentPassword;
            //    string newPassword = changePasswordDialog.NewPassword;

            //    // Call the PasswordManager method to change the master password
            //    _passwordManager.ChangeMasterPassword(currentPassword, newPassword);
            //}
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            // Implement sign out logic, e.g., navigate back to the login page
            NavigationService.Navigate(new LoginPage());
        }

        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
