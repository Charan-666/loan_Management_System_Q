namespace Kanini.LMP.Database.Enums
{
    public enum LoanPaymentStatus
    {
        /// <summary>
        /// Enum defining the payment status for an active loan account.
        /// This status is updated by the servicing system, not by a manager's approval action.
        /// </summary>


        Active = 0,             // Loan is current and on schedule
        InGracePeriod = 1,      // Payment missed, but within a grace period (not yet officially late)
        Late30Days = 2,         // 1-30 days past due
        Late60Days = 3,         // 31-60 days past due
        Late90Days = 4,         // 61-90 days past due (NPL/Non-Performing Loan risk)
        ChargedOff = 5,         // Deemed uncollectible
        FullyPaid = 6,          // Principal and Interest are fully paid
        Defaulted = 7           // Officially defaulted (used for internal reporting)
    }
}
