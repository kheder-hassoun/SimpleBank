using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace SimpleBank.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        [MaxLength(50, ErrorMessage = "Transaction type cannot exceed 50 characters.")]
        public string TransactionType { get; set; } // e.g., Deposit, Withdrawal

        [Required(ErrorMessage = "Amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        // Foreign key to Account
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        //[ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}
