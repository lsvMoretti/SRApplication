using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MemberInfo.xaml
    /// </summary>
    public partial class MemberInfo : Window
    {
        private static User currentUser;

        public MemberInfo(User user)
        {
            InitializeComponent();

            currentUser = user;

            TextDecoration blueUnderline = new TextDecoration();
            blueUnderline.Pen = new Pen(Brushes.Blue, 1.5);
            blueUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            #region Left Side

            #region Nickname

            Nickname.Background = Grid.Background;
            Nickname.IsReadOnly = true;
            Nickname.BorderThickness = new Thickness(0);
            Nickname.FontWeight = FontWeights.Bold;
            Nickname.FontSize = 16;
            Nickname.Foreground = Brushes.White;
            Nickname.HorizontalContentAlignment = HorizontalAlignment.Center;
            Nickname.VerticalContentAlignment = VerticalAlignment.Center;
            Nickname.Text = $"Nickname: {user.Username}";

            #endregion Nickname

            #region User Id

            UserId.Background = Grid.Background;
            UserId.IsReadOnly = true;
            UserId.BorderThickness = new Thickness(0);
            UserId.FontSize = 14;
            UserId.Foreground = Brushes.White;
            UserId.HorizontalContentAlignment = HorizontalAlignment.Left;
            UserId.VerticalContentAlignment = VerticalAlignment.Center;
            UserId.Text = $"User ID: {user.Id}";

            #endregion User Id

            #region Clan Rank

            ClanRank.Background = Grid.Background;
            ClanRank.IsReadOnly = true;
            ClanRank.BorderThickness = new Thickness(0);
            ClanRank.FontSize = 14;
            ClanRank.Foreground = Brushes.White;
            ClanRank.HorizontalContentAlignment = HorizontalAlignment.Left;
            ClanRank.VerticalContentAlignment = VerticalAlignment.Center;
            ClanRank.Text = $"Rank: {user.MemberStatus.AsString(EnumFormat.Description)}";

            #endregion Clan Rank

            #region Active Status

            ActiveStatus.Background = Grid.Background;
            ActiveStatus.IsReadOnly = true;
            ActiveStatus.BorderThickness = new Thickness(0);
            ActiveStatus.FontSize = 14;
            ActiveStatus.Foreground = Brushes.White;
            ActiveStatus.HorizontalContentAlignment = HorizontalAlignment.Left;
            ActiveStatus.VerticalContentAlignment = VerticalAlignment.Center;
            ActiveStatus.Text = $"Activity Status: {user.Status.AsString(EnumFormat.Description)}";

            #endregion Active Status

            #region Country

            Country.Background = Grid.Background;
            Country.IsReadOnly = true;
            Country.BorderThickness = new Thickness(0);
            Country.FontSize = 14;
            Country.Foreground = Brushes.White;
            Country.HorizontalContentAlignment = HorizontalAlignment.Left;
            Country.VerticalContentAlignment = VerticalAlignment.Center;
            Country.Text = $"Country: {user.Country}";

            #endregion Country

            #region Applicant Date

            ApplicantDate.Background = Grid.Background;
            ApplicantDate.IsReadOnly = true;
            ApplicantDate.BorderThickness = new Thickness(0);
            ApplicantDate.FontSize = 14;
            ApplicantDate.Foreground = Brushes.White;
            ApplicantDate.HorizontalContentAlignment = HorizontalAlignment.Left;
            ApplicantDate.VerticalContentAlignment = VerticalAlignment.Center;
            ApplicantDate.Text = $"Applicant Date: {user.ApplicationDate:dd-MMM-yy}";

            #endregion Applicant Date

            #region Total Days

            double totalDays = (DateTime.Now - user.ApplicationDate).TotalDays;

            TotalDays.Background = Grid.Background;
            TotalDays.IsReadOnly = true;
            TotalDays.BorderThickness = new Thickness(0);
            TotalDays.FontSize = 14;
            TotalDays.Foreground = Brushes.White;
            TotalDays.HorizontalContentAlignment = HorizontalAlignment.Left;
            TotalDays.VerticalContentAlignment = VerticalAlignment.Center;
            TotalDays.Text = $"Total Days: {Math.Round(totalDays)}";

            if (totalDays < 60)
            {
                TotalDays.Background = Brushes.OrangeRed;
            }

            if (totalDays >= 60 && totalDays < 90)
            {
                TotalDays.Background = Brushes.Yellow;
            }

            #endregion Total Days

            #endregion Left Side

            #region Right Side

            #region Steam ID

            SteamId.Background = Grid.Background;
            SteamId.IsReadOnly = true;
            SteamId.BorderThickness = new Thickness(0);
            SteamId.FontSize = 14;
            SteamId.Foreground = Brushes.White;
            SteamId.HorizontalContentAlignment = HorizontalAlignment.Right;
            SteamId.VerticalContentAlignment = VerticalAlignment.Center;
            SteamId.Text = $"Steam 64ID: {user.Steam64}";

            #endregion Steam ID

            #region Steam Profile

            SteamProfile.Background = Grid.Background;
            SteamProfile.IsReadOnly = true;
            SteamProfile.BorderThickness = new Thickness(0);
            SteamProfile.FontSize = 14;
            SteamProfile.Foreground = Brushes.White;
            SteamProfile.HorizontalContentAlignment = HorizontalAlignment.Right;
            SteamProfile.VerticalContentAlignment = VerticalAlignment.Center;
            SteamProfile.Text = $"Steam Profile";
            SteamProfile.MouseDoubleClick += SteamProfileOnMouseDoubleClick;
            SteamProfile.TextDecorations.Add(blueUnderline);

            #endregion Steam Profile

            #region Community Ban Link

            CommunityBanLink.Background = Grid.Background;
            CommunityBanLink.IsReadOnly = true;
            CommunityBanLink.BorderThickness = new Thickness(0);
            CommunityBanLink.FontSize = 14;
            CommunityBanLink.Foreground = Brushes.White;
            CommunityBanLink.HorizontalContentAlignment = HorizontalAlignment.Right;
            CommunityBanLink.VerticalContentAlignment = VerticalAlignment.Center;
            CommunityBanLink.Text = $"Squad Community Ban List";
            CommunityBanLink.MouseDoubleClick += CommunityBanOnMouseDoubleClick;
            CommunityBanLink.TextDecorations.Add(blueUnderline);

            #endregion Community Ban Link

            #endregion Right Side
        }

        private void SteamProfileOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"http://steamcommunity.com/profiles/{currentUser.Steam64}/")
            {
                UseShellExecute = true,
                Verb = "open"
            });
            e.Handled = true;
        }

        private void CommunityBanOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"https://squad-community-ban-list.com/search/{currentUser.Steam64}/")
            {
                UseShellExecute = true,
                Verb = "open"
            });
            e.Handled = true;
        }
    }
}