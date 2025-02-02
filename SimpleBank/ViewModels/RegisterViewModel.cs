using System.ComponentModel.DataAnnotations;

namespace SimpleBank.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City name cannot exceed 100 characters.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "PIN code is required.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN code must be exactly 4 digits.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN code must be numeric and exactly 4 digits.")]
        [Display(Name = "PIN Code")]
        public string PinCode { get; set; } = "0000"; // Default value
    }
}
