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

        public static HomeWindow Instance;
        public HomeWindow(string username, int userId)
        {
            InitializeComponent();

            Instance = this;

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

            User.CurrentLoggedInUser = user;

            if (user.UserManagement)
            {
                MenuItem membersMenuItem = new MenuItem
                {
                    Name = "MembersMenu",
                    Header = "Members"
                };

                MainMenu.Items.Add(membersMenuItem);

                MenuItem showMembersItem = new MenuItem
                {
                    Name = "ShowMembers",
                    Header = "Show Members"
                };

                membersMenuItem.Items.Add(showMembersItem);

                showMembersItem.Click += ShowMembers_OnClick;

                MenuItem addMemberItem = new MenuItem
                {
                    Name = "AddMember",
                    Header = "Add Member"
                };

                membersMenuItem.Items.Add(addMemberItem);

                addMemberItem.Click += AddMember_OnClick;

                if (user.Admissions)
                {
                    MenuItem admissionsMenuItem = new MenuItem
                    {
                        Name = "Admissions",
                        Header = "Admissions"
                    };

                    //TODO Add click event

                    membersMenuItem.Items.Add(admissionsMenuItem);
                }

                if (user.DiscordUserSync)
                {
                    MenuItem forceDiscordSyncItem = new MenuItem
                    {
                        Header = "Force Discord Sync",
                        Name = "DiscordSync"

                    };

                    forceDiscordSyncItem.Click += DiscordSync_OnClick;

                    membersMenuItem.Items.Add(forceDiscordSyncItem);
                }

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

        private void ShowMembers_OnClick(object sender, RoutedEventArgs e)
        {
            ShowMembers showMembers = new ShowMembers();

            showMembers.Show();
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
