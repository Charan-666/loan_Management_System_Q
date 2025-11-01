using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans
{
    public class LoanApprovalDetailDto
    {
        /// <summary>
        /// Master DTO that provides all data necessary for the manager to approve, 
        /// reject, or revise a single loan application.
        /// </summary>



        // --- 1. Top-Level Application Identifiers & Status ---
        public int LoanApplicationId { get; set; }
        public string ApplicationNumber { get; set; } = null!;
        public string LoanProductType { get; set; } = null!;
        public ApplicationStatus CurrentStatus { get; set; } // From the LoanApplication model

        // --- 2. Customer Profile Details ---
        public string CustomerFullName { get; set; } = null!;
        public string CustomerId { get; set; } = null!; // The ID displayed next to the name
        public string CustomerOccupation { get; set; } = null!;
        public string ProfileImageBase64 { get; set; } = null!; // Base64 encoding of the customer's profile photo

        // --- 3. Loan Request & Revised Amounts ---
        public decimal RequestedLoanAmount { get; set; }
        public int RequestedTenureMonths { get; set; }

        // Calculated by the risk engine
        public decimal EligibilityAmountCalculated { get; set; }
        public string EligibilityMessage { get; set; } = null!;

        // Updated by the manager's revision action
        public decimal RevisedAmountOffered { get; set; }
        public int RevisedTenureMonthsOffered { get; set; }

        // --- 4. Documents for Modal Pop-up (Base64 from PersonalDetails) ---
        public string SignatureImageBase64 { get; set; } = null!;
        public string IDProofImageBase64 { get; set; } = null!;

        // --- 5. Origination Workflow Status (From LoanOriginationWorkflow Model) ---
        public List<WorkflowStepDto> OriginationPipeline { get; set; } = new List<WorkflowStepDto>();

        // --- 6. Eligibility Scores & Status ---
        public decimal KYCEligibilityScore { get; set; }
        public decimal LoanDefaultScore { get; set; }
        public decimal OverallEligibilityScore { get; set; }

        // --- 7. Model Explanability Data (For Charts) ---
        public ModelExplainerDataDto ExplainerData { get; set; } = new ModelExplainerDataDto();
    }



    //for feature graph in default score check card
    public class FeatureContributionPoint
    {
        public int XAxisIndex { get; set; }
        public decimal PositiveContribution { get; set; }
        public decimal NegativeContribution { get; set; }
    }
}
