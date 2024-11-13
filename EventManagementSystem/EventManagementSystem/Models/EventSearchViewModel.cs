using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class EventSearchViewModel
    {
        [DataType(DataType.Date)]
        public DateTime DateEvent { get; set; }

      
    }
}
