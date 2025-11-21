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

        public async Task<List<EMIScheduleDto>> GenerateEMIScheduleAsync(int emiId)
        {
            var emiPlan = await _emiRepository.GetByIdAsync(emiId);
            if (emiPlan == null) return new List<EMIScheduleDto>();

            var payments = await _context.PaymentTransactions
                .Where(p => p.EMIId == emiId && p.Status == Database.Entities.PaymentStatus.Success)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();

            var schedule = new List<EMIScheduleDto>();
            var monthlyRate = emiPlan.RateOfInterest / 12 / 100;
            var outstandingBalance = emiPlan.PrincipleAmount;
            var startDate = await GetLoanStartDateAsync(emiPlan.LoanApplicationBaseId);

            for (int i = 1; i <= emiPlan.TermMonths; i++)
            {
                var interestAmount = outstandingBalance * monthlyRate;
                var principalAmount = emiPlan.MonthlyEMI - interestAmount;
                outstandingBalance -= principalAmount;
                var dueDate = startDate.AddMonths(i);
                var payment = payments.FirstOrDefault(p => p.PaymentDate.Month == dueDate.Month && p.PaymentDate.Year == dueDate.Year);
                var lateFee = payment == null && dueDate < DateTime.UtcNow ? await CalculateLateFeeForInstallment(dueDate) : 0;

                schedule.Add(new EMIScheduleDto
                {
                    InstallmentNumber = i,
                    DueDate = dueDate,
                    EMIAmount = emiPlan.MonthlyEMI,
                    PrincipalAmount = Math.Round(principalAmount, 2),
                    InterestAmount = Math.Round(interestAmount, 2),
                    OutstandingBalance = Math.Round(Math.Max(0, outstandingBalance), 2),
                    PaymentStatus = payment != null ? "Paid" : (dueDate < DateTime.UtcNow ? "Overdue" : "Pending"),
                    PaidDate = payment?.PaymentDate,
                    LateFee = lateFee
                });
            }

            return schedule;
        }

        public async Task<PrepaymentCalculationDto> CalculatePrepaymentAsync(int emiId, decimal prepaymentAmount)
        {
            var emiPlan = await _emiRepository.GetByIdAsync(emiId);
            if (emiPlan == null) throw new ArgumentException("EMI Plan not found");

            var paidAmount = await _context.PaymentTransactions
                .Where(p => p.EMIId == emiId && p.Status == Database.Entities.PaymentStatus.Success)
                .SumAsync(p => p.Amount);

            var currentOutstanding = emiPlan.TotalRepaymentAmount - paidAmount;
            var prepaymentCharges = prepaymentAmount * 0.02m; // 2% prepayment charges
            var newOutstanding = Math.Max(0, currentOutstanding - prepaymentAmount);
            
            var monthlyRate = emiPlan.RateOfInterest / 12 / 100;
            var remainingMonths = emiPlan.TermMonths - (paidAmount / emiPlan.MonthlyEMI);
            
            var originalInterest = (emiPlan.MonthlyEMI * (decimal)remainingMonths) - (currentOutstanding - (currentOutstanding * monthlyRate * (decimal)remainingMonths));
            var newInterest = newOutstanding > 0 ? CalculateInterestForBalance(newOutstanding, monthlyRate, (int)remainingMonths) : 0;
            var interestSaved = originalInterest - newInterest;
            
            var newEMI = newOutstanding > 0 ? CalculateEMIAmount(newOutstanding, emiPlan.RateOfInterest, (int)remainingMonths) : 0;
            var reducedTenure = newOutstanding > 0 ? 0 : (int)(remainingMonths - (newOutstanding / emiPlan.MonthlyEMI));

            return new PrepaymentCalculationDto
            {
                CurrentOutstanding = Math.Round(currentOutstanding, 2),
                PrepaymentAmount = prepaymentAmount,
                NewOutstanding = Math.Round(newOutstanding, 2),
                InterestSaved = Math.Round(interestSaved, 2),
                NewEMIAmount = Math.Round(newEMI, 2),
                ReducedTenure = reducedTenure,
                PrepaymentCharges = Math.Round(prepaymentCharges, 2),
                NetSavings = Math.Round(interestSaved - prepaymentCharges, 2)
            };
        }

        public async Task<decimal> CalculateLateFeeAsync(int emiId, DateTime currentDate)
        {
            var emiPlan = await _emiRepository.GetByIdAsync(emiId);
            if (emiPlan == null) return 0;

            var payments = await _context.PaymentTransactions
                .Where(p => p.EMIId == emiId && p.Status == Database.Entities.PaymentStatus.Success)
                .CountAsync();

            var startDate = await GetLoanStartDateAsync(emiPlan.LoanApplicationBaseId);
            var nextDueDate = startDate.AddMonths(payments + 1);
            
            if (currentDate <= nextDueDate) return 0;
            
            var daysOverdue = (currentDate - nextDueDate).Days;
            var lateFeeRate = 0.02m; // 2% per month
            var lateFee = emiPlan.MonthlyEMI * lateFeeRate * (daysOverdue / 30m);
            
            return Math.Round(Math.Min(lateFee, emiPlan.MonthlyEMI * 0.1m), 2); // Cap at 10% of EMI
        }

        public async Task<EMIRestructureResultDto> CalculateEMIRestructureAsync(EMIRestructureDto restructureDto)
        {
            var emiPlan = await _emiRepository.GetByIdAsync(restructureDto.EMIId);
            if (emiPlan == null) throw new ArgumentException("EMI Plan not found");

            var paidAmount = await _context.PaymentTransactions
                .Where(p => p.EMIId == restructureDto.EMIId && p.Status == Database.Entities.PaymentStatus.Success)
                .SumAsync(p => p.Amount);

            var currentOutstanding = emiPlan.TotalRepaymentAmount - paidAmount;
            var newTenure = restructureDto.NewTenureMonths ?? emiPlan.TermMonths;
            var newRate = restructureDto.NewInterestRate ?? emiPlan.RateOfInterest;
            var restructureCharges = currentOutstanding * 0.005m; // 0.5% restructure charges
            
            var newEMI = CalculateEMIAmount(currentOutstanding, newRate, newTenure);
            var newTotalAmount = newEMI * newTenure;
            var additionalInterest = newTotalAmount - currentOutstanding;
            
            var newSchedule = await GenerateNewScheduleAsync(currentOutstanding, newEMI, newRate, newTenure, restructureDto.MoratoriumMonths);

            return new EMIRestructureResultDto
            {
                OriginalEMI = emiPlan.MonthlyEMI,
                NewEMI = Math.Round(newEMI, 2),
                OriginalTenure = emiPlan.TermMonths,
                NewTenure = newTenure + restructureDto.MoratoriumMonths,
                AdditionalInterest = Math.Round(additionalInterest, 2),
                RestructureCharges = Math.Round(restructureCharges, 2),
                NewSchedule = newSchedule
            };
        }

        public async Task<EMIPlanDTO> ApplyEMIRestructureAsync(EMIRestructureDto restructureDto)
        {
            var emiPlan = await _emiRepository.GetByIdAsync(restructureDto.EMIId);
            if (emiPlan == null) throw new ArgumentException("EMI Plan not found");

            var calculation = await CalculateEMIRestructureAsync(restructureDto);
            
            emiPlan.TermMonths = calculation.NewTenure;
            emiPlan.MonthlyEMI = calculation.NewEMI;
            emiPlan.RateOfInterest = restructureDto.NewInterestRate ?? emiPlan.RateOfInterest;
            emiPlan.TotalRepaymentAmount = calculation.NewEMI * calculation.NewTenure;
            emiPlan.TotalInterestPaid = emiPlan.TotalRepaymentAmount - emiPlan.PrincipleAmount;
            
            var updated = await _emiRepository.UpdateAsync(emiPlan);
            return MapToDto(updated);
        }

        private async Task<DateTime> GetLoanStartDateAsync(int loanApplicationBaseId)
        {
            var loan = await _context.PersonalLoanApplications
                .FirstOrDefaultAsync(l => l.LoanApplicationBaseId == loanApplicationBaseId);
            return loan?.SubmissionDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
        }

        private async Task<decimal> CalculateLateFeeForInstallment(DateTime dueDate)
        {
            var daysOverdue = (DateTime.UtcNow - dueDate).Days;
            return daysOverdue > 0 ? Math.Min(daysOverdue * 50, 2000) : 0; // ₹50 per day, max ₹2000
        }

        private decimal CalculateInterestForBalance(decimal balance, decimal monthlyRate, int months)
        {
            return balance * monthlyRate * months;
        }

        private decimal CalculateEMIAmount(decimal principal, decimal annualRate, int months)
        {
            var monthlyRate = annualRate / 12 / 100;
            return (principal * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), months)) /
                   ((decimal)Math.Pow((double)(1 + monthlyRate), months) - 1);
        }

        private async Task<List<EMIScheduleDto>> GenerateNewScheduleAsync(decimal principal, decimal emi, decimal rate, int tenure, int moratoriumMonths)
        {
            var schedule = new List<EMIScheduleDto>();
            var monthlyRate = rate / 12 / 100;
            var outstandingBalance = principal;
            var startDate = DateTime.UtcNow;

            for (int i = 1; i <= tenure + moratoriumMonths; i++)
            {
                var dueDate = startDate.AddMonths(i);
                
                if (i <= moratoriumMonths)
                {
                    // Moratorium period - only interest
                    var interestOnly = outstandingBalance * monthlyRate;
                    schedule.Add(new EMIScheduleDto
                    {
                        InstallmentNumber = i,
                        DueDate = dueDate,
                        EMIAmount = Math.Round(interestOnly, 2),
                        PrincipalAmount = 0,
                        InterestAmount = Math.Round(interestOnly, 2),
                        OutstandingBalance = Math.Round(outstandingBalance, 2),
                        PaymentStatus = "Pending"
                    });
                }
                else
                {
                    var interestAmount = outstandingBalance * monthlyRate;
                    var principalAmount = emi - interestAmount;
                    outstandingBalance -= principalAmount;
                    
                    schedule.Add(new EMIScheduleDto
                    {
                        InstallmentNumber = i,
                        DueDate = dueDate,
                        EMIAmount = emi,
                        PrincipalAmount = Math.Round(principalAmount, 2),
                        InterestAmount = Math.Round(interestAmount, 2),
                        OutstandingBalance = Math.Round(Math.Max(0, outstandingBalance), 2),
                        PaymentStatus = "Pending"
                    });
                }
            }

            return schedule;
        }

        public async Task<object> GetCompleteEMIDetailsAsync(int emiId)
        {
            var emiPlan = await GetEmiPlanByIdAsync(emiId);
            if (emiPlan == null) return null;

            var schedule = await GenerateEMIScheduleAsync(emiId);
            var lateFee = await CalculateLateFeeAsync(emiId, DateTime.UtcNow);

            return new
            {
                EMIPlan = emiPlan,
                Schedule = schedule,
                CurrentLateFee = lateFee,
                ScheduleSummary = new
                {
                    TotalInstallments = schedule.Count,
                    PaidInstallments = schedule.Count(s => s.PaymentStatus == "Paid"),
                    OverdueInstallments = schedule.Count(s => s.PaymentStatus == "Overdue"),
                    TotalLateFees = schedule.Sum(s => s.LateFee),
                    NextDueDate = schedule.FirstOrDefault(s => s.PaymentStatus == "Pending")?.DueDate,
                    RemainingPrincipal = schedule.LastOrDefault()?.OutstandingBalance ?? 0
                }
            };
        }
    }
}