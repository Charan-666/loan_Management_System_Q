using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.PaymentTransaction
{
    public class PaymentTransactionResponseDTO
    {
        public int TransactionId { get; set; }
        public int EMIId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionReference { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
