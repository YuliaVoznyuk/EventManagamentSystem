using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class Speaker
    {
        public int Id { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Middlename { get; set; }
        [Required]
        public string Bio { get; set; }
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    }
}
