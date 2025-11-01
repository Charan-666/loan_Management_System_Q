using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities
{
    public class PaymentTransaction
    {
        [Key]
        public int TransactionId { get; set; }
        // Link to the EMI for which this payment was made
        [Required]
        [ForeignKey(nameof(EMIPlan))]
        public int EMIId { get; set; }
        // Amount paid in this transaction
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        // Date of payment
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        // Payment method: UPI, NetBanking, Card, Cash
        [Required]
        [MaxLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.UPI; // UPI, NetBanking, Card, Cash
        // Optional transaction reference from payment gateway
        [MaxLength(100)]
        public string? TransactionReference { get; set; }
        // Status of the payment
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        // Timestamps
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        // Soft delete flag
        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey(nameof(LoanAccount))]
        public int LoanAccountId { get; set; }
    }
    public enum PaymentStatus
    {
        Pending, Success, Failed, Reversed
    }
    public enum PaymentMethod
    {
        UPI, NetBanking, Card, Cash
    }
}
