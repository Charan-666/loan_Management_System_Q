using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILMPRepository<LoanApplicationBase, int> _loanAppRepository;
        private readonly ILMPRepository<PersonalLoanApplication, int> _personalLoanRepository;
        private readonly ILMPRepository<HomeLoanApplication, int> _homeLoanRepository;
        private readonly ILMPRepository<VehicleLoanApplication, int> _vehicleLoanRepository;
        private readonly ILMPRepository<LoanDetails, int> _loanDetailsRepository;
        private readonly ILMPRepository<PersonalDetails, int> _personalDetailsRepository;
        private readonly ILMPRepository<AddressInformation, int> _addressRepository;
        private readonly ILMPRepository<PropertyDetails, int> _propertyDetailsRepository;
        private readonly ILMPRepository<VehicleInformation, int> _vehicleInfoRepository;
        private readonly ILMPRepository<DocumentUpload, int> _documentRepository;
        private readonly ILMPRepository<LoanApplicant, int> _loanApplicantRepository;
        private readonly ILMPRepository<ApplicationDocumentLink, int> _documentLinkRepository;
        private readonly IEligibilityService _eligibilityService;

        public LoanApplicationService(
            ILMPRepository<LoanApplicationBase, int> loanAppRepository,
            ILMPRepository<PersonalLoanApplication, int> personalLoanRepository,
            ILMPRepository<HomeLoanApplication, int> homeLoanRepository,
            ILMPRepository<VehicleLoanApplication, int> vehicleLoanRepository,
            ILMPRepository<LoanDetails, int> loanDetailsRepository,
            ILMPRepository<PersonalDetails, int> personalDetailsRepository,
            ILMPRepository<AddressInformation, int> addressRepository,
            ILMPRepository<PropertyDetails, int> propertyDetailsRepository,
            ILMPRepository<VehicleInformation, int> vehicleInfoRepository,
            ILMPRepository<DocumentUpload, int> documentRepository,
            ILMPRepository<LoanApplicant, int> loanApplicantRepository,
            ILMPRepository<ApplicationDocumentLink, int> documentLinkRepository,
            IEligibilityService eligibilityService)
        {
            _loanAppRepository = loanAppRepository;
            _personalLoanRepository = personalLoanRepository;
            _homeLoanRepository = homeLoanRepository;
            _vehicleLoanRepository = vehicleLoanRepository;
            _loanDetailsRepository = loanDetailsRepository;
            _personalDetailsRepository = personalDetailsRepository;
            _addressRepository = addressRepository;
            _propertyDetailsRepository = propertyDetailsRepository;
            _vehicleInfoRepository = vehicleInfoRepository;
            _documentRepository = documentRepository;
            _loanApplicantRepository = loanApplicantRepository;
            _documentLinkRepository = documentLinkRepository;
            _eligibilityService = eligibilityService;
        }

        public async Task<PersonalLoanApplicationDTO> CreatePersonalLoanAsync(PersonalLoanApplicationCreateDTO dto, int customerId)
        {
            // Check eligibility for Personal Loan (ID = 1)
            var isEligible = await _eligibilityService.IsEligibleForLoanAsync(customerId, 1);
            if (!isEligible)
            {
                throw new InvalidOperationException("Customer eligibility score is below 55. Please improve your profile and try again.");
            }

            // Create main application
            var application = new PersonalLoanApplication
            {
                LoanProductType = dto.LoanProductType,
                Status = ApplicationStatus.Draft,
                SubmissionDate = dto.SubmissionDate,
                IsActive = true
            };

            var created = await _personalLoanRepository.AddAsync(application);

            // Add primary applicant
            await AddLoanApplicantAsync(created.LoanApplicationBaseId, customerId, ApplicantRole.Primary);

            // Create related details if provided
            if (dto.LoanDetails != null)
                await CreateLoanDetailsAsync(created.LoanApplicationBaseId, dto.LoanDetails);
            if (dto.PersonalDetails != null)
                await CreatePersonalDetailsAsync(created.LoanApplicationBaseId, dto.PersonalDetails);
            if (dto.AddressInformation != null)
                await CreateAddressInformationAsync(created.LoanApplicationBaseId, dto.AddressInformation);

            // Add co-applicants if provided
            if (dto.ApplicantIds != null)
            {
                foreach (var applicantId in dto.ApplicantIds)
                {
                    await AddLoanApplicantAsync(created.LoanApplicationBaseId, applicantId, ApplicantRole.CoBorrower);
                }
            }

            return MapToDto(created);
        }

        public async Task<HomeLoanApplicationDTO> CreateHomeLoanAsync(HomeLoanApplicationCreateDTO dto, int customerId)
        {
            // Check eligibility for Home Loan (ID = 3)
            var isEligible = await _eligibilityService.IsEligibleForLoanAsync(customerId, 3);
            if (!isEligible)
            {
                throw new InvalidOperationException("Customer eligibility score is below 65. Home loans require higher eligibility.");
            }

            // Create main application
            var application = new HomeLoanApplication
            {
                LoanProductType = dto.LoanProductType,
                Status = ApplicationStatus.Draft,
                SubmissionDate = dto.SubmissionDate,
                IsActive = true
            };

            var created = await _homeLoanRepository.AddAsync(application);

            // Add primary applicant
            await AddLoanApplicantAsync(created.LoanApplicationBaseId, customerId, ApplicantRole.Primary);

            // Create related details
            if (dto.LoanDetails != null)
                await CreateLoanDetailsAsync(created.LoanApplicationBaseId, dto.LoanDetails);
            if (dto.PersonalDetails != null)
                await CreatePersonalDetailsAsync(created.LoanApplicationBaseId, dto.PersonalDetails);
            if (dto.AddressInformation != null)
                await CreateAddressInformationAsync(created.LoanApplicationBaseId, dto.AddressInformation);
            if (dto.PropertyDetails != null)
                await CreatePropertyDetailsAsync(created.LoanApplicationBaseId, dto.PropertyDetails);

            // Add co-applicants if provided
            if (dto.ApplicantIds != null)
            {
                foreach (var applicantId in dto.ApplicantIds)
                {
                    await AddLoanApplicantAsync(created.LoanApplicationBaseId, applicantId, ApplicantRole.CoBorrower);
                }
            }

            return MapHomeLoanToDto(created);
        }

        public async Task<VehicleLoanApplicationDTO> CreateVehicleLoanAsync(VehicleLoanApplicationCreateDTO dto, int customerId)
        {
            // Check eligibility for Vehicle Loan (ID = 2)
            var isEligible = await _eligibilityService.IsEligibleForLoanAsync(customerId, 2);
            if (!isEligible)
            {
                throw new InvalidOperationException("Customer eligibility score is below 55. Please improve your profile and try again.");
            }

            // Create main application
            var application = new VehicleLoanApplication
            {
                LoanProductType = dto.LoanProductType,
                Status = ApplicationStatus.Draft,
                SubmissionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsActive = true
            };

            var created = await _vehicleLoanRepository.AddAsync(application);

            // Add primary applicant
            await AddLoanApplicantAsync(created.LoanApplicationBaseId, customerId, ApplicantRole.Primary);

            // Create related details
            if (dto.LoanDetails != null)
                await CreateLoanDetailsAsync(created.LoanApplicationBaseId, dto.LoanDetails);
            if (dto.PersonalDetails != null)
                await CreatePersonalDetailsAsync(created.LoanApplicationBaseId, dto.PersonalDetails);
            if (dto.AddressInformation != null)
                await CreateAddressInformationAsync(created.LoanApplicationBaseId, dto.AddressInformation);
            if (dto.VehicleInformation != null)
                await CreateVehicleInformationAsync(created.LoanApplicationBaseId, dto.VehicleInformation);

            // Add co-applicants if provided
            if (dto.ApplicantIds != null)
            {
                foreach (var applicantId in dto.ApplicantIds)
                {
                    await AddLoanApplicantAsync(created.LoanApplicationBaseId, applicantId, ApplicantRole.CoBorrower);
                }
            }

            return MapVehicleLoanToDto(created);
        }

        public async Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetAllPersonalLoansAsync()
        {
            var applications = await _personalLoanRepository.GetAllAsync();
            return applications.Select(MapToDto).ToList();
        }

        public async Task<PersonalLoanApplicationDTO?> GetPersonalLoanByIdAsync(int id)
        {
            var application = await _personalLoanRepository.GetByIdAsync(id);
            return application != null ? MapToDto(application) : null;
        }

        public async Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetLoansByStatusAsync(ApplicationStatus status)
        {
            var applications = await _personalLoanRepository.GetAllAsync(a => a.Status == status);
            return applications.Select(MapToDto).ToList();
        }

        public async Task<PersonalLoanApplicationDTO> UpdateLoanStatusAsync(int id, ApplicationStatus status)
        {
            var application = await _personalLoanRepository.GetByIdAsync(id);
            if (application == null) throw new ArgumentException("Loan application not found");

            application.Status = status;
            if (status == ApplicationStatus.Approved)
                application.ApprovedDate = DateOnly.FromDateTime(DateTime.UtcNow);

            var updated = await _personalLoanRepository.UpdateAsync(application);
            return MapToDto(updated);
        }

        private async Task CreateLoanDetailsAsync(int loanApplicationBaseId, Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.LoanDetailsDTO dto)
        {
            var loanDetails = new LoanDetails
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                LoanApplicationId = loanApplicationBaseId,
                RequestedAmount = dto.RequestedAmount,
                TenureMonths = dto.TenureMonths,
                AppliedDate = dto.AppliedDate,
                InterestRate = dto.InterestRate,
                MonthlyInstallment = dto.MonthlyInstallment
            };
            await _loanDetailsRepository.AddAsync(loanDetails);
        }

        private async Task CreatePersonalDetailsAsync(int loanApplicationBaseId, Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.PersonalDetails.PersonalDetailsDTO dto)
        {
            var personalDetails = new PersonalDetails
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                UserId = dto.UserId,
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                DistrictOfBirth = dto.DistrictOfBirth,
                CountryOfBirth = dto.CountryOfBirth,
                PANNumber = dto.PANNumber,
                EducationQualification = dto.EducationQualification,
                ResidentialStatus = dto.ResidentialStatus,
                Gender = dto.Gender,
                SignatureImage = dto.SignatureImage,
                IDProofImage = dto.IDProofImage
            };
            await _personalDetailsRepository.AddAsync(personalDetails);
        }

        private async Task CreateAddressInformationAsync(int loanApplicationBaseId, Database.EntitiesDto.LoanProductEntitiesDto.CommonLoanProductEntiesDto.AddressInformation.AddressInformationDTO dto)
        {
            var address = new AddressInformation
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                UserId = dto.UserId,
                PresentAddress = dto.PresentAddress,
                PermanentAddress = dto.PermanentAddress,
                District = dto.District,
                State = dto.State,
                Country = dto.Country,
                ZipCode = dto.ZipCode,
                EmailId = dto.EmailId,
                MobileNumber1 = int.Parse(dto.MobileNumber1),
                MobileNumber2 = string.IsNullOrEmpty(dto.MobileNumber2) ? 0 : int.Parse(dto.MobileNumber2)
            };
            await _addressRepository.AddAsync(address);
        }

        public async Task<int> UploadDocumentAsync(int loanApplicationBaseId, int userId, string documentName, string documentType, byte[] documentData)
        {
            var document = new DocumentUpload
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                UserId = userId,
                DocumentName = documentName,
                DocumentType = documentType,
                DocumentData = documentData,
                UploadedAt = DateTime.UtcNow
            };
            var created = await _documentRepository.AddAsync(document);

            // Link document to application
            await LinkDocumentToApplicationAsync(loanApplicationBaseId, created.DocumentId, documentType);

            return created.DocumentId;
        }

        private async Task AddLoanApplicantAsync(int loanApplicationBaseId, int customerId, ApplicantRole role)
        {
            var loanApplicant = new LoanApplicant
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                CustomerId = customerId,
                ApplicantRole = role,
                AddedDate = DateTime.UtcNow
            };
            await _loanApplicantRepository.AddAsync(loanApplicant);
        }

        private async Task LinkDocumentToApplicationAsync(int loanApplicationBaseId, int documentId, string documentType)
        {
            // Convert string to DocumentType enum
            var docType = Enum.TryParse<DocumentType>(documentType, true, out var parsedType) ? parsedType : DocumentType.Other;
            
            var documentLink = new ApplicationDocumentLink
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                DocumentId = documentId,
                DocumentRequirementType = docType,
                LinkedAt = DateTime.UtcNow
            };
            await _documentLinkRepository.AddAsync(documentLink);
        }

        public async Task<IReadOnlyList<int>> GetApplicantsByLoanAsync(int loanApplicationBaseId)
        {
            var applicants = await _loanApplicantRepository.GetAllAsync(la => la.LoanApplicationBaseId == loanApplicationBaseId);
            return applicants.Select(a => a.CustomerId).ToList();
        }

        public async Task<IReadOnlyList<int>> GetDocumentsByLoanAsync(int loanApplicationBaseId)
        {
            var documents = await _documentLinkRepository.GetAllAsync(dl => dl.LoanApplicationBaseId == loanApplicationBaseId);
            return documents.Select(d => d.DocumentId).ToList();
        }

        private async Task CreatePropertyDetailsAsync(int loanApplicationBaseId, Database.EntitiesDto.LoanProductEntitiesDto.HomeLoanEntitiesDto.PropertyDetailsDTO dto)
        {
            var propertyDetails = new PropertyDetails
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                UserId = dto.UserId,
                PropertyType = (PropertyType)dto.PropertyType,
                PropertyAddress = dto.PropertyAddress,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                OwnershipType = (OwnershipType)dto.OwnershipType
            };
            await _propertyDetailsRepository.AddAsync(propertyDetails);
        }

        private async Task CreateVehicleInformationAsync(int loanApplicationBaseId, Database.EntitiesDto.LoanProductEntitiesDto.VehicleLoanEntitiesDto.VehicleInformationDTO dto)
        {
            var vehicleInfo = new VehicleInformation
            {
                LoanApplicationBaseId = loanApplicationBaseId,
                UserId = dto.UserId,
                VehicleType = dto.VehicleType,
                Manufacturer = dto.Manufacturer,
                Model = dto.Model,
                Variant = dto.Variant,
                ManufacturingYear = dto.ManufacturingYear,
                VehicleCondition = dto.VehicleCondition,
                ExShowroomPrice = dto.ExShowroomPrice
            };
            await _vehicleInfoRepository.AddAsync(vehicleInfo);
        }

        public async Task<IReadOnlyList<HomeLoanApplicationDTO>> GetAllHomeLoansAsync()
        {
            var applications = await _homeLoanRepository.GetAllAsync();
            return applications.Select(MapHomeLoanToDto).ToList();
        }

        public async Task<HomeLoanApplicationDTO?> GetHomeLoanByIdAsync(int id)
        {
            var application = await _homeLoanRepository.GetByIdAsync(id);
            return application != null ? MapHomeLoanToDto(application) : null;
        }

        public async Task<IReadOnlyList<VehicleLoanApplicationDTO>> GetAllVehicleLoansAsync()
        {
            var applications = await _vehicleLoanRepository.GetAllAsync();
            return applications.Select(MapVehicleLoanToDto).ToList();
        }

        public async Task<VehicleLoanApplicationDTO?> GetVehicleLoanByIdAsync(int id)
        {
            var application = await _vehicleLoanRepository.GetByIdAsync(id);
            return application != null ? MapVehicleLoanToDto(application) : null;
        }

        private PersonalLoanApplicationDTO MapToDto(PersonalLoanApplication application)
        {
            return new PersonalLoanApplicationDTO
            {
                LoanApplicationBaseId = application.LoanApplicationBaseId,
                LoanProductType = application.LoanProductType,
                Status = application.Status,
                SubmissionDate = application.SubmissionDate,
                ApprovedDate = application.ApprovedDate,
                RejectionReason = application.RejectionReason,
                IsActive = application.IsActive,
                LoanDetails = null!, PersonalDetails = null!, AddressInformation = null!,
                FamilyEmergencyDetails = null!, EmploymentDetails = null!, FinancialInformation = null!, Declaration = null!
            };
        }

        private HomeLoanApplicationDTO MapHomeLoanToDto(HomeLoanApplication application)
        {
            return new HomeLoanApplicationDTO
            {
                LoanApplicationBaseId = application.LoanApplicationBaseId,
                LoanProductType = application.LoanProductType,
                Status = application.Status,
                SubmissionDate = application.SubmissionDate,
                ApprovedDate = application.ApprovedDate,
                RejectionReason = application.RejectionReason,
                IsActive = application.IsActive,
                LoanDetails = null!, PersonalDetails = null!, AddressInformation = null!,
                FamilyEmergencyDetails = null!, EmploymentDetails = null!, FinancialInformation = null!, Declaration = null!,
                BuilderInformation = null!, HomeLoanDetails = null!, PropertyDetails = null!
            };
        }

        private VehicleLoanApplicationDTO MapVehicleLoanToDto(VehicleLoanApplication application)
        {
            return new VehicleLoanApplicationDTO
            {
                LoanApplicationBaseId = application.LoanApplicationBaseId,
                LoanProductType = application.LoanProductType,
                Status = application.Status,
                SubmissionDate = application.SubmissionDate,
                ApprovedDate = application.ApprovedDate,
                RejectionReason = application.RejectionReason,
                IsActive = application.IsActive,
                LoanDetails = null!, PersonalDetails = null!, AddressInformation = null!,
                FamilyEmergencyDetails = null!, EmploymentDetails = null!, FinancialInformation = null!, Declaration = null!,
                DealerInformation = null!, VehicleLoanDetails = null!, VehicleInformation = null!
            };
        }
    }
}