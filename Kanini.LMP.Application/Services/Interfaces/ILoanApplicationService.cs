using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ILoanApplicationService
    {
        // Personal Loan
        Task<PersonalLoanApplicationDTO> CreatePersonalLoanAsync(PersonalLoanApplicationCreateDTO dto, int customerId);
        Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetAllPersonalLoansAsync();
        Task<PersonalLoanApplicationDTO?> GetPersonalLoanByIdAsync(int id);
        
        // Home Loan
        Task<HomeLoanApplicationDTO> CreateHomeLoanAsync(HomeLoanApplicationCreateDTO dto, int customerId);
        Task<IReadOnlyList<HomeLoanApplicationDTO>> GetAllHomeLoansAsync();
        Task<HomeLoanApplicationDTO?> GetHomeLoanByIdAsync(int id);
        
        // Vehicle Loan
        Task<VehicleLoanApplicationDTO> CreateVehicleLoanAsync(VehicleLoanApplicationCreateDTO dto, int customerId);
        Task<IReadOnlyList<VehicleLoanApplicationDTO>> GetAllVehicleLoansAsync();
        Task<VehicleLoanApplicationDTO?> GetVehicleLoanByIdAsync(int id);
        
        // Common
        Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetLoansByStatusAsync(ApplicationStatus status);
        Task<PersonalLoanApplicationDTO> UpdateLoanStatusAsync(int id, ApplicationStatus status);
        Task<int> UploadDocumentAsync(int loanApplicationBaseId, int userId, string documentName, string documentType, byte[] documentData);
        
        // Junction table methods
        Task<IReadOnlyList<int>> GetApplicantsByLoanAsync(int loanApplicationBaseId);
        Task<IReadOnlyList<int>> GetDocumentsByLoanAsync(int loanApplicationBaseId);
    }
}