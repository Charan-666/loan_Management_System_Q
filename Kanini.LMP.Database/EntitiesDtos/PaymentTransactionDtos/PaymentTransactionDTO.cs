using Kanini.LMP.Database.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.PaymentTransaction
{
    public class PaymentTransactionDTO
    {
        /// <summary>
        /// Unique identifier for the payment transaction.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// ID of the EMI plan associated with this payment.
        /// </summary>
        [Required(ErrorMessage = "EMIId is required.")]
        public int EMIId { get; set; }

        /// <summary>
        /// ID of the associated Loan Account.
        /// </summary>
        [Required(ErrorMessage = "LoanAccountId is required.")]
        public int LoanAccountId { get; set; }

        /// <summary>
        /// Amount paid in this transaction.
        /// </summary>
        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Date and time the payment was made.
        /// </summary>
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Method used to make the payment (UPI, NetBanking, Card, Cash).
        /// </summary>
        [Required]
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Optional reference number or ID from payment gateway.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? TransactionReference { get; set; }

        /// <summary>
        /// Status of the payment (Pending, Success, Failed, Reversed).
        /// </summary>
        [Required]
        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus Status { get; set; }

        /// <summary>
        /// Timestamp when this record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when this record was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Indicates whether this transaction record is active (not deleted).
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Optional: EMI number or loan details for display in UI.
        /// </summary>
        public string? LoanAccountNumber { get; set; }

        /// <summary>
        /// Optional: Customer name for easier API response mapping.
        /// </summary>
        public string? CustomerName { get; set; }
    }

    // Reuse enums from entity layer for consistency
    public enum PaymentStatus
    {
        Pending, Success, Failed, Reversed
    }

    public enum PaymentMethod
    {
        UPI, NetBanking, Card, Cash
    }
}

