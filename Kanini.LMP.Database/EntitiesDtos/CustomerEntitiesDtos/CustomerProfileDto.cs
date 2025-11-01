using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class CustomerProfileDto
    {
        /// <summary>
        /// Unique identifier for the customer.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Associated UserId (link to User entity).
        /// </summary>
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        /// <summary>
        /// Customer's date of birth.
        /// </summary>
        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Gender of the customer.
        /// </summary>
        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        /// <summary>
        /// Customer’s phone number.
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// Customer’s occupation.
        /// </summary>
        [Required(ErrorMessage = "Occupation is required.")]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        /// <summary>
        /// Annual income of the customer.
        /// </summary>
        [Required(ErrorMessage = "Annual income is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Annual income must be positive.")]
        public decimal AnnualIncome { get; set; }

        /// <summary>
        /// Credit score (0–900 range).
        /// </summary>
        [Required(ErrorMessage = "Credit score is required.")]
        [Range(0, 900, ErrorMessage = "Credit score must be between 0 and 900.")]
        public decimal CreditScore { get; set; }

        /// <summary>
        /// Base64 string representation of profile image (instead of byte[] for API transfer).
        /// </summary>
        public string? ProfileImageBase64 { get; set; }

        /// <summary>
        /// Home ownership status (Owned, Rented, etc.)
        /// </summary>
        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        /// <summary>
        /// Timestamp for last profile update.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Computed customer age (optional, not stored in DB).
        /// </summary>
        public int Age { get; set; }

    }
}
