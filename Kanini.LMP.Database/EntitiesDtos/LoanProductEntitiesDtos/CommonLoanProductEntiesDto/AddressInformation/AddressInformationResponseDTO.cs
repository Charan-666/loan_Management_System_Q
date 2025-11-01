using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation
{
    public class AddressInformationResponseDTO
    {
        public int AddressInformationId { get; set; }
        public string PresentAddress { get; set; } = null!;
        public string PermanentAddress { get; set; } = null!;
        public string District { get; set; } = null!;
        public IndianStates State { get; set; }
        public string Country { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public string MobileNumber1 { get; set; } = null!;
        public string? MobileNumber2 { get; set; }
        public string? UserName { get; set; } // optional for display in response

    }
}
