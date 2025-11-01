
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape
{
    public class CustomerProfileDto
    {
        /// <summary>
        /// DTO representing the static profile and core demographic information 
        /// for the selected customer, gathered from Customer, PersonalDetails, and AddressInformation.
        /// </summary>


        public int CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string Occupation { get; set; } = null!;
        public byte[] ProfileImage { get; set; } = null!; 
        public string CustomerSegment { get; set; } = null!; 

            //        if (customer.CreditScore >= 750 && activeLoans.All(l => l.DaysPastDue <= 0))
            //{
            //    dto.CustomerSegment = "Excellent Customer";
            //}
            //else if (customer.CreditScore >= 650 && activeLoans.All(l => l.DaysPastDue <= 15))
            //{
            //    dto.CustomerSegment = "Good Customer"; // Matches the UI
            //}
            //else
            //{
            //    dto.CustomerSegment = "High Risk";
            //}





// Demographic Details (The small modal window)
public int Age { get; set; } // Calculated from DateOfBirth
        public string LoanIdLast { get; set; } = null!; // ID of the latest/active loan, if applicable
        public decimal AnnualIncome { get; set; }
        public decimal CreditScore { get; set; } // Added for completeness in a real scenario
        public string AddressState { get; set; } = null!; // Requires StateCode in AddressInformation
        public string ZipCode { get; set; } = null!; // Requires ZipCode in AddressInformation
        public string HomeOwnership { get; set; } = null!; // Requires HomeOwnership field on Customer
        public string PreferredContacts { get; set; } = null!; // Requires PreferredContacts field on Customer

    }
}