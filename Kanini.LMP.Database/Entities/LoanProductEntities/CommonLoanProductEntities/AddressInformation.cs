using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class AddressInformation
    {
        [Key]
        public int AddressInformationId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        // FK to User
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        // Residential and permanent addresses
        [Required]
        [MaxLength(250)]
        public string PresentAddress { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string PermanentAddress { get; set; } = null!;

        // Location details
        [Required]
        [MaxLength(100)]
        public string District {  get; set; } = null!;
        [Required]
        public IndianStates State { get; set; }
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = null!;
        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = null!;
        // Contact details
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string EmailId { get; set; } = null!;
        [Required]
        [Phone]
        public int MobileNumber1 { get; set; } 
        [Phone]
        public int MobileNumber2 { get; set;}

        

    }
}