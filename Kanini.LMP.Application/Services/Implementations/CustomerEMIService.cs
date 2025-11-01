using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Data;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class CustomerEMIService : ICustomerEMIService
    {
        private readonly LmpDbContext _context;

        public CustomerEMIService(LmpDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId)
        {
            var emiPlan = await _context.EMIPlans
                .Include(e => e.PersonalLoanApplication)
                .ThenInclude(p => p.Customer)
                .Where(e => e.PersonalLoanApplication.CustomerId == customerId && 
                           e.Status == EMIPlanStatus.Active && 
                           !e.IsCompleted)
                .FirstOrDefaultAsync();

            if (emiPlan == null) return null;

            var payments = await _context.PaymentTransactions
                .Where(p => p.EMIId == emiPlan.EMIId && p.Status == Database.Entities.PaymentStatus.Success)
                .ToListAsync();

            var totalPaid = payments.Sum(p => p.Amount);
            var pendingAmount = emiPlan.TotalRepaymentAmount - totalPaid;
            
            // Calculate interest paid vs principal paid
            var interestPaid = Math.Min(totalPaid, emiPlan.TotalInterestPaid);
            var principalPaid = totalPaid - interestPaid;
            
            // Calculate EMIs paid and remaining
            var emisPaid = payments.Count;
            var emisRemaining = emiPlan.TermMonths - emisPaid;
            
            // Calculate next due date (assuming monthly payments)
            var nextDueDate = emiPlan.PersonalLoanApplication.CreatedAt.AddMonths(emisPaid + 1);
            var isOverdue = nextDueDate < DateTime.UtcNow && pendingAmount > 0;
            var daysOverdue = isOverdue ? (DateTime.UtcNow - nextDueDate).Days : 0;

            return new CustomerEMIDashboardDto
            {
                EMIId = emiPlan.EMIId,
                LoanAccountId = emiPlan.LoanAccountId,
                TotalLoanAmount = emiPlan.PrincipleAmount,
                MonthlyEMI = emiPlan.MonthlyEMI,
                PendingAmount = pendingAmount,
                TotalInterest = emiPlan.TotalInterestPaid,
                InterestPaid = interestPaid,
                PrincipalPaid = principalPaid,
                CurrentMonthEMI = emiPlan.MonthlyEMI,
                NextDueDate = nextDueDate,
                EMIsPaid = emisPaid,
                EMIsRemaining = emisRemaining,
                Status = emiPlan.Status.ToString(),
                IsOverdue = isOverdue,
                DaysOverdue = daysOverdue
            };
        }

        public async Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId)
        {
            var emiPlans = await _context.EMIPlans
                .Include(e => e.PersonalLoanApplication)
                .ThenInclude(p => p.Customer)
                .Where(e => e.PersonalLoanApplication.CustomerId == customerId)
                .ToListAsync();

            var result = new List<CustomerEMIDashboardDto>();

            foreach (var emiPlan in emiPlans)
            {
                var payments = await _context.PaymentTransactions
                    .Where(p => p.EMIId == emiPlan.EMIId && p.Status == Database.Entities.PaymentStatus.Success)
                    .ToListAsync();

                var totalPaid = payments.Sum(p => p.Amount);
                var pendingAmount = emiPlan.TotalRepaymentAmount - totalPaid;
                
                var interestPaid = Math.Min(totalPaid, emiPlan.TotalInterestPaid);
                var principalPaid = totalPaid - interestPaid;
                
                var emisPaid = payments.Count;
                var emisRemaining = emiPlan.TermMonths - emisPaid;
                
                var nextDueDate = emiPlan.PersonalLoanApplication.CreatedAt.AddMonths(emisPaid + 1);
                var isOverdue = nextDueDate < DateTime.UtcNow && pendingAmount > 0;
                var daysOverdue = isOverdue ? (DateTime.UtcNow - nextDueDate).Days : 0;

                result.Add(new CustomerEMIDashboardDto
                {
                    EMIId = emiPlan.EMIId,
                    LoanAccountId = emiPlan.LoanAccountId,
                    TotalLoanAmount = emiPlan.PrincipleAmount,
                    MonthlyEMI = emiPlan.MonthlyEMI,
                    PendingAmount = pendingAmount,
                    TotalInterest = emiPlan.TotalInterestPaid,
                    InterestPaid = interestPaid,
                    PrincipalPaid = principalPaid,
                    CurrentMonthEMI = emiPlan.MonthlyEMI,
                    NextDueDate = nextDueDate,
                    EMIsPaid = emisPaid,
                    EMIsRemaining = emisRemaining,
                    Status = emiPlan.Status.ToString(),
                    IsOverdue = isOverdue,
                    DaysOverdue = daysOverdue
                });
            }

            return result;
        }
    }
}