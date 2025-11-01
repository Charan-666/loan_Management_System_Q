using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Authentication
{
    public class CustomerRegistrationDTO
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number must be a valid 10-digit Indian mobile number starting with 6-9")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Occupation is required")]
        [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
        public string Occupation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Annual income is required")]
        [Range(100000, 100000000, ErrorMessage = "Annual income must be between ₹1,00,000 and ₹10,00,00,000")]
        public decimal AnnualIncome { get; set; }

        [Range(300, 900, ErrorMessage = "Credit score must be between 300 and 900")]
        public decimal CreditScore { get; set; } = 650; // Default credit score

        [Required(ErrorMessage = "Home ownership status is required")]
        public HomeOwnershipStatus HomeOwnershipStatus { get; set; }
    }
}