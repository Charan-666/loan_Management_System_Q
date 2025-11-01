using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class LoanApplicationBase
    {
        // *** NOTE: This ID serves as the Primary Key for ALL inherited applications ***
        [Key]
        public int LoanApplicationBaseId { get; set; }
        // FK → LoanProduct (Discriminator for derived loan types)
        [Required]

        [ForeignKey(nameof(LoanProduct))]
        public string LoanProductType { get; set; } = null!; // Discriminator field

        // --- Common Nested Details (1:1 relationships) ---
        [Required]
        public LoanDetails LoanDetails { get; set; } = null!;
        [Required]
        public PersonalDetails PersonalDetails { get; set; } = null!;
        [Required]
        public AddressInformation AddressInformation { get; set; } = null!;
        [Required]
        public FamilyEmergencyDetails FamilyEmergencyDetails { get; set; } = null!;
        [Required]
        public EmploymentDetails EmploymentDetails { get; set; } = null!;
        [Required]
        public FinancialInformation FinancialInformation { get; set; } = null!;
        [Required]
        public Declaration Declaration { get; set; } = null!;

        // --- Common Workflow Fields ---
        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        [MaxLength(500)]
        public string? RejectionReason { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;

        // --- Common M:M Navigation Properties (Joint Account & Documents) ---
        /// <summary>
        /// M:M Joint Accounts: Links to all participating customers (Primary, Joint).
        /// </summary>
        public ICollection<LoanApplicant> Applicants { get; set; } = new List<LoanApplicant>();

        /// <summary>
        /// M:M Document Uploads: Links to the specific files uploaded for this application.
        /// </summary>
        public ICollection<ApplicationDocumentLink> DocumentLinks { get; set; } = new List<ApplicationDocumentLink>();
    }
}
