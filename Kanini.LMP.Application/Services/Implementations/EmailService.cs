using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.Email;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                
                using var client = new SmtpClient(smtpSettings["Host"] ?? "smtp.gmail.com", int.Parse(smtpSettings["Port"] ?? "587"))
                {
                    Credentials = new NetworkCredential(smtpSettings["Username"] ?? "", smtpSettings["Password"] ?? ""),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true")
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"] ?? "noreply@lmp.com", smtpSettings["FromName"] ?? "LMP"),
                    Subject = emailDto.Subject,
                    Body = emailDto.Body,
                    IsBodyHtml = emailDto.IsHtml
                };

                message.To.Add(new MailAddress(emailDto.ToEmail, emailDto.ToName));

                // Add attachments
                foreach (var attachment in emailDto.Attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    message.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                }

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {emailDto.ToEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailDto.ToEmail}");
                return false;
            }
        }

        public async Task<bool> SendLoanApplicationSubmittedEmailAsync(string customerEmail, string customerName, int applicationId, string loanType, decimal amount, byte[] applicationPdf)
        {
            var template = GetLoanApplicationSubmittedTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{ApplicationId}}", applicationId.ToString() },
                { "{{LoanType}}", loanType },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{SubmissionDate}}", DateTime.Now.ToString("dd MMM yyyy") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = $"Loan Application Submitted - Application #{applicationId}",
                Body = body,
                Attachments = new List<EmailAttachment>
                {
                    new EmailAttachment
                    {
                        FileName = $"LoanApplication_{applicationId}.pdf",
                        Content = applicationPdf,
                        ContentType = "application/pdf"
                    }
                }
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanApprovedEmailAsync(string customerEmail, string customerName, int applicationId, decimal amount, string loanType)
        {
            var template = GetLoanApprovedTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{ApplicationId}}", applicationId.ToString() },
                { "{{LoanType}}", loanType },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{ApprovalDate}}", DateTime.Now.ToString("dd MMM yyyy") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = $"🎉 Loan Approved - Application #{applicationId}",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanRejectedEmailAsync(string customerEmail, string customerName, int applicationId, string reason)
        {
            var template = GetLoanRejectedTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{ApplicationId}}", applicationId.ToString() },
                { "{{Reason}}", reason },
                { "{{Date}}", DateTime.Now.ToString("dd MMM yyyy") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = $"Loan Application Update - Application #{applicationId}",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendPaymentSuccessEmailAsync(string customerEmail, string customerName, decimal amount, string emiDetails, DateTime paymentDate)
        {
            var template = GetPaymentSuccessTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{EMIDetails}}", emiDetails },
                { "{{PaymentDate}}", paymentDate.ToString("dd MMM yyyy HH:mm") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = "✅ Payment Successful - EMI Payment Confirmed",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendPaymentFailedEmailAsync(string customerEmail, string customerName, decimal amount, string emiDetails, string reason)
        {
            var template = GetPaymentFailedTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{EMIDetails}}", emiDetails },
                { "{{Reason}}", reason },
                { "{{Date}}", DateTime.Now.ToString("dd MMM yyyy HH:mm") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = "❌ Payment Failed - Action Required",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendEMIDueReminderEmailAsync(string customerEmail, string customerName, decimal amount, DateTime dueDate, int daysUntilDue)
        {
            var template = GetEMIDueReminderTemplate();
            var urgencyMessage = daysUntilDue <= 0 ? "is due today" : $"is due in {daysUntilDue} days";
            
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{DueDate}}", dueDate.ToString("dd MMM yyyy") },
                { "{{UrgencyMessage}}", urgencyMessage }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = $"📅 EMI Payment Reminder - Due {dueDate:dd MMM yyyy}",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendOverduePaymentEmailAsync(string customerEmail, string customerName, decimal amount, int daysPastDue)
        {
            var template = GetOverduePaymentTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{DaysPastDue}}", daysPastDue.ToString() },
                { "{{Date}}", DateTime.Now.ToString("dd MMM yyyy") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = $"⚠️ Urgent: Overdue Payment - {daysPastDue} Days Past Due",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanDisbursedEmailAsync(string customerEmail, string customerName, decimal amount, int loanAccountId, DateTime disbursementDate)
        {
            var template = GetLoanDisbursedTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{Amount}}", $"₹{amount:N2}" },
                { "{{LoanAccountId}}", loanAccountId.ToString() },
                { "{{DisbursementDate}}", disbursementDate.ToString("dd MMM yyyy") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = "💰 Loan Disbursed Successfully",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<bool> SendLoanFullyPaidEmailAsync(string customerEmail, string customerName, int loanAccountId, decimal totalAmountPaid)
        {
            var template = GetLoanFullyPaidTemplate();
            var body = ReplacePlaceholders(template.HtmlBody, new Dictionary<string, string>
            {
                { "{{CustomerName}}", customerName },
                { "{{LoanAccountId}}", loanAccountId.ToString() },
                { "{{TotalAmountPaid}}", $"₹{totalAmountPaid:N2}" },
                { "{{CompletionDate}}", DateTime.Now.ToString("dd MMM yyyy") }
            });

            var emailDto = new EmailDto
            {
                ToEmail = customerEmail,
                ToName = customerName,
                Subject = "🎉 Congratulations! Loan Fully Paid",
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        private string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
        {
            var result = template;
            foreach (var placeholder in placeholders)
            {
                result = result.Replace(placeholder.Key, placeholder.Value);
            }
            return result;
        }

        private EmailTemplate GetLoanApplicationSubmittedTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2c5aa0;'>Loan Application Submitted Successfully</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>Thank you for submitting your loan application. We have received your application and it is now under review.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='margin-top: 0; color: #2c5aa0;'>Application Details:</h3>
                            <p><strong>Application ID:</strong> {{ApplicationId}}</p>
                            <p><strong>Loan Type:</strong> {{LoanType}}</p>
                            <p><strong>Requested Amount:</strong> {{Amount}}</p>
                            <p><strong>Submission Date:</strong> {{SubmissionDate}}</p>
                        </div>
                        
                        <p>Please find your application document attached to this email for your records.</p>
                        <p>Our team will review your application and get back to you within 3-5 business days.</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetLoanApprovedTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>🎉 Congratulations! Your Loan is Approved</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>We are pleased to inform you that your loan application has been <strong>approved</strong>!</p>
                        
                        <div style='background-color: #d4edda; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745;'>
                            <h3 style='margin-top: 0; color: #155724;'>Approved Loan Details:</h3>
                            <p><strong>Application ID:</strong> {{ApplicationId}}</p>
                            <p><strong>Loan Type:</strong> {{LoanType}}</p>
                            <p><strong>Approved Amount:</strong> {{Amount}}</p>
                            <p><strong>Approval Date:</strong> {{ApprovalDate}}</p>
                        </div>
                        
                        <p>The loan amount will be disbursed to your registered bank account within 2-3 business days.</p>
                        <p>You will receive further communication regarding EMI schedule and payment details.</p>
                        
                        <p>Thank you for choosing our services!</p>
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetLoanRejectedTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #dc3545;'>Loan Application Update</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>Thank you for your interest in our loan services. After careful review of your application #{{ApplicationId}}, we regret to inform you that we are unable to approve your loan application at this time.</p>
                        
                        <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                            <h3 style='margin-top: 0; color: #721c24;'>Reason:</h3>
                            <p>{{Reason}}</p>
                        </div>
                        
                        <p>You may reapply after addressing the mentioned concerns or contact our customer service for more information.</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetPaymentSuccessTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>✅ Payment Successful</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>Your EMI payment has been processed successfully!</p>
                        
                        <div style='background-color: #d4edda; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745;'>
                            <h3 style='margin-top: 0; color: #155724;'>Payment Details:</h3>
                            <p><strong>Amount Paid:</strong> {{Amount}}</p>
                            <p><strong>EMI Details:</strong> {{EMIDetails}}</p>
                            <p><strong>Payment Date:</strong> {{PaymentDate}}</p>
                        </div>
                        
                        <p>Thank you for your timely payment. Your next EMI will be due as per your schedule.</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetPaymentFailedTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #dc3545;'>❌ Payment Failed</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>We were unable to process your EMI payment. Please try again or contact our support team.</p>
                        
                        <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                            <h3 style='margin-top: 0; color: #721c24;'>Payment Details:</h3>
                            <p><strong>Amount:</strong> {{Amount}}</p>
                            <p><strong>EMI Details:</strong> {{EMIDetails}}</p>
                            <p><strong>Failure Reason:</strong> {{Reason}}</p>
                            <p><strong>Date:</strong> {{Date}}</p>
                        </div>
                        
                        <p>Please ensure sufficient balance and try again, or contact our customer service for assistance.</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetEMIDueReminderTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #ffc107;'>📅 EMI Payment Reminder</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>This is a friendly reminder that your EMI payment {{UrgencyMessage}}.</p>
                        
                        <div style='background-color: #fff3cd; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #ffc107;'>
                            <h3 style='margin-top: 0; color: #856404;'>Payment Details:</h3>
                            <p><strong>EMI Amount:</strong> {{Amount}}</p>
                            <p><strong>Due Date:</strong> {{DueDate}}</p>
                        </div>
                        
                        <p>Please ensure timely payment to avoid late fees and maintain your credit score.</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetOverduePaymentTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #dc3545;'>⚠️ Urgent: Overdue Payment</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>Your EMI payment is now <strong>{{DaysPastDue}} days overdue</strong>. Immediate action is required.</p>
                        
                        <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545;'>
                            <h3 style='margin-top: 0; color: #721c24;'>Overdue Details:</h3>
                            <p><strong>Overdue Amount:</strong> {{Amount}}</p>
                            <p><strong>Days Past Due:</strong> {{DaysPastDue}}</p>
                        </div>
                        
                        <p>Please make the payment immediately to avoid additional penalties and protect your credit score.</p>
                        <p>If you're facing financial difficulties, please contact our customer service team.</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetLoanDisbursedTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>💰 Loan Disbursed Successfully</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>Great news! Your loan amount has been successfully disbursed to your registered bank account.</p>
                        
                        <div style='background-color: #d4edda; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745;'>
                            <h3 style='margin-top: 0; color: #155724;'>Disbursement Details:</h3>
                            <p><strong>Disbursed Amount:</strong> {{Amount}}</p>
                            <p><strong>Loan Account ID:</strong> {{LoanAccountId}}</p>
                            <p><strong>Disbursement Date:</strong> {{DisbursementDate}}</p>
                        </div>
                        
                        <p>Your EMI schedule will begin from next month. You will receive a separate communication with your EMI details.</p>
                        
                        <p>Thank you for choosing our services!</p>
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }

        private EmailTemplate GetLoanFullyPaidTemplate()
        {
            return new EmailTemplate
            {
                HtmlBody = @"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>🎉 Congratulations! Loan Fully Paid</h2>
                        <p>Dear {{CustomerName}},</p>
                        <p>Congratulations! You have successfully completed all payments for your loan.</p>
                        
                        <div style='background-color: #d4edda; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745;'>
                            <h3 style='margin-top: 0; color: #155724;'>Loan Completion Details:</h3>
                            <p><strong>Loan Account ID:</strong> {{LoanAccountId}}</p>
                            <p><strong>Total Amount Paid:</strong> {{TotalAmountPaid}}</p>
                            <p><strong>Completion Date:</strong> {{CompletionDate}}</p>
                        </div>
                        
                        <p>Your loan account has been closed and a completion certificate will be sent to you separately.</p>
                        <p>Thank you for being a valued customer. We look forward to serving you again in the future!</p>
                        
                        <p>Best regards,<br>LMP Team</p>
                    </div>
                </body>
                </html>"
            };
        }
    }
}