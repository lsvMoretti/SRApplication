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
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public string Username { get; }
        public int UserId { get; }

        public HomeWindow(string username, int userId)
        {
            InitializeComponent();

            Username = username;
            UserId = userId;

            UsernameText.Content = $"Welcome {username}!";

            using Database database = new Database();

            User user = database.Users.Find(userId);

            if (user == null)
            {
                MessageBox.Show("An error occurred. Try re-logging.");

                LoginWindow login = new LoginWindow();

                login.Show();

                Close();

                return;
            }

            if (user.UserManagement)
            {
                MembersMenu.Visibility = Visibility.Visible;
            }

            if (user.DiscordUserSync)
            {
                MenuItem forceDiscordSync = new MenuItem
                {
                    Header = "Force Discord Sync",
                    Name = "DiscordSync"

                };
                forceDiscordSync.Click += DiscordSync_OnClick;

                MembersMenu.Items.Add(forceDiscordSync);
            }
        }

        private void CloseItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        private void LogoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButton.YesNo) !=
                MessageBoxResult.Yes) return;

            LoginWindow login = new LoginWindow();

            login.Show();

            Close();

            return;

        }

        private void SearchMember_OnClick(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void AddMember_OnClick(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void DiscordSync_OnClick(object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}
