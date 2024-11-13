using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace EventManagementSystem.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        [Required]
        public Category Category { get; set; }
        [Required]
        public Format Format { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Speaker> Speakers { get; set; } = new List<Speaker>();

        public string? OnlineLink { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }


    }
}
