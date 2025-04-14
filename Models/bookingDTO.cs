using System.ComponentModel.DataAnnotations;

namespace TURF_BOOKINGS.Models
{
    public class bookingDTO
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public List<string> TimeSlots { get; set; } = new List<string>();
    }
}
