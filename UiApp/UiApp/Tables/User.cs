using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UiApp.Tables
{
    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UserManagement { get; set; }
        public bool DiscordUserSync { get; set; }
        public MemberStatus MemberStatus { get; set; }

        public static User CurrentLoggedInUser { get; set; }
        
    }

    public enum MemberStatus
    {
        [Description("Non Member")]
        NonMember = 0,
        [Description("Applicant - Pending")]
        ApplicantPending,
        [Description("Applicant")]
        Applicant,
        [Description("Recruit")]
        Recruit,
        [Description("Member")]
        Member,
        [Description("Senior Member")]
        SeniorMember,
        [Description("Staff")]
        Staff,
        [Description("Director")]
        Director,
        [Description("Founder")]
        Founder
    }
}