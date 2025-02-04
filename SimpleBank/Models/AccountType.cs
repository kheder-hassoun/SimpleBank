using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBank.Models
{
    public class AccountType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Account type is required.")]
        [MaxLength(50, ErrorMessage = "Account type cannot exceed 50 characters.")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "Maximum withdrawal amount per day is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Maximum withdrawal amount must be greater than zero.")]
        public decimal MaxWithdrawalPerDay { get; set; }

        [Required(ErrorMessage = "Minimum account balance is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Minimum account balance must be greater than zero.")]
        public decimal MinAccountBalance { get; set; }
    }
}