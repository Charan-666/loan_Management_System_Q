using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto
{
    public class LoanOriginationWorkflowDTO
    {
        public int WorkflowId { get; set; } 

        [Required(ErrorMessage = "Loan Application Base ID is required.")]
        public int LoanApplicationBaseId { get; set; }

        [Required(ErrorMessage = "Workflow step name is required.")]
        public string StepName { get; set; } = null!;  // Using string for easier frontend mapping (instead of enum)

        [Required(ErrorMessage = "Step status is required.")]
        public string StepStatus { get; set; } = "InProgress"; // Default value for new workflows

        public DateTime? CompletionDate { get; set; }

        public int? ManagerId { get; set; }

        [MaxLength(1000, ErrorMessage = "Manager notes cannot exceed 1000 characters.")]
        public string? ManagerNotes { get; set; }
    }
}

