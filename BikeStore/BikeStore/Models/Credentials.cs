using System.ComponentModel.DataAnnotations;

namespace BikeStore.Models
{
    public class Credentials
    {
        [Required]
        public string Mail { get; set; }
        [Required]
        public string Password { get; set; }
        public string Name { get; set; }
        public int Id {  get; set; }

        public Credentials() { }

        public Credentials(string Mail, string Password, int id)
        {
            this.Mail = Mail;
            this.Password = Password;
            Id = id;
        }
    }
}
