using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmiCalculatorService : IEmiCalculatorService
    {
        private readonly ILMPRepository<EMIPlan, int> _emiRepository;
        private readonly LmpDbContext _context;

        public EmiCalculatorService(ILMPRepository<EMIPlan, int> emiRepository, LmpDbContext context)
        {
            _emiRepository = emiRepository;
            _context = context;
        }

        public async Task<EMIPlanDTO> CalculateEmiAsync(decimal principalAmount, decimal interestRate, int termMonths)
        {
            var monthlyRate = interestRate / 12 / 100;
            var emi = (principalAmount * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), termMonths)) /
                      ((decimal)Math.Pow((double)(1 + monthlyRate), termMonths) - 1);
            
            var totalRepayment = emi * termMonths;
            var totalInterest = totalRepayment - principalAmount;

            return new EMIPlanDTO
            {
                PrincipleAmount = principalAmount,
                TermMonths = termMonths,
                RateOfInterest = interestRate,
                MonthlyEMI = Math.Round(emi, 2),
                TotalInerestPaid = Math.Round(totalInterest, 2),
                TotalRepaymentAmount = Math.Round(totalRepayment, 2),
                Status = EMIPlanStatus.Active,
                IsCompleted = false
            };
        }

        public async Task<EMIPlanDTO> CreateEmiPlanAsync(EMIPlanCreateDTO createDto)
        {
            var calculatedEmi = await CalculateEmiAsync(createDto.PrincipalAmount, createDto.RateOfInterest, createDto.TermMonths);

            var emiPlan = new EMIPlan
            {
                LoanApplicationBaseId = createDto.LoanApplicationBaseId,
                LoanAccountId = createDto.LoanAccountId,
                PrincipleAmount = createDto.PrincipalAmount,
                TermMonths = createDto.TermMonths,
                RateOfInterest = createDto.RateOfInterest,
                MonthlyEMI = calculatedEmi.MonthlyEMI,
                TotalInterestPaid = calculatedEmi.TotalInerestPaid,
                TotalRepaymentAmount = calculatedEmi.TotalRepaymentAmount,
                Status = EMIPlanStatus.Active,
                IsCompleted = false
            };

            var created = await _emiRepository.AddAsync(emiPlan);
            return MapToDto(created);
        }

        public async Task<EMIPlanDTO> GetEmiPlanByIdAsync(int emiId)
        {
            var emiPlan = await _emiRepository.GetByIdAsync(emiId);
            return emiPlan != null ? MapToDto(emiPlan) : null;
        }

        public async Task<IEnumerable<EMIPlanDTO>> GetEmiPlansByLoanApplicationAsync(int loanApplicationId)
        {
            var emiPlans = await _emiRepository.GetAllAsync();
            return emiPlans.Where(e => e.LoanApplicationBaseId == loanApplicationId).Select(MapToDto);
        }

        private EMIPlanDTO MapToDto(EMIPlan emiPlan)
        {
            return new EMIPlanDTO
            {
                EMIId = emiPlan.EMIId,
                LoanAppicationBaseId = emiPlan.LoanApplicationBaseId,
                LoanAccountId = emiPlan.LoanAccountId,
                PrincipleAmount = emiPlan.PrincipleAmount,
                TermMonths = emiPlan.TermMonths,
                RateOfInterest = emiPlan.RateOfInterest,
                MonthlyEMI = emiPlan.MonthlyEMI,
                TotalInerestPaid = emiPlan.TotalInterestPaid,
                TotalRepaymentAmount = emiPlan.TotalRepaymentAmount,
                Status = emiPlan.Status,
                IsCompleted = emiPlan.IsCompleted
            };
        }

        public async Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId)
        {
            var emiData = await (from emi in _context.EMIPlans
                                join loan in _context.PersonalLoanApplications on emi.LoanApplicationBaseId equals loan.LoanApplicationBaseId
                                join applicant in _context.LoanApplicants on loan.LoanApplicationBaseId equals applicant.LoanApplicationBaseId
                                join account in _context.LoanAccounts on emi.LoanAccountId equals account.LoanAccountId
                                where applicant.CustomerId == customerId && 
                                      emi.Status == EMIPlanStatus.Active && 
                                      !emi.IsCompleted
                                select new { emi, loan, account })
                                .FirstOrDefaultAsync();

            if (emiData == null) return null;

            var payments = await _context.PaymentTransactions
                .Where(p => p.EMIId == emiData.emi.EMIId && p.Status == Database.Entities.PaymentStatus.Success)
                .ToListAsync();

            var totalPaid = payments.Sum(p => p.Amount);
            var pendingAmount = emiData.emi.TotalRepaymentAmount - totalPaid;
            var interestPaid = Math.Min(totalPaid, emiData.emi.TotalInterestPaid);
            var principalPaid = totalPaid - interestPaid;
            var emisPaid = payments.Count;
            var emisRemaining = emiData.emi.TermMonths - emisPaid;
            var nextDueDate = emiData.loan.SubmissionDate.ToDateTime(TimeOnly.MinValue).AddMonths(emisPaid + 1);
            var isOverdue = nextDueDate < DateTime.UtcNow && pendingAmount > 0;
            var daysOverdue = isOverdue ? (DateTime.UtcNow - nextDueDate).Days : 0;

            return new CustomerEMIDashboardDto
            {
                EMIId = emiData.emi.EMIId,
                LoanAccountId = emiData.emi.LoanAccountId,
                TotalLoanAmount = emiData.emi.PrincipleAmount,
                MonthlyEMI = emiData.emi.MonthlyEMI,
                PendingAmount = pendingAmount,
                TotalInterest = emiData.emi.TotalInterestPaid,
                InterestPaid = interestPaid,
                PrincipalPaid = principalPaid,
                CurrentMonthEMI = emiData.emi.MonthlyEMI,
                NextDueDate = nextDueDate,
                EMIsPaid = emisPaid,
                EMIsRemaining = emisRemaining,
                Status = emiData.emi.Status.ToString(),
                IsOverdue = isOverdue,
                DaysOverdue = daysOverdue,
                LateFeeAmount = emiData.account.TotalLateFeePaidAmount,
                PaymentStatus = emiData.account.CurrentPaymentStatus.ToString()
            };
        }

        public async Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId)
        {
            var emiData = await (from emi in _context.EMIPlans
                                join loan in _context.PersonalLoanApplications on emi.LoanApplicationBaseId equals loan.LoanApplicationBaseId
                                join applicant in _context.LoanApplicants on loan.LoanApplicationBaseId equals applicant.LoanApplicationBaseId
                                where applicant.CustomerId == customerId
                                select new { emi, loan })
                                .ToListAsync();

            var result = new List<CustomerEMIDashboardDto>();

            foreach (var data in emiData)
            {
                var payments = await _context.PaymentTransactions
                    .Where(p => p.EMIId == data.emi.EMIId && p.Status == Database.Entities.PaymentStatus.Success)
                    .ToListAsync();

                var totalPaid = payments.Sum(p => p.Amount);
                var pendingAmount = data.emi.TotalRepaymentAmount - totalPaid;
                var interestPaid = Math.Min(totalPaid, data.emi.TotalInterestPaid);
                var principalPaid = totalPaid - interestPaid;
                var emisPaid = payments.Count;
                var emisRemaining = data.emi.TermMonths - emisPaid;
                var nextDueDate = data.loan.SubmissionDate.ToDateTime(TimeOnly.MinValue).AddMonths(emisPaid + 1);
                var isOverdue = nextDueDate < DateTime.UtcNow && pendingAmount > 0;
                var daysOverdue = isOverdue ? (DateTime.UtcNow - nextDueDate).Days : 0;

                result.Add(new CustomerEMIDashboardDto
                {
                    EMIId = data.emi.EMIId,
                    LoanAccountId = data.emi.LoanAccountId,
                    TotalLoanAmount = data.emi.PrincipleAmount,
                    MonthlyEMI = data.emi.MonthlyEMI,
                    PendingAmount = pendingAmount,
                    TotalInterest = data.emi.TotalInterestPaid,
                    InterestPaid = interestPaid,
                    PrincipalPaid = principalPaid,
                    CurrentMonthEMI = data.emi.MonthlyEMI,
                    NextDueDate = nextDueDate,
                    EMIsPaid = emisPaid,
                    EMIsRemaining = emisRemaining,
                    Status = data.emi.Status.ToString(),
                    IsOverdue = isOverdue,
                    DaysOverdue = daysOverdue
                });
            }

            return result;
        }
    }
}