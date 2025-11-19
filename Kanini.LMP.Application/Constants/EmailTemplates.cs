namespace Kanini.LMP.Application.Constants
{
    public static class EmailTemplates
    {
        public const string PasswordResetSubject = "Password Reset Request - LMP System";
        
        public const string PasswordResetBody = @"
Hello {0},

You have requested to reset your password. Please use the following token to reset your password:

Reset Token: {1}

This token will expire in 1 hour.

If you did not request this, please ignore this email.

Best regards,
LMP Team";

        public const string LoanApprovedSubject = "🎉 Loan Approved - Application #{0}";
        
        public const string LoanApprovedBody = @"
Dear {0},

We are pleased to inform you that your loan application has been approved!

Application ID: {1}
Loan Type: {2}
Approved Amount: ₹{3:N2}
Approval Date: {4}

The loan amount will be disbursed to your registered bank account within 2-3 business days.

Thank you for choosing our services!

Best regards,
LMP Team";

        public const string PaymentSuccessSubject = "✅ Payment Successful - EMI Payment Confirmed";
        
        public const string PaymentSuccessBody = @"
Dear {0},

Your EMI payment has been processed successfully!

Amount Paid: ₹{1:N2}
Payment Date: {2}

Thank you for your timely payment.

Best regards,
LMP Team";
    }
}