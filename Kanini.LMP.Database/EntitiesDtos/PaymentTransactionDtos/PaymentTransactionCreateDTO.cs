using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.PaymentTransaction
{
    public class PaymentTransactionCreateDTO
    {
        [Required(ErrorMessage = "EMIId is required.")]
        public int EMIId { get; set; }

        [Required(ErrorMessage = "LoanAccountId is required.")]
        public int LoanAccountId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? TransactionReference { get; set; }
    }
}
