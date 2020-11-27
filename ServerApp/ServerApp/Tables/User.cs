using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApp.Tables
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
        /// Current Member Role / Status
        /// </summary>
        public MemberStatus MemberStatus { get; set; }

        /// <summary>
        /// The current status of activity
        /// </summary>
        public Status Status { get; set; }

        public string Country { get; set; }

        /// <summary>
        /// Unique User ID of Discord
        /// </summary>
        public string DiscordId { get; set; }

        /// <summary>
        /// Unique Steam64 ID
        /// </summary>
        public string Steam64 { get; set; }

        /// <summary>
        /// Ability to manage users
        /// </summary>
        public bool UserManagement { get; set; }

        /// <summary>
        /// Ability to force discord sync
        /// </summary>
        public bool DiscordUserSync { get; set; }

        /// <summary>
        /// Admissions Team Member
        /// </summary>
        public bool Admissions { get; set; }

        /// <summary>
        /// Date of Birth
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Application Join Date
        /// </summary>
        public DateTime ApplicationDate { get; set; }

        /// <summary>
        /// Date of leaving clan
        /// </summary>
        public DateTime LeavingDate { get; set; }

        /// <summary>
        /// Pre-clan squad hours
        /// </summary>
        public int PreClanSquadHours { get; set; }

        /// <summary>
        /// Notes for / from Admissions Team
        /// </summary>
        public string AdmissionNotes { get; set; }

        /// <summary>
        /// Notes from Staff Team
        /// </summary>
        public string AdminNotes { get; set; }

        /// <summary>
        /// Application Question No.1
        /// </summary>
        public string AppQuestionOne { get; set; }

        /// <summary>
        /// Application Answer No.1
        /// </summary>
        public string AppAnswerOne { get; set; }

        /// <summary>
        /// Application Question No.2
        /// </summary>
        public string AppQuestionTwo { get; set; }

        /// <summary>
        /// Application Answer No.2
        /// </summary>
        public string AppAnswerTwo { get; set; }

        /// <summary>
        /// Application Question No.3
        /// </summary>
        public string AppQuestionThree { get; set; }

        /// <summary>
        /// Application Answer No.3
        /// </summary>
        public string AppAnswerThree { get; set; }

        /// <summary>
        /// Application Question No.4
        /// </summary>
        public string AppQuestionFour { get; set; }

        /// <summary>
        /// Application Answer No.4
        /// </summary>
        public string AppAnswerFour { get; set; }

        public int Age
        {
            get
            {
                DateTime now = DateTime.Today;
                int age = now.Year - BirthDate.Year;
                if (now < BirthDate.AddYears(age)) age--;

                return age;
            }
        }
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

    public enum Status
    {
        Active,
        LOA,
        MIA
    }
}