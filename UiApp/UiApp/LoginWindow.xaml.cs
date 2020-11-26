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
using UiApp.Tables;

namespace UiApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private async void AttemptLogin()
        {

            if (!UsernameBox.Text.Any() || !PasswordBox.Password.Any())
            {

                MessageLabel.Content = "You must enter a username or password!";
                return;
            }

            MessageLabel.Content = "Logging In...";

            await using Database database = new Database();

            if (!database.Users.Any())
            {
                string pass = BCrypt.Net.BCrypt.EnhancedHashPassword("test12");

                User unsociableUser = new User
                {
                    Username = "Unsociable",
                    Password = pass
                };

                await database.Users.AddAsync(unsociableUser);

                await database.SaveChangesAsync();
            }

            string username = UsernameBox.Text;

            User user = database.Users.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                MessageLabel.Content = "Unable to find this username!";
                return;
            }

            bool passMatch = BCrypt.Net.BCrypt.EnhancedVerify(PasswordBox.Password, user.Password);

            if (!passMatch)
            {
                MessageLabel.Content = "Incorrect Password!";
                return;
            }

            MessageLabel.Content = "Success! Please wait..";

            HomeWindow homeWindow = new HomeWindow(username, user.Id);
            homeWindow.Show();
            this.Close();
        }

        private void PasswordBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) AttemptLogin();
        }
    }
}
