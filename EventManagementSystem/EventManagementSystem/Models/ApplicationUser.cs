using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string Surname { get; set; }
        [Required]

        public string Name { get; set; }
        [Required]

        public string Middlename { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();


    }
}
