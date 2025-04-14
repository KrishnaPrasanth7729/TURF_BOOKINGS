using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TURF_BOOKINGS.Models
{
    public class BookingModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public List<string> TimeSlots { get; set; } = new List<string>();

        public string Email { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        public int isDeleted { get; set; } = 0;
    }
}
