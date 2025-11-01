using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer
{
    public class CustomerResponseDTO
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string? UserFullName { get; set; } // optional from User table

        public DateOnly DateOfBirth { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; } = null!;
        public string Occupation { get; set; } = null!;
        public decimal AnnualIncome { get; set; }
        public decimal CreditScore { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
