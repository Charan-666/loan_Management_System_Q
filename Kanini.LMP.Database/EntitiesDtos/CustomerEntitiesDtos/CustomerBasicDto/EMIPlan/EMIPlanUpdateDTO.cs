using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan
{
    public class EMIPlanUpdateDTO
    {
        [Required]
        public int EMIId { get; set; }

        [Required]
        public EMIPlanStatus Status { get; set; }

        [Display(Name = "Loan Fully Paid")]
        public bool IsCompleted { get; set; }
    }
}
