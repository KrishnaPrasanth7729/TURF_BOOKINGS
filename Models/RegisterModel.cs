using System.ComponentModel.DataAnnotations;


namespace TURF_BOOKINGS.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }


        [Key]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[a-z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email must be a valid gmail address.")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must start with 6, 7, 8, or 9 and be 10 digits long.")]
        public string? Phone { get; set; } 

    }
}
