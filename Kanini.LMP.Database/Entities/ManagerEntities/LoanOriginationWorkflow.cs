using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.ManagerEntities
{
    public class LoanOriginationWorkflow
    {
        [Key]
        public int WorkflowId { get; set; }

        // LINK CHANGE: Reference the base class ID, not a specific product ID.
        // This ensures this workflow tracker works for HomeLoanApplication, PersonalLoanApplication, etc.
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; } // Renamed from LoanApplicationId

        [Required]
        public ManagerEnum StepName { get; set; } // Assuming ManagerEnum is defined elsewhere

        /// <summary>
        /// Status of the step: Completed, OnHold, Failed, InProgress.
        /// </summary>
        [Required]
        public StepStatus StepStatus { get; set; } = StepStatus.InProgress; // Assuming StepStatus enum is defined elsewhere

        public DateTime? CompletionDate { get; set; }

        public int? ManagerId { get; set; } // Who completed the step
        [MaxLength(1000)]

        public string? ManagerNotes { get; set; }
    }

}
