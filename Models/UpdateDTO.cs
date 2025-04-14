using System.ComponentModel.DataAnnotations;

namespace TURF_BOOKINGS.Models
{
    public class UpdateDTO
    {

        public int Id { get; set; }



        [Required]
        public List<string> TimeSlots { get; set; } = new List<string>();
    }
}
