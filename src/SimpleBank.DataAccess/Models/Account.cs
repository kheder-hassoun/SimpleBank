using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SimpleBank.DataAccess.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        [MaxLength(20, ErrorMessage = "Account number cannot exceed 20 characters.")]
        public string AccountNumber { get; set; } // Unique account number

        [Required(ErrorMessage = "Account type is required.")]
        public int AccountTypeId { get; set; }

        [ForeignKey("AccountTypeId")]
        [ValidateNever]
        public AccountType AccountType { get; set; }

        [Required(ErrorMessage = "Balance is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative.")]
        public decimal Balance { get; set; }

        // Foreign key to ApplicationUser
        // [Required(ErrorMessage = "ApplicationUserId is required.")]
        public int ApplicationUserId { get; set; }
        [ValidateNever]
        //[ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        // Navigation property: Each account can have multiple transactions
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
