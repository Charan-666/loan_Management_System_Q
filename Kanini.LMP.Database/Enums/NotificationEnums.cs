namespace Kanini.LMP.Database.Enums
{
    public enum NotificationChannel
    {
        Email,
        SMS,
        Push,
        WhatsApp,
        InApp
    }

    public enum NotificationType
    {
        LoanApplication,
        PaymentDue,
        PaymentSuccess,
        PaymentFailed,
        LoanApproved,
        LoanRejected,
        LoanDisbursed,
        DocumentVerification,
        General
    }

    public enum NotificationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}