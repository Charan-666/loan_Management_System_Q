using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class HomeLoanApplication: LoanApplicationBase
    {
         
        
            // Product-specific details for Home Loans:
            [Required]
            public BuilderInformation BuilderInformation { get; set; } = null!;
            [Required]
            public HomeLoanDetails HomeLoanDetails { get; set; } = null!;
            [Required]
            public PropertyDetails PropertyDetails { get; set; } = null!;

            // NOTE: DocumentUpload in the original HomeLoan model is now handled by the DocumentLinks collection in the Base class.
        }
    }

