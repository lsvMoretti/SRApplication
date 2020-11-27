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
        private static int _currentRow = 0;

        public ShowMembers()
        {
            InitializeComponent();
            FormatGrid();
            LoadAllMembers();
        }

        private void FormatGrid()
        {
            MembersGrid.Children.Clear();

            MembersGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            MembersGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25, GridUnitType.Star) });
            MembersGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25, GridUnitType.Star) });

            MembersGrid.ShowGridLines = true;

            _currentRow = 0;

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
            userIdTitleBlock.SetValue(Grid.RowProperty, _currentRow);
            MembersGrid.Children.Add(userIdTitleBlock);

            #endregion User ID Title

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
            usernameTitleBlock.SetValue(Grid.RowProperty, _currentRow);
            MembersGrid.Children.Add(usernameTitleBlock);

            #endregion Username Title

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
            memberRoleBlock.SetValue(Grid.RowProperty, _currentRow);
            MembersGrid.Children.Add(memberRoleBlock);

            #endregion View Member Title

            _currentRow += 1;
        }

        private void LoadAllMembers()
        {
            MembersGrid.Children.Clear();
            _currentRow = 1;

            using Database database = new Database();

            List<User> users = database.Users.OrderBy(x => x.Id).ToList();

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
                userIdBlock.SetValue(Grid.RowProperty, _currentRow);
                MembersGrid.Children.Add(userIdBlock);

                #endregion User ID

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
                usernameBlock.SetValue(Grid.RowProperty, _currentRow);
                MembersGrid.Children.Add(usernameBlock);

                #endregion Username

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

                viewButton.Click += (sender, args) =>
                {
                    MemberInfo memberInfo = new MemberInfo(user);
                    memberInfo.Show();
                };

                viewButton.SetValue(Grid.ColumnProperty, 2);
                viewButton.SetValue(Grid.RowProperty, _currentRow);

                MembersGrid.Children.Add(viewButton);

                #endregion View User

                _currentRow += 1;
            }
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            SearchMembers();
        }

        private void SearchMembers()
        {
            if (!SearchBox.Text.Any())
            {
                LoadAllMembers();
                return;
            }

            MembersGrid.Children.Clear();
            _currentRow = 1;

            using Database database = new Database();

            List<User> users = database.Users.Where(x => x.Username.ToLower().Contains(SearchBox.Text.ToLower()))
                .ToList();

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
                userIdBlock.SetValue(Grid.RowProperty, _currentRow);
                MembersGrid.Children.Add(userIdBlock);

                #endregion User ID

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
                usernameBlock.SetValue(Grid.RowProperty, _currentRow);
                MembersGrid.Children.Add(usernameBlock);

                #endregion Username

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

                viewButton.Click += (sender, args) =>
                {
                    MemberInfo memberInfo = new MemberInfo(user);
                    memberInfo.Show();
                };

                viewButton.SetValue(Grid.ColumnProperty, 2);
                viewButton.SetValue(Grid.RowProperty, _currentRow);

                MembersGrid.Children.Add(viewButton);

                #endregion View User

                _currentRow += 1;
            }
        }

        private void SearchBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SearchMembers();
        }
    }
}