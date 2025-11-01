using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape
{
    internal class CustomerScapeMasterDto
    {
        /// <summary>
        /// The main DTO returned by the API call when the manager searches for a Customer ID.
        /// Contains all nested data required to render the full Customer Scape page.
        /// </summary>


        public CustomerProfileDto Profile { get; set; } = new CustomerProfileDto();

        public List<CustomerLoanSummaryDto> LoanHistory { get; set; } = new List<CustomerLoanSummaryDto>();
    }
}
