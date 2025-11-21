using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.CreditDtos
{
    public class EligibilityProfileRequest
    {
        [Required]
        public bool IsExistingBorrower { get; set; }

        // Required for new users, optional for existing users (use existing profile data)
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "PAN must be in format: ABCDE1234F (5 letters, 4 digits, 1 letter)")]
        public string? PAN { get; set; }

        [Range(18, 80)]
        public int? Age { get; set; }

        [Range(100000, 50000000)]
        public decimal? AnnualIncome { get; set; }

        [StringLength(100)]
        public string? Occupation { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        // Additional fields for existing borrowers only
        [Range(0, 50)]
        public int? ExperienceYears { get; set; }

        [StringLength(200)]
        public string? EmployerName { get; set; }

        [Range(0, 100000)]
        public decimal? MonthlyEMI { get; set; }

        [Range(0, 10000000)]
        public decimal? ExistingLoanAmount { get; set; }

        public int? PreviousLoanCount { get; set; }

        // Payment behavior details
        public int? OnTimePayments { get; set; }
        
        public int? LatePayments { get; set; }
        
        public int? MissedPayments { get; set; }
        
        public bool? HasDefaultHistory { get; set; }
        
        public int? DaysOverdueMax { get; set; } // Worst overdue period
    }
}