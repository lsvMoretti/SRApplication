using System;
using System.Collections.Generic;
using System.IO;
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
using EnumsNET;
using UiApp.Tables;

namespace UiApp
{
    /// <summary>
    /// Interaction logic for ShowMembers.xaml
    /// </summary>
    public partial class ShowMembers : Window
    {
        public ShowMembers()
        {
            InitializeComponent();
            LoadMembersInfo();
        }

        private void LoadMembersInfo()
        {
            MembersGrid.ColumnDefinitions.Add(new ColumnDefinition{Width = new GridLength(10, GridUnitType.Star)});
            MembersGrid.ColumnDefinitions.Add(new ColumnDefinition{ Width = new GridLength(25, GridUnitType.Star) });
            MembersGrid.ColumnDefinitions.Add(new ColumnDefinition{ Width = new GridLength(25, GridUnitType.Star) });

            MembersGrid.ShowGridLines = true;

            using Database database = new Database();

            List<User> users = database.Users.OrderBy(x => x.Id).ToList();

            int r = 0;

            MembersGrid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });

            #region User ID Title

            TextBlock userIdTitleBlock = new TextBlock
            {
                Text = "User ID",
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.ExtraBold,
                TextAlignment = TextAlignment.Center
            };

            userIdTitleBlock.SetValue(Grid.ColumnProperty, 0);
            userIdTitleBlock.SetValue(Grid.RowProperty, r);
            MembersGrid.Children.Add(userIdTitleBlock);

            #endregion

            #region Username Title

            TextBlock usernameTitleBlock = new TextBlock
            {
                Text = "Username",
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.ExtraBold,
                TextAlignment = TextAlignment.Center
            };

            usernameTitleBlock.SetValue(Grid.ColumnProperty, 1);
            usernameTitleBlock.SetValue(Grid.RowProperty, r);
            MembersGrid.Children.Add(usernameTitleBlock);

            #endregion

            #region View Member Title

            TextBlock memberRoleBlock = new TextBlock
            {
                Text = "View",
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.ExtraBold,
                TextAlignment = TextAlignment.Center
            };

            memberRoleBlock.SetValue(Grid.ColumnProperty, 2);
            memberRoleBlock.SetValue(Grid.RowProperty, r);
            MembersGrid.Children.Add(memberRoleBlock);

            #endregion

            r += 1;

            foreach (User user in users)
            {
                MembersGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });

                #region User ID

                TextBlock userIdBlock = new TextBlock
                {
                    Text = user.Id.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5)

                };
                userIdBlock.SetValue(Grid.ColumnProperty, 0);
                userIdBlock.SetValue(Grid.RowProperty, r);
                MembersGrid.Children.Add(userIdBlock);

                #endregion

                #region Username

                TextBlock usernameBlock = new TextBlock
                {
                    Text = user.Username,
                    Foreground = Brushes.White,
                    FontSize = 16,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5)
                };
                usernameBlock.SetValue(Grid.ColumnProperty, 1);
                usernameBlock.SetValue(Grid.RowProperty, r);
                MembersGrid.Children.Add(usernameBlock);

                #endregion

                #region View User

                Button viewButton = new Button
                {
                    Background = Brushes.LightGray,
                    Foreground = Brushes.Black,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Content = "View",
                    Height = 36,
                    Width = 90,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10)
                };

                viewButton.Click += MemberViewButtonOnClick;

                viewButton.SetValue(Grid.ColumnProperty, 2);
                viewButton.SetValue(Grid.RowProperty, r);

                MembersGrid.Children.Add(viewButton);
                
                #endregion

                r += 1;
            }
        }

        private void MemberViewButtonOnClick(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void OnUserMemberStatusChanged(User user, int newMemberStatus)
        {
            if (!User.CurrentLoggedInUser.UserManagement)
            {
                InfoLabel.Content = $"You don't have access!";
                return;
            }

            MemberStatus memberStatus = (MemberStatus)newMemberStatus;

            if (User.CurrentLoggedInUser.MemberStatus < memberStatus)
            {
                InfoLabel.Content = $"You don't have access!";
                return;
            }

            using Database database = new Database();

            User member = database.Users.FirstOrDefault(x => x.Id == user.Id);

            if (member == null) return;

            member.MemberStatus = memberStatus;

            database.SaveChanges();

            if (User.CurrentLoggedInUser.Id == member.Id)
            {
                User.CurrentLoggedInUser = member;
            }

            InfoLabel.Content = $"You've updated {member.Username}'s user management role to {(memberStatus).AsString(EnumFormat.Description)}";
        }
    }
}
