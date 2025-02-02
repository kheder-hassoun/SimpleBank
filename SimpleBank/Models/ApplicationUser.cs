using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace SimpleBank.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        // Navigation property: Each user can have multiple accounts
        public ICollection<Account> Accounts { get; set; } = new List<Account>();


    }
}
