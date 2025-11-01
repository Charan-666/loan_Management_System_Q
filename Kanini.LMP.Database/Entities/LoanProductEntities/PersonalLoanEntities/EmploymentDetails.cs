using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities
{
    public class EmploymentDetails
    {
        [Key]
        public int EmploymentDetailsId { get; set; }
        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Employment information
        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Designation {  get; set; } = null!;
        [Required]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
        public int Experience { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string EmailId { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string OfficeAddress { get; set; } = null!;
    }
}