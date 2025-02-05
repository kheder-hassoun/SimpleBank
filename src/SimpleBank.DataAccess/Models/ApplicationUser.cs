using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace SimpleBank.DataAccess.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        // Navigation property: Each user can have multiple accounts
        public ICollection<Account> Accounts { get; set; } = new List<Account>();


        // New properties with validation attributes
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
