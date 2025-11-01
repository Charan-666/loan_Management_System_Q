using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // ADDED: Navigation property to the M:M Join Entity
        public ICollection<LoanApplicant> Applications { get; set; } = new List<LoanApplicant>();
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]

        public Gender Gender { get; set; }

        [Required]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string Occupation {  get; set; } = null!;
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualIncome { get; set; }
        [Required]
        [Range(0, 900)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal CreditScore { get; set; }

        [Required]
        public byte[] ProfileImage { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        // Computed field — not stored in DB
        [NotMapped]
        public int Age => DateTime.Today.Year - DateOfBirth.Year -
                         (DateTime.Today < DateOfBirth.ToDateTime(TimeOnly.MinValue).AddYears(DateTime.Today.Year - DateOfBirth.Year) ? 1 : 0);

        // [MaxLength(250)]
        // [DisplayName("Address")]
        // public string? Address { get; set; }

        // [MaxLength(100)]
        // [DisplayName("City")]
        // public string? City { get; set; }

        // [MaxLength(100)]
        // [DisplayName("State")]
        // public string? State { get; set; }

        // [MaxLength(10)]
        // [DisplayName("Postal Code")]
        // public string? PostalCode { get; set; }

        // public string AadhaarNumber { get; set; } = null!;

        // public string PANNumber { get; set; } = null!;



        // public byte[] PANCardImage { get; set; } = null!;

        // public byte[] AadhaarFrontImage { get; set; } = null!;

        // public byte[] AadhaarBackImage { get; set; } = null!;

        // public byte[] IncomeProofDocument {  get; set; } = null!;


    }
}
