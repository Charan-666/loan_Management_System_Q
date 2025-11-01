using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanProducts",
                columns: table => new
                {
                    LoanProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoanProductDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanProducts", x => x.LoanProductId);
                    table.UniqueConstraint("AK_LoanProducts_LoanProductName", x => x.LoanProductName);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplicationBases",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanProductType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplicationBases", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_LoanApplicationBases_LoanProducts_LoanProductType",
                        column: x => x.LoanProductType,
                        principalTable: "LoanProducts",
                        principalColumn: "LoanProductName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnnualIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HomeOwnershipStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentUploads",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUploads", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_DocumentUploads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentUploads_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AddressInformations",
                columns: table => new
                {
                    AddressInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PresentAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MobileNumber1 = table.Column<int>(type: "int", nullable: false),
                    MobileNumber2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressInformations", x => x.AddressInformationId);
                    table.ForeignKey(
                        name: "FK_AddressInformations_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Declarations",
                columns: table => new
                {
                    DeclarationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declarations", x => x.DeclarationId);
                    table.ForeignKey(
                        name: "FK_Declarations_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentDetails",
                columns: table => new
                {
                    EmploymentDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OfficeAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentDetails", x => x.EmploymentDetailsId);
                    table.ForeignKey(
                        name: "FK_EmploymentDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FamilyEmergencyDetails",
                columns: table => new
                {
                    FamilyEmergencyDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RelationshipWithApplicant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MobileNumber = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyEmergencyDetails", x => x.FamilyEmergencyDetailsId);
                    table.ForeignKey(
                        name: "FK_FamilyEmergencyDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialInformations",
                columns: table => new
                {
                    FinancialInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    Rent = table.Column<int>(type: "int", nullable: false),
                    PrimaryOther = table.Column<int>(type: "int", nullable: false),
                    RentandUtility = table.Column<int>(type: "int", nullable: false),
                    FoodandClothing = table.Column<int>(type: "int", nullable: false),
                    Education = table.Column<int>(type: "int", nullable: false),
                    LoanRepayment = table.Column<int>(type: "int", nullable: false),
                    ExpenseOther = table.Column<int>(type: "int", nullable: false),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialInformations", x => x.FinancialInformationId);
                    table.ForeignKey(
                        name: "FK_FinancialInformations_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HomeLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeLoanApplications", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_HomeLoanApplications_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDetails",
                columns: table => new
                {
                    LoanDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MonthlyInstallment = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDetails", x => x.LoanDetailsId);
                    table.ForeignKey(
                        name: "FK_LoanDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanOriginationWorkflows",
                columns: table => new
                {
                    WorkflowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<int>(type: "int", nullable: false),
                    StepStatus = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    ManagerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanOriginationWorkflows", x => x.WorkflowId);
                    table.ForeignKey(
                        name: "FK_LoanOriginationWorkflows_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalDetails",
                columns: table => new
                {
                    PersonalDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DistrictOfBirth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryOfBirth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PANNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EducationQualification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResidentialStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    SignatureImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IDProofImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDetails", x => x.PersonalDetailsId);
                    table.ForeignKey(
                        name: "FK_PersonalDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalLoanApplications", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_PersonalLoanApplications_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLoanApplications", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_VehicleLoanApplications_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanAccounts",
                columns: table => new
                {
                    LoanAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CurrentPaymentStatus = table.Column<int>(type: "int", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaysPastDue = table.Column<int>(type: "int", nullable: false),
                    LastStatusUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaidPrincipal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaidInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalRemaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalLateFeePaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanAccounts", x => x.LoanAccountId);
                    table.ForeignKey(
                        name: "FK_LoanAccounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanAccounts_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplicants",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ApplicantRole = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplicants", x => new { x.LoanApplicationBaseId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_LoanApplicants_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanApplicants_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationDocumentLinks",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    DocumentRequirementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDocumentLinks", x => new { x.LoanApplicationBaseId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_ApplicationDocumentLinks_DocumentUploads_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "DocumentUploads",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationDocumentLinks_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuilderInformations",
                columns: table => new
                {
                    BuilderInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BuilderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BuilderRegistrationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuilderInformations", x => x.BuilderInformationId);
                    table.ForeignKey(
                        name: "FK_BuilderInformations_HomeLoanApplications_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "HomeLoanApplications",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HomeLoanDetails",
                columns: table => new
                {
                    HomeLoanDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    PropertyCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanPurpose = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeLoanDetails", x => x.HomeLoanDetailsId);
                    table.ForeignKey(
                        name: "FK_HomeLoanDetails_HomeLoanApplications_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "HomeLoanApplications",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyDetails",
                columns: table => new
                {
                    PropertyDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    PropertyAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    OwnershipType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDetails", x => x.PropertyDetailsId);
                    table.ForeignKey(
                        name: "FK_PropertyDetails_HomeLoanApplications_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "HomeLoanApplications",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DealerInformations",
                columns: table => new
                {
                    DealerInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DealerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DealerAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealerInformations", x => x.DealerInformationId);
                    table.ForeignKey(
                        name: "FK_DealerInformations_VehicleLoanApplications_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "VehicleLoanApplications",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInformations",
                columns: table => new
                {
                    VehicleInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Variant = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ManufacturingYear = table.Column<int>(type: "int", nullable: false),
                    VehicleCondition = table.Column<int>(type: "int", nullable: false),
                    ExShowroomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInformations", x => x.VehicleInformationId);
                    table.ForeignKey(
                        name: "FK_VehicleInformations_VehicleLoanApplications_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "VehicleLoanApplications",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoanDetails",
                columns: table => new
                {
                    VehicleLoanDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    OnRoadPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanPurposeVehicle = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLoanDetails", x => x.VehicleLoanDetailsId);
                    table.ForeignKey(
                        name: "FK_VehicleLoanDetails_VehicleLoanApplications_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "VehicleLoanApplications",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMIPlans",
                columns: table => new
                {
                    EMIId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    LoanAccountId = table.Column<int>(type: "int", nullable: false),
                    PrincipleAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TermMonths = table.Column<int>(type: "int", nullable: false),
                    RateOfInterest = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MonthlyEMI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInterestPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMIPlans", x => x.EMIId);
                    table.ForeignKey(
                        name: "FK_EMIPlans_LoanAccounts_LoanAccountId",
                        column: x => x.LoanAccountId,
                        principalTable: "LoanAccounts",
                        principalColumn: "LoanAccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMIId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LoanAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_EMIPlans_EMIId",
                        column: x => x.EMIId,
                        principalTable: "EMIPlans",
                        principalColumn: "EMIId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "LoanProducts",
                columns: new[] { "LoanProductId", "IsActive", "LoanProductDescription", "LoanProductName" },
                values: new object[,]
                {
                    { 1, true, "Unsecured loan for personal use, high flexibility, terms up to 7 years.", "PersonalLoan-Standard" },
                    { 2, true, "Secured loan for pre-owned vehicles, up to 90% financing, maximum term 5 years.", "CarLoan-Used" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FullName", "PasswordHash", "Roles", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "alex.johnson@customer.com", "Alex Johnson", "hashedpassword_alex", 0, 0, null },
                    { 2, new DateTime(2025, 10, 1, 15, 0, 0, 0, DateTimeKind.Utc), "emma.brown@customer.com", "Emma Brown", "hashedpassword_emma", 0, 0, null },
                    { 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "alice.smith@manager.com", "Alice Smith (Credit)", "hashedpassword_alice", 1, 0, null },
                    { 4, new DateTime(2024, 2, 20, 9, 30, 0, 0, DateTimeKind.Utc), "bob.davis@manager.com", "Bob Davis (Verification)", "hashedpassword_bob", 1, 0, null }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "AnnualIncome", "CreditScore", "DateOfBirth", "Gender", "HomeOwnershipStatus", "Occupation", "PhoneNumber", "ProfileImage", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 1500000.00m, 780m, new DateOnly(1995, 10, 20), 0, 1, "Software Engineer", "9876543210", new byte[] { 222, 173, 190, 239 }, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, 950000.00m, 725m, new DateOnly(1998, 5, 15), 1, 0, "Data Analyst", "8012345678", new byte[] { 222, 173, 190, 239 }, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "DocumentUploads",
                columns: new[] { "DocumentId", "DocumentData", "DocumentName", "DocumentType", "LoanApplicationBaseId", "UploadedAt", "UserId", "UserId1" },
                values: new object[,]
                {
                    { 1, new byte[] { 85, 84, 73, 76 }, "Electricity Bill Jan 2025", "AddressProof", 1, new DateTime(2025, 1, 15, 11, 0, 0, 0, DateTimeKind.Utc), 1, null },
                    { 2, new byte[] { 80, 65, 89, 83 }, "Pay Stub October 2025", "IncomeProof", 2, new DateTime(2025, 10, 20, 16, 0, 0, 0, DateTimeKind.Utc), 2, null }
                });

            migrationBuilder.InsertData(
                table: "LoanApplicationBases",
                columns: new[] { "LoanApplicationBaseId", "ApprovedDate", "IsActive", "LoanProductType", "RejectionReason", "Status", "SubmissionDate" },
                values: new object[,]
                {
                    { 1, new DateOnly(2025, 1, 25), true, "PersonalLoan-Standard", null, 4, new DateOnly(2025, 1, 15) },
                    { 2, null, true, "CarLoan-Used", null, 1, new DateOnly(2025, 10, 20) }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationId", "CreatedAt", "IsRead", "Message", "Title", "UserId", "UserId1" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 2, 9, 30, 0, 0, DateTimeKind.Utc), false, "Your Personal Loan application has been approved and funds have been disbursed to your account.", "Loan Disbursed Successfully", 1, null },
                    { 2, new DateTime(2025, 10, 15, 14, 0, 0, 0, DateTimeKind.Utc), true, "A payment of ₹4,403.95 is due in 5 days for your Car Loan.", "Payment Due Reminder", 2, null },
                    { 3, new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), true, "Thank you for registering with our loan management system.", "Welcome to Loan Management Portal", 1, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationDocumentLinks",
                columns: new[] { "DocumentId", "LoanApplicationBaseId", "DocumentRequirementType", "LinkedAt" },
                values: new object[,]
                {
                    { 1, 1, "AddressProof-UtilityBill", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "IncomeProof-PayStub", new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "LoanAccounts",
                columns: new[] { "LoanAccountId", "CurrentPaymentStatus", "CustomerId", "DaysPastDue", "DisbursementDate", "LastPaymentDate", "LastStatusUpdate", "LoanApplicationBaseId", "PrincipalRemaining", "TotalLateFeePaidAmount", "TotalLoanAmount", "TotalPaidInterest", "TotalPaidPrincipal" },
                values: new object[,]
                {
                    { 1, 0, 1, 0, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 350000.00m, 0.00m, 500000.00m, 45000.00m, 150000.00m },
                    { 2, 6, 2, 0, new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0.00m, 150.00m, 50000.00m, 2847.40m, 50000.00m }
                });

            migrationBuilder.InsertData(
                table: "LoanApplicants",
                columns: new[] { "CustomerId", "LoanApplicationBaseId", "AddedDate", "ApplicantRole" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 2, 1, new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                table: "LoanDetails",
                columns: new[] { "LoanDetailsId", "AppliedDate", "InterestRate", "LoanApplicationBaseId", "LoanApplicationId", "MonthlyInstallment", "RequestedAmount", "TenureMonths" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), 12.00m, 1, 0, 11122.22m, 500000.00m, 60 },
                    { 2, new DateTime(2025, 10, 20, 15, 30, 0, 0, DateTimeKind.Utc), 10.00m, 2, 0, 4403.95m, 50000.00m, 12 }
                });

            migrationBuilder.InsertData(
                table: "LoanOriginationWorkflows",
                columns: new[] { "WorkflowId", "CompletionDate", "LoanApplicationBaseId", "ManagerId", "ManagerNotes", "StepName", "StepStatus" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 20, 14, 30, 0, 0, DateTimeKind.Utc), 1, 3, "Approved with standard rate.", 1, 0 },
                    { 2, null, 2, 4, "Awaiting verification report.", 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "PersonalLoanApplications",
                column: "LoanApplicationBaseId",
                value: 1);

            migrationBuilder.InsertData(
                table: "VehicleLoanApplications",
                column: "LoanApplicationBaseId",
                value: 2);

            migrationBuilder.InsertData(
                table: "EMIPlans",
                columns: new[] { "EMIId", "IsCompleted", "LoanAccountId", "LoanApplicationBaseId", "MonthlyEMI", "PrincipleAmount", "RateOfInterest", "Status", "TermMonths", "TotalInterestPaid", "TotalRepaymentAmount" },
                values: new object[,]
                {
                    { 1, false, 1, 1, 11122.22m, 500000.00m, 12.00m, 0, 60, 167333.20m, 667333.20m },
                    { 2, true, 2, 2, 4403.95m, 50000.00m, 10.00m, 2, 12, 2847.40m, 52847.40m }
                });

            migrationBuilder.InsertData(
                table: "PaymentTransactions",
                columns: new[] { "TransactionId", "Amount", "CreatedAt", "EMIId", "IsActive", "LoanAccountId", "PaymentDate", "PaymentMethod", "Status", "TransactionReference", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 11122.22m, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, 1, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, "UPI-TXN-1234567890", null },
                    { 2, 11122.22m, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, 1, new DateTime(2025, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "NB-TXN-0987654321", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressInformations_LoanApplicationBaseId",
                table: "AddressInformations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocumentLinks_DocumentId",
                table: "ApplicationDocumentLinks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BuilderInformations_LoanApplicationBaseId",
                table: "BuilderInformations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DealerInformations_LoanApplicationBaseId",
                table: "DealerInformations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Declarations_LoanApplicationBaseId",
                table: "Declarations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_UserId",
                table: "DocumentUploads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_UserId1",
                table: "DocumentUploads",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_EMIPlans_LoanAccountId",
                table: "EMIPlans",
                column: "LoanAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentDetails_LoanApplicationBaseId",
                table: "EmploymentDetails",
                column: "LoanApplicationBaseId",
                unique: true,
                filter: "[LoanApplicationBaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyEmergencyDetails_LoanApplicationBaseId",
                table: "FamilyEmergencyDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialInformations_LoanApplicationBaseId",
                table: "FinancialInformations",
                column: "LoanApplicationBaseId",
                unique: true,
                filter: "[LoanApplicationBaseId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HomeLoanDetails_LoanApplicationBaseId",
                table: "HomeLoanDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanAccounts_CustomerId",
                table: "LoanAccounts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanAccounts_LoanApplicationBaseId",
                table: "LoanAccounts",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicants_CustomerId",
                table: "LoanApplicants",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicationBases_LoanProductType",
                table: "LoanApplicationBases",
                column: "LoanProductType");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDetails_LoanApplicationBaseId",
                table: "LoanDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanOriginationWorkflows_LoanApplicationBaseId",
                table: "LoanOriginationWorkflows",
                column: "LoanApplicationBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_EMIId",
                table: "PaymentTransactions",
                column: "EMIId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_LoanApplicationBaseId",
                table: "PersonalDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_LoanApplicationBaseId",
                table: "PropertyDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInformations_LoanApplicationBaseId",
                table: "VehicleInformations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoanDetails_LoanApplicationBaseId",
                table: "VehicleLoanDetails",
                column: "LoanApplicationBaseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressInformations");

            migrationBuilder.DropTable(
                name: "ApplicationDocumentLinks");

            migrationBuilder.DropTable(
                name: "BuilderInformations");

            migrationBuilder.DropTable(
                name: "DealerInformations");

            migrationBuilder.DropTable(
                name: "Declarations");

            migrationBuilder.DropTable(
                name: "EmploymentDetails");

            migrationBuilder.DropTable(
                name: "FamilyEmergencyDetails");

            migrationBuilder.DropTable(
                name: "FinancialInformations");

            migrationBuilder.DropTable(
                name: "HomeLoanDetails");

            migrationBuilder.DropTable(
                name: "LoanApplicants");

            migrationBuilder.DropTable(
                name: "LoanDetails");

            migrationBuilder.DropTable(
                name: "LoanOriginationWorkflows");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PersonalDetails");

            migrationBuilder.DropTable(
                name: "PersonalLoanApplications");

            migrationBuilder.DropTable(
                name: "PropertyDetails");

            migrationBuilder.DropTable(
                name: "VehicleInformations");

            migrationBuilder.DropTable(
                name: "VehicleLoanDetails");

            migrationBuilder.DropTable(
                name: "DocumentUploads");

            migrationBuilder.DropTable(
                name: "EMIPlans");

            migrationBuilder.DropTable(
                name: "HomeLoanApplications");

            migrationBuilder.DropTable(
                name: "VehicleLoanApplications");

            migrationBuilder.DropTable(
                name: "LoanAccounts");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "LoanApplicationBases");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "LoanProducts");
        }
    }
}
