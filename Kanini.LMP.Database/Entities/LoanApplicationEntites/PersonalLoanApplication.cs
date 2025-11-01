using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;

﻿using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class PersonalLoanApplication : LoanApplicationBase
    {
        // Personal Loans often have no unique fields beyond the base,
        // as all its core data (employment, financial) is in the base class's nested models.

        // Add any unique field here if a Personal Loan requires something specific
        // that no other loan type needs.
    }

}
