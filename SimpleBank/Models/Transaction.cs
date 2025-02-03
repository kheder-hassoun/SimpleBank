using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpleBank.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        [MaxLength(50, ErrorMessage = "Transaction type cannot exceed 50 characters.")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        // New fields
        [Display(Name = "Source Account")]
        [MaxLength(20, ErrorMessage = "Account number cannot exceed 20 characters.")]
        public string? SourceAccountNumber { get; set; }

        [Display(Name = "Destination Account")]
        [MaxLength(20, ErrorMessage = "Account number cannot exceed 20 characters.")]
        public string? DestinationAccountNumber { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; }


        // for sechudling

        [Display(Name = "Scheduled Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending/Completed/Failed


        // Foreign key to Account
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        public Account Account { get; set; }
    }
}