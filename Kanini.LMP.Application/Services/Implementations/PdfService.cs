using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class PdfService : IPdfService
    {
        private readonly LmpDbContext _context;

        public PdfService(LmpDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateLoanApplicationPdfAsync(int applicationId)
        {
            // Get application details
            var application = await _context.LoanApplicationBases
                .Include(app => app.LoanDetails)
                .FirstOrDefaultAsync(app => app.LoanApplicationBaseId == applicationId);

            if (application == null)
                throw new ArgumentException("Application not found");

            var loanDetails = application.LoanDetails;

            // Generate simple PDF content
            var pdfContent = GenerateApplicationPdfContent(application, loanDetails);
            
            return Encoding.UTF8.GetBytes(pdfContent);
        }

        public async Task<byte[]> GeneratePaymentReceiptPdfAsync(int transactionId)
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(pt => pt.TransactionId == transactionId);

            if (transaction == null)
                throw new ArgumentException("Transaction not found");

            var receiptContent = GeneratePaymentReceiptContent(transaction);
            return Encoding.UTF8.GetBytes(receiptContent);
        }

        public async Task<byte[]> GenerateEMISchedulePdfAsync(int loanAccountId)
        {
            var loanAccount = await _context.LoanAccounts
                .FirstOrDefaultAsync(la => la.LoanAccountId == loanAccountId);

            if (loanAccount == null)
                throw new ArgumentException("Loan account not found");

            var scheduleContent = GenerateEMIScheduleContent(loanAccount);
            return Encoding.UTF8.GetBytes(scheduleContent);
        }

        private string GenerateApplicationPdfContent(dynamic application, dynamic loanDetails)
        {
            return $@"
LOAN APPLICATION DOCUMENT
========================

Application ID: {application.LoanApplicationBaseId}
Application Date: {application.SubmissionDate}
Status: {application.Status}

CUSTOMER INFORMATION
-------------------
Application ID: {application.LoanApplicationBaseId}
Loan Product: {application.LoanProductType}

LOAN DETAILS
-----------
Loan Type: {application.LoanProductType}
Requested Amount: ₹{loanDetails?.RequestedAmount ?? 0:N2}
Tenure: {loanDetails?.TenureMonths ?? 0} months
Interest Rate: {loanDetails?.InterestRate ?? 0}%
Monthly EMI: ₹{loanDetails?.MonthlyInstallment ?? 0:N2}

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}

This is a system-generated document.
";
        }

        private string GeneratePaymentReceiptContent(dynamic transaction)
        {
            return $@"
PAYMENT RECEIPT
==============

Transaction ID: {transaction.TransactionId}
Payment Date: {transaction.PaymentDate:dd MMM yyyy HH:mm}
Amount: ₹{transaction.Amount:N2}
Payment Method: {transaction.PaymentMethod}
Status: {transaction.Status}
Reference: {transaction.TransactionReference ?? "N/A"}
Loan Account: {transaction.LoanAccountId}

Thank you for your payment!

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}
";
        }

        private string GenerateEMIScheduleContent(dynamic loanAccount)
        {
            return $@"
EMI SCHEDULE
===========

Loan Account ID: {loanAccount.LoanAccountId}
Loan Amount: ₹{loanAccount.TotalLoanAmount:N2}

PAYMENT STATUS
-------------
Total Paid (Principal): ₹{loanAccount.TotalPaidPrincipal:N2}
Total Paid (Interest): ₹{loanAccount.TotalPaidInterest:N2}
Remaining Principal: ₹{loanAccount.PrincipalRemaining:N2}
Current Status: {loanAccount.CurrentPaymentStatus}

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}
";
        }
    }
}