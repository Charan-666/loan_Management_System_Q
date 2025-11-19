using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;

namespace Kanini.LMP.Data.Data
{
    public class LmpDbContext : DbContext
    {
        public LmpDbContext(DbContextOptions<LmpDbContext> options) : base(options) { }

        /* ---------- Aggregates ---------- */
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<LoanApplicationBase> LoanApplicationBases => Set<LoanApplicationBase>();
        public DbSet<PersonalLoanApplication> PersonalLoanApplications => Set<PersonalLoanApplication>();
        public DbSet<HomeLoanApplication> HomeLoanApplications => Set<HomeLoanApplication>();
        public DbSet<VehicleLoanApplication> VehicleLoanApplications => Set<VehicleLoanApplication>();

        public DbSet<LoanApplicant> LoanApplicants => Set<LoanApplicant>();
        public DbSet<ApplicationDocumentLink> ApplicationDocumentLinks => Set<ApplicationDocumentLink>();
        public DbSet<DocumentUpload> DocumentUploads => Set<DocumentUpload>();
        public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
        public DbSet<EMIPlan> EMIPlans => Set<EMIPlan>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
        public DbSet<LoanOriginationWorkflow> LoanOriginationWorkflows => Set<LoanOriginationWorkflow>();
        public DbSet<LoanAccount> LoanAccounts => Set<LoanAccount>();
        public DbSet<Notification> Notifications => Set<Notification>();

        /* ---------- Common-detail entities ---------- */
        public DbSet<AddressInformation> AddressInformations => Set<AddressInformation>();
        public DbSet<PersonalDetails> PersonalDetails => Set<PersonalDetails>();
        public DbSet<FamilyEmergencyDetails> FamilyEmergencyDetails => Set<FamilyEmergencyDetails>();
        public DbSet<EmploymentDetails> EmploymentDetails => Set<EmploymentDetails>();
        public DbSet<FinancialInformation> FinancialInformations => Set<FinancialInformation>();
        public DbSet<LoanDetails> LoanDetails => Set<LoanDetails>();
        public DbSet<Declaration> Declarations => Set<Declaration>();

        /* ---------- Product-specific details ---------- */
        public DbSet<BuilderInformation> BuilderInformations => Set<BuilderInformation>();
        public DbSet<HomeLoanDetails> HomeLoanDetails => Set<HomeLoanDetails>();
        public DbSet<PropertyDetails> PropertyDetails => Set<PropertyDetails>();
        public DbSet<DealerInformation> DealerInformations => Set<DealerInformation>();
        public DbSet<VehicleInformation> VehicleInformations => Set<VehicleInformation>();
        public DbSet<VehicleLoanDetails> VehicleLoanDetails => Set<VehicleLoanDetails>();


        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ====================================================================
            // 0. Identity Configuration for Int Keys
            // ====================================================================

            // Ensure all int PKs are configured to be auto-incrementing identity columns
            foreach (var entityType in b.Model.GetEntityTypes())
            {
                var pk = entityType.FindPrimaryKey();
                if (pk != null && pk.Properties.Count == 1 && pk.Properties[0].ClrType == typeof(int))
                {
                    pk.Properties[0].ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                }
            }


            // ====================================================================
            // 1. TPT (Table-Per-Type) Inheritance Setup
            // ====================================================================
            b.Entity<LoanApplicationBase>().ToTable("LoanApplicationBases");
            b.Entity<PersonalLoanApplication>().ToTable("PersonalLoanApplications");
            b.Entity<HomeLoanApplication>().ToTable("HomeLoanApplications");
            b.Entity<VehicleLoanApplication>().ToTable("VehicleLoanApplications");

            // ====================================================================
            // 2. CORE RELATIONSHIPS
            // ====================================================================

            // User (1) to Customer (1)
            b.Entity<Customer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // LoanApplicationBase (M) to LoanProduct (1) - Links string FK to string PK
            b.Entity<LoanApplicationBase>()
                .HasOne<LoanProduct>()
                .WithMany()
                .HasForeignKey(la => la.LoanProductType)
                .HasPrincipalKey(lp => lp.LoanProductName)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================================
            // 3. M:M JOINT ACCOUNTS (LoanApplicant)
            // ====================================================================

            // Composite key must use the Base ID (int)
            b.Entity<LoanApplicant>()
                .HasKey(la => new { la.LoanApplicationBaseId, la.CustomerId });

            b.Entity<LoanApplicant>()
                .HasOne(la => la.LoanApplicationBase)
                .WithMany(app => app.Applicants)
                .HasForeignKey(la => la.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicant>()
                .HasOne(la => la.Customer)
                .WithMany(c => c.Applications)
                .HasForeignKey(la => la.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================================
            // 4. M:M DOCUMENT UPLOADS (ApplicationDocumentLink)
            // ====================================================================

            // Composite key must use the Base ID (int)
            b.Entity<ApplicationDocumentLink>()
                .HasKey(ad => new { ad.LoanApplicationBaseId, ad.DocumentId });

            b.Entity<ApplicationDocumentLink>()
                .HasOne(ad => ad.LoanApplicationBase)
                .WithMany(app => app.DocumentLinks)
                .HasForeignKey(ad => ad.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<ApplicationDocumentLink>()
                .HasOne(ad => ad.DocumentUpload)
                .WithMany(doc => doc.ApplicationLinks)
                .HasForeignKey(ad => ad.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================================
            // 5. 1:1 NESTED DETAILS (TPT FIXES)
            // ====================================================================

            // All 1:1 details MUST link to LoanApplicationBase via the Base ID (the shared PK).

            Action<ModelBuilder, Type, string, string> ConfigureOneToOne = (builder, detailType, navPropName, fkPropName) =>
            {
                builder.Entity<LoanApplicationBase>()
                    .HasOne(navPropName)
                    .WithOne()
                    .HasForeignKey(detailType, fkPropName)
                    .OnDelete(DeleteBehavior.Cascade);
            };

            // Common Details (Assumes all now have LoanApplicationBaseId FK)
            ConfigureOneToOne(b, typeof(LoanDetails), nameof(LoanApplicationBase.LoanDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(PersonalDetails), nameof(LoanApplicationBase.PersonalDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(AddressInformation), nameof(LoanApplicationBase.AddressInformation), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(FamilyEmergencyDetails), nameof(LoanApplicationBase.FamilyEmergencyDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(EmploymentDetails), nameof(LoanApplicationBase.EmploymentDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(FinancialInformation), nameof(LoanApplicationBase.FinancialInformation), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(Declaration), nameof(LoanApplicationBase.Declaration), "LoanApplicationBaseId");

            // Product-Specific Details (Link to their derived application type)
            b.Entity<HomeLoanApplication>()
                .HasOne(hla => hla.PropertyDetails)
                .WithOne()
                .HasForeignKey<PropertyDetails>(pd => pd.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<VehicleLoanApplication>()
                .HasOne(vla => vla.VehicleInformation)
                .WithOne()
                .HasForeignKey<VehicleInformation>(vi => vi.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<HomeLoanApplication>()
                .HasOne(hla => hla.BuilderInformation)
                .WithOne()
                .HasForeignKey<BuilderInformation>(bi => bi.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<HomeLoanApplication>()
                .HasOne(hla => hla.HomeLoanDetails)
                .WithOne()
                .HasForeignKey<HomeLoanDetails>(hld => hld.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<VehicleLoanApplication>()
                .HasOne(vla => vla.DealerInformation)
                .WithOne()
                .HasForeignKey<DealerInformation>(di => di.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<VehicleLoanApplication>()
                .HasOne(vla => vla.VehicleLoanDetails)
                .WithOne()
                .HasForeignKey<VehicleLoanDetails>(vld => vld.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);


            // ====================================================================
            // 6. MANAGER MODELS INTEGRATION (Your Models)
            // ====================================================================

            // LoanOriginationWorkflow links to LoanApplicationBase
            b.Entity<LoanOriginationWorkflow>()
                .HasOne<LoanApplicationBase>()
                .WithMany()
                .HasForeignKey(w => w.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // LoanAccount links to LoanApplicationBase (1:1)
            b.Entity<LoanAccount>()
                .HasOne<LoanApplicationBase>()
                .WithOne()
                .HasForeignKey<LoanAccount>(a => a.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // LoanAccount links to Customer (M:1)
            b.Entity<LoanAccount>()
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================================
            // 7. PAYMENT AND MANAGER ENTITIES
            // ====================================================================

            // PaymentTransaction (M) to EMIPlan (1)
            b.Entity<PaymentTransaction>()
                .HasOne<EMIPlan>()
                .WithMany()
                .HasForeignKey(pt => pt.EMIId)
                .OnDelete(DeleteBehavior.Restrict);

            // EMIPlan (M) to LoanAccount (1) - Assumes EMIPlan is linked to the Servicing Account
            b.Entity<EMIPlan>()
                .HasOne<LoanAccount>()
                .WithMany()
                .HasForeignKey(e => e.LoanAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // DocumentUpload (M) to User (1)
            b.Entity<DocumentUpload>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(du => du.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification (M) to User (1)
            b.Entity<Notification>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // ====================================================================
            // 8. DATA SEEDING (All IDs converted to int)
            // ====================================================================

            // NOTE: All IDs used here are sequential integers. EF Core will ensure
            // uniqueness and auto-increment based on the TPT/Identity configurations.

            // --------------------------------------------------------------------
            // 1. LOAN PRODUCTS
            // --------------------------------------------------------------------
            b.Entity<LoanProduct>().HasData(
                new LoanProduct
                {
                    LoanProductId = 1,
                    LoanProductName = "PersonalLoan-Standard",
                    LoanProductDescription = "Unsecured loan for personal use, high flexibility, terms up to 7 years.",
                    IsActive = true
                },
                new LoanProduct
                {
                    LoanProductId = 2,
                    LoanProductName = "CarLoan-Used",
                    LoanProductDescription = "Secured loan for pre-owned vehicles, up to 90% financing, maximum term 5 years.",
                    IsActive = true
                });

            // --------------------------------------------------------------------
            // 2. USERS
            // --------------------------------------------------------------------
            b.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Alex Johnson",
                    Email = "alex.johnson@customer.com",
                    PasswordHash = "hashedpassword_alex",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 2,
                    FullName = "Emma Brown",
                    Email = "emma.brown@customer.com",
                    PasswordHash = "hashedpassword_emma",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 10, 1, 15, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 3,
                    FullName = "Alice Smith (Credit)",
                    Email = "alice.smith@manager.com",
                    PasswordHash = "hashedpassword_alice",
                    Roles = UserEnums.Manager,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 4,
                    FullName = "Bob Davis (Verification)",
                    Email = "bob.davis@manager.com",
                    PasswordHash = "hashedpassword_bob",
                    Roles = UserEnums.Manager,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 2, 20, 9, 30, 0, DateTimeKind.Utc)
                });

            // --------------------------------------------------------------------
            // 3. CUSTOMERS
            // --------------------------------------------------------------------
            b.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    UserId = 1,
                    DateOfBirth = new DateOnly(1995, 10, 20),
                    Gender = Gender.Male,
                    PhoneNumber = "9876543210",
                    Occupation = "Software Engineer",
                    AnnualIncome = 1_500_000.00m,
                    CreditScore = 780m,
                    ProfileImage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
                    UpdatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), // FIXED: Static datetime
                    HomeOwnershipStatus = HomeOwnershipStatus.Owned
                },
                new Customer
                {
                    CustomerId = 2,
                    UserId = 2,
                    DateOfBirth = new DateOnly(1998, 5, 15),
                    Gender = Gender.Female,
                    PhoneNumber = "8012345678",
                    Occupation = "Data Analyst",
                    AnnualIncome = 950_000.00m,
                    CreditScore = 725m,
                    ProfileImage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
                    UpdatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), // FIXED: Static datetime
                    HomeOwnershipStatus = HomeOwnershipStatus.Rented
                });

            // --------------------------------------------------------------------
            // 4. LOAN APPLICATION BASE (TPT Tables)
            // --------------------------------------------------------------------
            b.Entity<PersonalLoanApplication>().HasData(
                new PersonalLoanApplication
                {
                    LoanApplicationBaseId = 1,
                    LoanProductType = "PersonalLoan-Standard",
                    Status = ApplicationStatus.Approved,
                    SubmissionDate = new DateOnly(2025, 1, 15),
                    ApprovedDate = new DateOnly(2025, 1, 25),
                    IsActive = true
                });

            b.Entity<VehicleLoanApplication>().HasData(
                new VehicleLoanApplication
                {
                    LoanApplicationBaseId = 2,
                    LoanProductType = "CarLoan-Used",
                    Status = ApplicationStatus.Submitted,
                    SubmissionDate = new DateOnly(2025, 10, 20),
                    IsActive = true
                });

            // --------------------------------------------------------------------
            // 5. LOAN APPLICANTS (junction)
            // --------------------------------------------------------------------
            b.Entity<LoanApplicant>().HasData(
                new LoanApplicant
                {
                    LoanApplicationBaseId = 1,
                    CustomerId = 1,
                    ApplicantRole = ApplicantRole.Primary,
                    AddedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc) // FIXED: Static datetime
                },
                new LoanApplicant
                {
                    LoanApplicationBaseId = 1,
                    CustomerId = 2,
                    ApplicantRole = ApplicantRole.CoBorrower,
                    AddedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc) // FIXED: Static datetime
                });

            // --------------------------------------------------------------------
            // 6. LOAN DETAILS
            // --------------------------------------------------------------------
            b.Entity<LoanDetails>().HasData(
                new LoanDetails
                {
                    LoanDetailsId = 1,
                    LoanApplicationBaseId = 1,
                    RequestedAmount = 500_000.00m,
                    TenureMonths = 60,
                    AppliedDate = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    InterestRate = 12.00m,
                    MonthlyInstallment = 11_122.22m
                },
                new LoanDetails
                {
                    LoanDetailsId = 2,
                    LoanApplicationBaseId = 2,
                    RequestedAmount = 50_000.00m,
                    TenureMonths = 12,
                    AppliedDate = new DateTime(2025, 10, 20, 15, 30, 0, DateTimeKind.Utc),
                    InterestRate = 10.00m,
                    MonthlyInstallment = 4_403.95m
                });

            // --------------------------------------------------------------------
            // 7. LOAN ACCOUNTS
            // --------------------------------------------------------------------
            b.Entity<LoanAccount>().HasData(
                new LoanAccount
                {
                    LoanAccountId = 1,
                    LoanApplicationBaseId = 1,
                    CustomerId = 1,
                    CurrentPaymentStatus = LoanPaymentStatus.Active,
                    DisbursementDate = new DateTime(2025, 2, 1),
                    DaysPastDue = 0,
                    LastStatusUpdate = new DateTime(2025, 10, 25),
                    TotalLoanAmount = 500_000.00m,
                    TotalPaidPrincipal = 150_000.00m,
                    TotalPaidInterest = 45_000.00m,
                    PrincipalRemaining = 350_000.00m,
                    LastPaymentDate = new DateTime(2025, 10, 25),
                    TotalLateFeePaidAmount = 0.00m
                },
                new LoanAccount
                {
                    LoanAccountId = 2,
                    LoanApplicationBaseId = 2,
                    CustomerId = 2,
                    CurrentPaymentStatus = LoanPaymentStatus.FullyPaid,
                    DisbursementDate = new DateTime(2023, 5, 10),
                    DaysPastDue = 0,
                    LastStatusUpdate = new DateTime(2024, 5, 10),
                    TotalLoanAmount = 50_000.00m,
                    TotalPaidPrincipal = 50_000.00m,
                    TotalPaidInterest = 2_847.40m,
                    PrincipalRemaining = 0.00m,
                    LastPaymentDate = new DateTime(2024, 5, 10),
                    TotalLateFeePaidAmount = 150.00m
                });

            // --------------------------------------------------------------------
            // 8. EMI PLANS
            // --------------------------------------------------------------------
            b.Entity<EMIPlan>().HasData(
                new EMIPlan
                {
                    EMIId = 1,
                    LoanAccountId = 1,
                    LoanApplicationBaseId = 1,
                    PrincipleAmount = 500_000.00m,
                    TermMonths = 60,
                    RateOfInterest = 12.00m,
                    MonthlyEMI = 11_122.22m,
                    TotalInterestPaid = 167_333.20m,
                    TotalRepaymentAmount = 667_333.20m,
                    Status = EMIPlanStatus.Active,
                    IsCompleted = false
                },
                new EMIPlan
                {
                    EMIId = 2,
                    LoanAccountId = 2,
                    LoanApplicationBaseId = 2,
                    PrincipleAmount = 50_000.00m,
                    TermMonths = 12,
                    RateOfInterest = 10.00m,
                    MonthlyEMI = 4_403.95m,
                    TotalInterestPaid = 2_847.40m,
                    TotalRepaymentAmount = 52_847.40m,
                    Status = EMIPlanStatus.Closed,
                    IsCompleted = true
                });

            // --------------------------------------------------------------------
            // 9. PAYMENT TRANSACTIONS
            // --------------------------------------------------------------------
            b.Entity<PaymentTransaction>().HasData(
                new PaymentTransaction
                {
                    TransactionId = 1,
                    EMIId = 1,
                    LoanAccountId = 1,
                    Amount = 11_122.22m,
                    PaymentDate = new DateTime(2025, 10, 25),
                    PaymentMethod = PaymentMethod.UPI,
                    TransactionReference = "UPI-TXN-1234567890",
                    Status = PaymentStatus.Success,
                    CreatedAt = new DateTime(2025, 10, 25),
                    IsActive = true
                },
                new PaymentTransaction
                {
                    TransactionId = 2,
                    EMIId = 1,
                    LoanAccountId = 1,
                    Amount = 11_122.22m,
                    PaymentDate = new DateTime(2025, 10, 24),
                    PaymentMethod = PaymentMethod.NetBanking,
                    TransactionReference = "NB-TXN-0987654321",
                    Status = PaymentStatus.Failed,
                    CreatedAt = new DateTime(2025, 10, 24),
                    IsActive = true
                });

            // --------------------------------------------------------------------
            // 10. LOAN ORIGINATION WORKFLOW
            // --------------------------------------------------------------------
            b.Entity<LoanOriginationWorkflow>().HasData(
                new LoanOriginationWorkflow
                {
                    WorkflowId = 1,
                    LoanApplicationBaseId = 1,
                    StepName = ManagerEnum.Review,
                    StepStatus = StepStatus.Completed,
                    CompletionDate = new DateTime(2025, 1, 20, 14, 30, 0, DateTimeKind.Utc),
                    ManagerId = 3,
                    ManagerNotes = "Approved with standard rate."
                },
                new LoanOriginationWorkflow
                {
                    WorkflowId = 2,
                    LoanApplicationBaseId = 2,
                    StepName = ManagerEnum.DocumentVerificationI,
                    StepStatus = StepStatus.InProgress,
                    ManagerId = 4,
                    ManagerNotes = "Awaiting verification report."
                });

            // --------------------------------------------------------------------
            // 11. DOCUMENTS
            // --------------------------------------------------------------------
            var utilBill = new byte[] { 0x55, 0x54, 0x49, 0x4C };
            var payStub = new byte[] { 0x50, 0x41, 0x59, 0x53 };

            b.Entity<DocumentUpload>().HasData(
                new DocumentUpload
                {
                    DocumentId = 1,
                    LoanApplicationBaseId = 1,
                    UserId = 1,
                    DocumentName = "Electricity Bill Jan 2025",
                    DocumentType = "AddressProof",
                    UploadedAt = new DateTime(2025, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                    DocumentData = utilBill
                },
                new DocumentUpload
                {
                    DocumentId = 2,
                    LoanApplicationBaseId = 2,
                    UserId = 2,
                    DocumentName = "Pay Stub October 2025",
                    DocumentType = "IncomeProof",
                    UploadedAt = new DateTime(2025, 10, 20, 16, 0, 0, DateTimeKind.Utc),
                    DocumentData = payStub
                });

            // --------------------------------------------------------------------
            // 12. APPLICATION DOCUMENT LINKS
            // --------------------------------------------------------------------
            b.Entity<ApplicationDocumentLink>().HasData(
                new ApplicationDocumentLink
                {
                    LoanApplicationBaseId = 1,
                    DocumentId = 1,
                    DocumentRequirementType = DocumentType.Other,
                    LinkedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    Status = DocumentStatus.Pending,
                    VerificationNotes =null,
                    VerifiedAt = null,
                    VerifiedBy = null
                },
                new ApplicationDocumentLink
                {
                    LoanApplicationBaseId = 2,
                    DocumentId = 2,
                    DocumentRequirementType = DocumentType.Other,
                    LinkedAt = new DateTime(2025, 10, 17, 0, 0, 0, DateTimeKind.Utc),
                    Status = DocumentStatus.Pending,
                    VerificationNotes = null,
                    VerifiedAt = null,
                    VerifiedBy = null
                });

            // --------------------------------------------------------------------
            // 13. NOTIFICATIONS
            // --------------------------------------------------------------------
            b.Entity<Notification>().HasData(
                new Notification
                {
                    NotificationId = 1,
                    UserId = 1,
                    Title = "Loan Disbursed Successfully",
                    Message = "Your Personal Loan application has been approved and funds have been disbursed to your account.",
                    IsRead = false,
                    CreatedAt = new DateTime(2025, 2, 2, 9, 30, 0, DateTimeKind.Utc)
                },
                new Notification
                {
                    NotificationId = 2,
                    UserId = 2,
                    Title = "Payment Due Reminder",
                    Message = "A payment of ₹4,403.95 is due in 5 days for your Car Loan.",
                    IsRead = true,
                    CreatedAt = new DateTime(2025, 10, 15, 14, 0, 0, DateTimeKind.Utc)
                },
                new Notification
                {
                    NotificationId = 3,
                    UserId = 1,
                    Title = "Welcome to Loan Management Portal",
                    Message = "Thank you for registering with our loan management system.",
                    IsRead = true,
                    CreatedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                });

            // ====================================================================
            // 9. FINAL SAFETY NET
            // ====================================================================

            // Ensure all remaining relationships default to RESTRICT for safety
            foreach (var relationship in b.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}