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
    }
}