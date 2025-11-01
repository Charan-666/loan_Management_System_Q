using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Data;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EMINotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EMINotificationBackgroundService> _logger;

        public EMINotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<EMINotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckEMIDueDatesAsync();
                    await CheckOverduePaymentsAsync();
                    
                    // Run every 24 hours
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking EMI due dates");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Retry after 1 hour on error
                }
            }
        }

        private async Task CheckEMIDueDatesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LmpDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var today = DateTime.Today;
            var threeDaysFromNow = today.AddDays(3);

            // Get active loan accounts with upcoming EMI due dates
            var upcomingEMIs = await context.LoanAccounts
                .Where(la => la.CurrentPaymentStatus == LoanPaymentStatus.Active)
                .Join(context.EMIPlans,
                    la => la.LoanAccountId,
                    emi => emi.LoanAccountId,
                    (la, emi) => new { LoanAccount = la, EMI = emi })
                .Where(x => x.EMI.Status == EMIPlanStatus.Active && !x.EMI.IsCompleted)
                .ToListAsync();

            foreach (var emiInfo in upcomingEMIs)
            {
                // Calculate next due date (assuming monthly EMIs)
                var nextDueDate = emiInfo.LoanAccount.DisbursementDate.AddMonths(1);
                
                // Notify 3 days before due date
                if (nextDueDate.Date == threeDaysFromNow.Date)
                {
                    await notificationService.NotifyEMIDueAsync(
                        emiInfo.LoanAccount.CustomerId,
                        emiInfo.EMI.MonthlyEMI,
                        nextDueDate,
                        emiInfo.EMI.EMIId);
                }
                
                // Notify on due date
                if (nextDueDate.Date == today.Date)
                {
                    await notificationService.NotifyEMIDueAsync(
                        emiInfo.LoanAccount.CustomerId,
                        emiInfo.EMI.MonthlyEMI,
                        nextDueDate,
                        emiInfo.EMI.EMIId);
                }
            }

            _logger.LogInformation($"Checked {upcomingEMIs.Count} EMI accounts for due date notifications");
        }

        private async Task CheckOverduePaymentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LmpDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            // Get overdue loan accounts
            var overdueAccounts = await context.LoanAccounts
                .Where(la => la.DaysPastDue > 0 && 
                           (la.CurrentPaymentStatus == LoanPaymentStatus.Late30Days ||
                            la.CurrentPaymentStatus == LoanPaymentStatus.Late60Days ||
                            la.CurrentPaymentStatus == LoanPaymentStatus.Late90Days))
                .Join(context.EMIPlans,
                    la => la.LoanAccountId,
                    emi => emi.LoanAccountId,
                    (la, emi) => new { LoanAccount = la, EMI = emi })
                .ToListAsync();

            foreach (var overdueInfo in overdueAccounts)
            {
                // Apply penalty based on days overdue
                await ApplyOverduePenaltyAsync(context, overdueInfo.LoanAccount, overdueInfo.EMI);
                
                await notificationService.NotifyOverduePaymentAsync(
                    overdueInfo.LoanAccount.CustomerId,
                    overdueInfo.EMI.MonthlyEMI,
                    overdueInfo.LoanAccount.DaysPastDue,
                    overdueInfo.EMI.EMIId);
            }

            _logger.LogInformation($"Checked {overdueAccounts.Count} overdue accounts for notifications and penalties");
        }

        private async Task ApplyOverduePenaltyAsync(LmpDbContext context, dynamic loanAccount, dynamic emi)
        {
            var daysPastDue = (int)loanAccount.DaysPastDue;
            var monthlyEMI = (decimal)emi.MonthlyEMI;
            var currentLateFee = (decimal)loanAccount.TotalLateFeePaidAmount;
            
            decimal penaltyAmount = 0;
            
            // Penalty structure based on days overdue
            if (daysPastDue >= 7 && daysPastDue < 30)
            {
                // 2% of monthly EMI after 7 days
                penaltyAmount = monthlyEMI * 0.02m;
            }
            else if (daysPastDue >= 30 && daysPastDue < 60)
            {
                // 5% of monthly EMI after 30 days
                penaltyAmount = monthlyEMI * 0.05m;
            }
            else if (daysPastDue >= 60)
            {
                // 10% of monthly EMI after 60 days
                penaltyAmount = monthlyEMI * 0.10m;
            }
            
            if (penaltyAmount > 0)
            {
                // Update loan account with penalty
                var account = await context.LoanAccounts.FindAsync((int)loanAccount.LoanAccountId);
                if (account != null)
                {
                    account.TotalLateFeePaidAmount += penaltyAmount;
                    account.LastStatusUpdate = DateTime.UtcNow;
                    
                    // Update payment status based on days overdue
                    if (daysPastDue >= 90)
                        account.CurrentPaymentStatus = LoanPaymentStatus.Late90Days;
                    else if (daysPastDue >= 60)
                        account.CurrentPaymentStatus = LoanPaymentStatus.Late60Days;
                    else if (daysPastDue >= 30)
                        account.CurrentPaymentStatus = LoanPaymentStatus.Late30Days;
                    
                    await context.SaveChangesAsync();
                    
                    _logger.LogInformation($"Applied penalty of {penaltyAmount:C} to loan account {account.LoanAccountId} for {daysPastDue} days overdue");
                }
            }
        }
    }
}