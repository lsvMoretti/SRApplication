using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UiApp.Tables
{
    [Table("user")]
    public class User
    {
        /// <summary>
        /// Unique MySQL ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Discord Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Unique UI Login Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Ability to manage users
        /// </summary>
        public bool UserManagement { get; set; }
        /// <summary>
        /// Ability to force discord sync
        /// </summary>
        public bool DiscordUserSync { get; set; }
        /// <summary>
        /// Current Member Role / Status
        /// </summary>
        public MemberStatus MemberStatus { get; set; }
        /// <summary>
        /// Admissions Team Member
        /// </summary>
        public bool Admissions { get; set; }

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