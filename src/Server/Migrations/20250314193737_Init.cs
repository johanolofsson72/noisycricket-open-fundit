using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationBudgetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationBudgetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ProjectId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ProjectNumber = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "000000"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SchemaId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SchemaNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchemaClaimTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Number = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "000000"),
                    NewEventCounter = table.Column<int>(type: "int", nullable: false),
                    NewAuditCounter = table.Column<int>(type: "int", nullable: false),
                    InternalBudgetsTotalAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    MilestonePayoutTotalAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    EarlierSupportTotalAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    InternalBudgetsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BudgetAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    OurContribution = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SignedContractDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Applicant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Audits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Controls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistributionManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Events = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinanceManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InternalBudgets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Producer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Progress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredDocuments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScriptManager = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UserId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ApplicationId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SchemaId = table.Column<int>(type: "int", nullable: false),
                    SchemaNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    TempPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Controls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ControlTypeId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ControlTypeName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    BaseStructure = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentDeliveryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDeliveryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RequirementTypeId = table.Column<int>(type: "int", nullable: false),
                    DeliveryTypeId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    MimeType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Extension = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: ""),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Phrases = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false, defaultValue: ""),
                    Summarize = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false, defaultValue: ""),
                    Binary = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsCertified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeliverDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CertifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    LockedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GridLayouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GridName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GridStateJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridLayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageQueue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    MessageTypeId = table.Column<int>(type: "int", nullable: false),
                    Outgoing = table.Column<bool>(type: "bit", nullable: false),
                    Incoming = table.Column<bool>(type: "bit", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ProjectTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    ProjectNumber = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    ApplicationId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ApplicationTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    ApplicationStatusId = table.Column<int>(type: "int", nullable: false),
                    SchemaId = table.Column<int>(type: "int", nullable: false),
                    SchemaNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    ReactionDescription = table.Column<int>(type: "int", nullable: false),
                    SystemMessageDestination = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Receiver = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MilestoneRequirementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilestoneRequirementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequirementsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RequirementsDeliveredCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RequirementsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequirementsExpired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TotalPayments = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    Payments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenAiCacheItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnCount = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAiCacheItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenAiProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ProjectTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectManagerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProducerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    SchemaNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OurContribution = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Embedding = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAiProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenAiUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Organizations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAiUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Vat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Mail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    Logo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false, defaultValue: ""),
                    Country = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Addresses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationBudgetTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currencies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentDeliveryTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Genders = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MilestoneRequirementTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReactionTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Statuses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemMessageDestinations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumberTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumberTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: "000000"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Applications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReactionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Messages = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimTag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Controls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Events = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Progress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiredDocuments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchIndex",
                columns: table => new
                {
                    RowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<decimal>(type: "decimal(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchIndex", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatisticIdentifier = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Query = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Columns = table.Column<int>(type: "int", nullable: false),
                    Rows = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemMessageDestinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMessageDestinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    LastName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    LastLoginDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastProject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    VisibleApplicationTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Favorites = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Addresses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Organizations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumbers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Statistics = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "Id", "Names" },
                values: new object[,]
                {
                    { 1, "[\"Standard\",\"Default\",\"Standard\",\"Standard\",\"Defecto\",\"Par d\\u00E9faut\",\"Standard\",\"Standard\"]" },
                    { 3, "[\"Beskrivning av vad utvecklingen avser\",\"Description of what the development pertains to\",\"Beskrivelse af hvad udviklingen vedr\\u00F8rer\",\"Beschreibung dessen, was die Entwicklung betrifft\",\"Descripci\\u00F3n de a qu\\u00E9 se refiere el desarrollo\",\"Description de ce que le d\\u00E9veloppement concerne\",\"Descrizione di cosa riguarda lo sviluppo\",\"Beskrivelse av hva utviklingen gjelder\"]" },
                    { 6, "[\"Bekr\\u00E4ftad finansiering dokumentation\",\"Documentation of confirmed financing\",\"Dokumentation af bekr\\u00E6ftet finansiering\",\"Dokumentation der best\\u00E4tigten Finanzierung\",\"Documentaci\\u00F3n de la financiaci\\u00F3n confirmada\",\"Documentation de financement confirm\\u00E9\",\"Documentazione di finanziamento confermata\",\"Dokumentasjon av bekreftet finansiering\"]" },
                    { 9, "[\"Faktura f\\u00F6r rat 1\",\"Invoice for rate 1\",\"Faktura f\\u00F6r rat 1\",\"Invoice for rate 1\",\"Invoice for rate 1\",\"Invoice for rate 1\",\"Invoice for rate 1\",\"Faktura f\\u00F6r rat 1\"]" },
                    { 10, "[\"\\u00D6vrigt material\",\"Other material\",\"Andet materiale\",\"Anderes Material\",\"Otro material\",\"Autre mat\\u00E9riel\",\"Altro materiale\",\"Annet materiale\"]" },
                    { 12, "[\"Projektbeskrivning\",\"Project description\",\"Projektbeskrivelse\",\"Projektbeschreibung\",\"Descripci\\u00F3n del proyecto\",\"Description du projet\",\"Descrizione del progetto\",\"Prosjektbeskrivelse\"]" },
                    { 14, "[\"Manus\",\"Script\",\"Manuskript\",\"Drehbuch\",\"Gui\\u00F3n\",\"Sc\\u00E9nario\",\"Sceneggiatura\",\"Manus\"]" },
                    { 16, "[\"Synopsis/Treatment\",\"Synopsis/Treatment\",\"Synopsis/Behandling\",\"Synopsis/Behandlung\",\"Sinopsis/Tratamiento\",\"Synopsis/Traitement\",\"Sinossi/Trattamento\",\"Synopsis/Behandling\"]" },
                    { 17, "[\"Tidsplan\",\"Timeline\",\"Tidsplan\",\"Zeitplan\",\"Cronograma\",\"Calendrier\",\"Pianificazione\",\"Tidsplan\"]" },
                    { 21, "[\"Budget\",\"Budget\",\"Budget\",\"Budget\",\"Presupuesto\",\"Budget\",\"Bilancio\",\"Budsjett\"]" },
                    { 22, "[\"Finansieringsplan\",\"Financing Plan\",\"Finansieringsplan\",\"Finanzierungsplan\",\"Plan de financiaci\\u00F3n\",\"Plan de financement\",\"Piano di finanziamento\",\"Finansieringsplan\"]" },
                    { 59, "[\"Rekommendation till styrelsen\",\"Recommendation to the Board\",\"Anbefaling til bestyrelsen\",\"Empfehlung an den Vorstand\",\"Recomendaci\\u00F3n al consejo\",\"Recommendation au conseil\",\"Raccomandazione al consiglio\",\"Anbefaling til styret\"]" },
                    { 60, "[\"Avslagsbrev\",\"Rejection Letter\",\"Afslagbrev\",\"Ablehnungsbrief\",\"Carta de rechazo\",\"Lettre de rejet\",\"Lettera di rifiuto\",\"Avslagsbrev\"]" },
                    { 66, "[\"Standardmall\",\"General template\",\"Standardmall\",\"General template\",\"General template\",\"General template\",\"General template\",\"Standardmall\"]" },
                    { 69, "[\"Uppladdade instillinger\",\"Uploaded Settings\",\"Uploadede indstillinger\",\"Hochgeladene Einstellungen\",\"Configuraciones subidas\",\"Param\\u00E8tres t\\u00E9l\\u00E9charg\\u00E9s\",\"Impostazioni caricate\",\"Lastede opp innstillinger\"]" },
                    { 70, "[\"Interne vurderinger\",\"Internal Assessments\",\"Interne vurderinger\",\"Interne Bewertungen\",\"Evaluaciones internas\",\"\\u00C9valuations internes\",\"Valutazioni interne\",\"Interne vurderinger\"]" },
                    { 74, "[\"Disponert bevilgning\",\"Allocated Grant\",\"Bevilget tilskud\",\"Zugewiesener Zuschuss\",\"Subvenci\\u00F3n asignada\",\"Subvention allou\\u00E9e\",\"Contributo assegnato\",\"Tildelt tilskudd\"]" },
                    { 75, "[\"Team- och sk\\u00E5despelarlista\",\"Team and Cast List\",\"Team og skuespillerliste\",\"Team- und Schauspielerliste\",\"Lista de equipo y reparto\",\"Liste de l\\u0027\\u00E9quipe et des acteurs\",\"Elenco del team e del cast\",\"Team og skuespillerliste\"]" },
                    { 78, "[\"Spendbudget\",\"Expenditure Budget\",\"Udgiftsbudget\",\"Ausgabenbudget\",\"Presupuesto de gastos\",\"Budget des d\\u00E9penses\",\"Budget delle spese\",\"Utgiftsbudsjett\"]" },
                    { 79, "[\"Regiavtal\",\"Director\\u0027s Agreement\",\"Instrukt\\u00F8raftale\",\"Regievertrag\",\"Acuerdo del director\",\"Accord de r\\u00E9alisation\",\"Contratto di regia\",\"Regiavtale\"]" },
                    { 80, "[\"FIV riktlinjer\",\"FIV Guidelines\",\"FIV-retningslinjer\",\"FIV-Richtlinien\",\"Directrices FIV\",\"Directives FIV\",\"Linee guida FIV\",\"FIV-retningslinjer\"]" },
                    { 81, "[\"Manusavtal\",\"Script Agreement\",\"Manusaftale\",\"Drehbuchvertrag\",\"Acuerdo de gui\\u00F3n\",\"Accord de sc\\u00E9nario\",\"Accordo di sceneggiatura\",\"Manusavtale\"]" },
                    { 82, "[\"Recoupmentplan\",\"Recoupment Plan\",\"Tilbagebetalingsplan\",\"R\\u00FCckzahlungsplan\",\"Plan de recuperaci\\u00F3n\",\"Plan de recouvrement\",\"Piano di recupero\",\"Tilbakebetalingsplan\"]" },
                    { 83, "[\"R\\u00E4ttighetsuppl\\u00E5telse\",\"Grant of Rights\",\"Rettighedsoverlade\",\"Rechteeinr\\u00E4umung\",\"Cesi\\u00F3n de derechos\",\"Cession de droits\",\"Cessione dei diritti\",\"Tilgang av rettigheter\"]" },
                    { 84, "[\"Distributionsavtal\",\"Distribution Agreement\",\"Distributionsaftale\",\"Vertriebsvereinbarung\",\"Acuerdo de distribuci\\u00F3n\",\"Accord de distribution\",\"Contratto di distribuzione\",\"Distribusjonsavtale\"]" },
                    { 85, "[\"CAMA / (LOC)\",\"CAMA / (LOC)\",\"CAMA / (LOC)\",\"CAMA / (LOC)\",\"CAMA / (LOC)\",\"CAMA / (LOC)\",\"CAMA / (LOC)\",\"CAMA / (LOC)\"]" },
                    { 86, "[\"Likviditetsplan\",\"Liquidity Plan\",\"Likviditetsplan\",\"Liquidit\\u00E4tsplan\",\"Plan de liquidez\",\"Plan de liquidit\\u00E9\",\"Piano di liquidit\\u00E0\",\"Likviditetsplan\"]" },
                    { 87, "[\"Tidsplan / Produktionsplan\",\"Timeline / Production Plan\",\"Tidsplan / Produktionsplan\",\"Zeitplan / Produktionsplan\",\"Cronograma / Plan de producci\\u00F3n\",\"Calendrier / Plan de production\",\"Pianificazione / Piano di produzione\",\"Tidsplan / Produksjonsplan\"]" },
                    { 88, "[\"Producentens egeninsats\",\"Producer\\u0027s own input\",\"Producentens egen indsats\",\"Eigene Leistung des Produzenten\",\"La propia aportaci\\u00F3n del productor\",\"Contribution propre du producteur\",\"Contributo personale del produttore\",\"Produsentens egen innsats\"]" },
                    { 89, "[\"Internationellt huvudavtal\",\"International Main Agreement\",\"International hovedaftale\",\"Internationaler Hauptvertrag\",\"Acuerdo principal internacional\",\"Accord principal international\",\"Accordo principale internazionale\",\"Internasjonal hovedavtale\"]" },
                    { 90, "[\"Salesavtal\",\"Sales Agreement\",\"Salgsafltale\",\"Verkaufsvertrag\",\"Contrato de ventas\",\"Contrat de vente\",\"Contratto di vendita\",\"Salgsavtale\"]" },
                    { 91, "[\"Locationlista\",\"Location List\",\"Stedliste\",\"Standortliste\",\"Lista de ubicaciones\",\"Liste des lieux\",\"Elenco delle localit\\u00E0\",\"Lokasjonsliste\"]" },
                    { 92, "[\"CAMAavtal\",\"CAMA Agreement\",\"CAMA-aftale\",\"CAMA-Vereinbarung\",\"Acuerdo CAMA\",\"Accord CAMA\",\"Accordo CAMA\",\"CAMA-avtale\"]" },
                    { 93, "[\"Bondavtal\",\"Bond Agreement\",\"Bond-aftale\",\"Anleihevertrag\",\"Contrato de bonos\",\"Contrat d\\u0027obligations\",\"Contratto obbligazionario\",\"Obligasjonsavtale\"]" },
                    { 94, "[\"Side letters\",\"Side Letters\",\"Side breve\",\"Seitenbriefe\",\"Cartas marginales\",\"Lettres de c\\u00F4t\\u00E9\",\"Lettere accessorie\",\"Sidebrev\"]" },
                    { 95, "[\"Avtal mellan parter\",\"Inter-Party Agreement\",\"Inter-party-aftale\",\"Inter-party-Vereinbarung\",\"Acuerdo entre partes\",\"Accord entre parties\",\"Accordo tra le parti\",\"Avtale mellom parter\"]" },
                    { 96, "[\"Till\\u00E4ggsavtal\",\"Supplementary Agreement\",\"Supplerende aftale\",\"Erg\\u00E4nzungsvereinbarung\",\"Acuerdo suplementario\",\"Accord suppl\\u00E9mentaire\",\"Accordo supplementare\",\"Tilleggsavtale\"]" },
                    { 97, "[\"Leverant\\u00F6rslista\",\"Supplier List\",\"Leverand\\u00F8rliste\",\"Lieferantenliste\",\"Lista de proveedores\",\"Liste des fournisseurs\",\"Elenco dei fornitori\",\"Leverand\\u00F8rliste\"]" },
                    { 98, "[\"Slutredovisning\",\"Final Report\",\"Slutrapport\",\"Endbericht\",\"Informe final\",\"Rapport final\",\"Rapporto finale\",\"Sluttrapport\"]" },
                    { 99, "[\"Upphovsr\\u00E4ttsavtal Manus och Regi\",\"Copyright Agreement Script and Direction\",\"Ophavsretsaftale Manuskript og Instruktion\",\"Urheberrechtsvereinbarung Drehbuch und Regie\",\"Acuerdo de derechos de autor Gui\\u00F3n y Direcci\\u00F3n\",\"Accord de droits d\\u0027auteur Sc\\u00E9nario et R\\u00E9alisation\",\"Accordo sui diritti d\\u0027autore Sceneggiatura e Regia\",\"Opphavsrettsavtale Manus og Regi\"]" },
                    { 100, "[\"Finansieringsunderlag \\u00F6vriga parter, LOC mm\",\"Financing Documentation for Other Parties, LOC etc.\",\"Finansieringsdokumentation for andre parter, LOC osv.\",\"Finanzierungsunterlagen f\\u00FCr andere Parteien, LOC usw.\",\"Documentaci\\u00F3n de financiaci\\u00F3n para otras partes, LOC etc.\",\"Documentation de financement pour d\\u0027autres parties, LOC, etc.\",\"Documentazione di finanziamento per altre parti, LOC ecc.\",\"Finansieringsdokumentasjon for andre parter, LOC mm\"]" },
                    { 111, "[\"Distributionsplan\",\"Distribution Plan\",\"Distributionsplan\",\"Vertriebsplan\",\"Plan de distribuci\\u00F3n\",\"Plan de distribution\",\"Piano di distribuzione\",\"Distribusjonsplan\"]" },
                    { 112, "[\"CV p\\u00E5 nyckelfunktioner\",\"CV of Key Personnel\",\"CV af n\\u00F8glepersonale\",\"CV der Schl\\u00FCsselpersonen\",\"CV del personal clave\",\"CV des personnels cl\\u00E9s\",\"CV del personale chiave\",\"CV av n\\u00F8kkelpersonell\"]" },
                    { 113, "[\"Ekonomisk redovisning\",\"Financial Report\",\"\\u00D8konomisk rapport\",\"Finanzbericht\",\"Informe financiero\",\"Rapport financier\",\"Rapporto finanziario\",\"Finansiell rapport\"]" },
                    { 114, "[\"Konstn\\u00E4rlig redovisning\",\"Artistic Report\",\"Kunstnerisk rapport\",\"K\\u00FCnstlerischer Bericht\",\"Informe art\\u00EDstico\",\"Rapport artistique\",\"Rapporto artistico\",\"Kunstnerisk rapport\"]" }
                });

            migrationBuilder.InsertData(
                table: "MessageTypes",
                columns: new[] { "Id", "Names" },
                values: new object[,]
                {
                    { 1, "[\"Koppla applikation till projekt\",\"Connect application to project\",\"Tilslut applikation til projekt\",\"Anwendung mit Projekt verbinden\",\"Conectar aplicaci\\u00F3n al proyecto\",\"Connecter l\\u0027application au projet\",\"Collegare l\\u0027applicazione al progetto\",\"Koble applikasjonen til prosjektet\"]" },
                    { 9, "[\"Skapa avslagsbrev\",\"Create rejection letter\",\"Opret afslag brev\",\"Ablehnungsbrief erstellen\",\"Crear carta de rechazo\",\"Cr\\u00E9er une lettre de rejet\",\"Creare la lettera di rifiuto\",\"Opprett avslagsbrev\"]" },
                    { 22, "[\"Application kopplad till projekt\",\"Application linked to project\",\"Ans\\u00F8gning forbundet til projekt\",\"Anwendung mit Projekt verkn\\u00FCpft\",\"Aplicaci\\u00F3n vinculada al proyecto\",\"Application li\\u00E9e au projet\",\"Applicazione collegata al progetto\",\"S\\u00F8knad knyttet til prosjektet\"]" },
                    { 901, "[\"Bed\\u00F6mning ja\",\"Assessment yes\",\"Vurdering ja\",\"Beurteilung ja\",\"Evaluaci\\u00F3n s\\u00ED\",\"\\u00C9valuation oui\",\"Valutazione s\\u00EC\",\"Vurdering ja\"]" },
                    { 902, "[\"Bed\\u00F6mning nej\",\"Assessment no\",\"Vurdering nej\",\"Beurteilung nein\",\"Evaluaci\\u00F3n no\",\"\\u00C9valuation non\",\"Valutazione no\",\"Vurdering nei\"]" },
                    { 903, "[\"Bed\\u00F6mning tilldela roller\",\"Assessment assign roles\",\"Vurdering tildele roller\",\"Beurteilung Rollen zuweisen\",\"Evaluaci\\u00F3n asignar roles\",\"\\u00C9valuation attribuer des r\\u00F4les\",\"Valutazione assegnare ruoli\",\"Vurdering tildele roller\"]" },
                    { 916, "[\"Bed\\u00F6mning nej och st\\u00E4ng\",\"Assessment no and close\",\"Vurdering nei og lukk\",\"Beurteilung nein und schlie\\u00DFen\",\"Evaluaci\\u00F3n no y cerrar\",\"\\u00C9valuation non et fermer\",\"Valutazione no e chiudi\",\"Vurdering nei og steng\"]" },
                    { 1001, "[\"Ny anv\\u00E4ndare\",\"New user\",\"Ny bruger\",\"Neuer Benutzer\",\"Nuevo usuario\",\"Nouvel utilisateur\",\"Nuovo utente\",\"Ny bruker\"]" },
                    { 1002, "[\"Fr\\u00E5n client\",\"From client\",\"Fra klient\",\"Vom Client\",\"Desde el cliente\",\"Du client\",\"Dal client\",\"Fra klient\"]" },
                    { 1003, "[\"Till client\",\"To client\",\"Til klient\",\"Zum Client\",\"Al cliente\",\"Au client\",\"Al client\",\"Til client\"]" },
                    { 1004, "[\"AutoStatus1\",\"AutoStatus1\",\"AutoStatus1\",\"AutoStatus1\",\"AutoStatus1\",\"AutoStatus1\",\"AutoStatus1\",\"AutoStatus1\"]" },
                    { 1005, "[\"AutoStatus2\",\"AutoStatus2\",\"AutoStatus2\",\"AutoStatus2\",\"AutoStatus2\",\"AutoStatus2\",\"AutoStatus2\",\"AutoStatus2\"]" },
                    { 1006, "[\"Fra client wizard\",\"From client wizard\",\"Fra klientguide\",\"Vom Client-Assistenten\",\"Desde el asistente del cliente\",\"Du client assistant\",\"Dal wizard client\",\"Fra klientveiviser\"]" },
                    { 1007, "[\"Til client wizard\",\"To client wizard\",\"Til klientguide\",\"Zum Client-Assistenten\",\"Al asistente del cliente\",\"Au client assistant\",\"Al wizard client\",\"Til klientveiviser\"]" },
                    { 2001, "[\"Avancerad\",\"Advanced\",\"Avanceret\",\"Erweitert\",\"Avanzado\",\"Avanc\\u00E9\",\"Avanzato\",\"Avansert\"]" },
                    { 2002, "[\"Ofullst\\u00E4ndig\",\"Incomplete\",\"Ufuldst\\u00E6ndig\",\"Unvollst\\u00E4ndig\",\"Incompleto\",\"Incomplet\",\"Incompleto\",\"Ufullstendig\"]" },
                    { 2003, "[\"Klar f\\u00F6r greenlight\",\"Ready for greenlight\",\"Klar til gr\\u00F8nt lys\",\"Bereit f\\u00FCr gr\\u00FCnes Licht\",\"Listo para la luz verde\",\"Pr\\u00EAt pour le feu vert\",\"Pronto per il via libera\",\"Klar til gr\\u00F8nt lys\"]" },
                    { 2004, "[\"Fler dokument\",\"More documents\",\"Flere dokumenter\",\"Mehr Dokumente\",\"M\\u00E1s documentos\",\"Plus de documents\",\"Pi\\u00F9 documenti\",\"Flere dokumenter\"]" },
                    { 3001, "[\"Requirements\",\"Requirements\",\"Krav\",\"Anforderungen\",\"Requisitos\",\"Exigences\",\"Requisiti\",\"Krav\"]" },
                    { 4001, "[\"Nekat projekt att registrera\",\"Denied project to register\",\"N\\u00E6gtet projekt at registrere\",\"Projekt abgelehnt zu registrieren\",\"Proyecto denegado para registrar\",\"Projet refus\\u00E9 \\u00E0 enregistrer\",\"Progetto negato da registrare\",\"Prosjekt avvist for registrering\"]" },
                    { 4002, "[\"Beslutsbrev att registrera\",\"Decision letter to register\",\"Afg\\u00F8relsesbrev at registrere\",\"Entscheidungsbrief zu registrieren\",\"Carta de decisi\\u00F3n para registrar\",\"Lettre de d\\u00E9cision \\u00E0 enregistrer\",\"Lettera di decisione da registrare\",\"Beslutningsbrev for registrering\"]" },
                    { 4003, "[\"M\\u00F6tesprotokoll att registrera\",\"Meeting protocol to register\",\"M\\u00F8deprotokol at registrere\",\"Protokoll der Sitzung zu registrieren\",\"Protocolo de reuni\\u00F3n para registrar\",\"Protocole de r\\u00E9union \\u00E0 enregistrer\",\"Protocollo della riunione da registrare\",\"M\\u00F8teprotokoll for registrering\"]" },
                    { 4004, "[\"Nekat projekt och st\\u00E4ng f\\u00F6r registrering\",\"Denied project and close to register\",\"N\\u00E6gtet projekt og luk for at registrere\",\"Projekt abgelehnt und schlie\\u00DFen zu registrieren\",\"Proyecto denegado y cerrar para registrar\",\"Projet refus\\u00E9 et fermer \\u00E0 enregistrer\",\"Progetto negato e chiudi per registrare\",\"Prosjekt avvist og lukk for registrering\"]" },
                    { 5001, "[\"Produktionsroll tilldelad\",\"Production role assigned\",\"Produktionsrolle tildelt\",\"Produktionsrolle zugewiesen\",\"Rol de producci\\u00F3n asignado\",\"R\\u00F4le de production attribu\\u00E9\",\"Ruolo di produzione assegnato\",\"Produksjonsrolle tildelt\"]" },
                    { 5002, "[\"Finansroll tilldelad\",\"Finance role assigned\",\"Finansrolle tildelt\",\"Finanzrolle zugewiesen\",\"Rol de finanzas asignado\",\"R\\u00F4le des finances attribu\\u00E9\",\"Ruolo finanziario assegnato\",\"Finansrolle tildelt\"]" },
                    { 5003, "[\"Manuskriptkonsult tilldelad\",\"Script consult assigned\",\"Manuskriptkonsulent tildelt\",\"Drehbuchberatung zugewiesen\",\"Consulta de gui\\u00F3n asignado\",\"Consultation de script attribu\\u00E9\",\"Consulenza per la sceneggiatura assegnata\",\"Manuskriptkonsulent tildelt\"]" },
                    { 5004, "[\"Distributionskonsult tilldelad\",\"Distribution consult assigned\",\"Distributionskonsulent tildelt\",\"Verteilungsberatung zugewiesen\",\"Consulta de distribuci\\u00F3n asignado\",\"Consultation de distribution attribu\\u00E9e\",\"Consulenza per la distribuzione assegnata\",\"Distributionskonsulent tildelt\"]" },
                    { 5005, "[\"Planera m\\u00F6te med produktionsgrupp\",\"Plan meeting with production group\",\"Planl\\u00E6g m\\u00F8de med produktionsgruppen\",\"Planen Sie ein Treffen mit der Produktionsgruppe\",\"Plan reuni\\u00F3n con el grupo de producci\\u00F3n\",\"Planifier une r\\u00E9union avec le groupe de production\",\"Pianifica una riunione con il gruppo di produzione\",\"Planlegg m\\u00F8te med produksjonsgruppen\"]" },
                    { 5006, "[\"Produktionsmeddelande\",\"Production message\",\"Produktionsbesked\",\"Produktionsnachricht\",\"Mensaje de producci\\u00F3n\",\"Message de production\",\"Messaggio di produzione\",\"Produksjonsmelding\"]" },
                    { 5007, "[\"Utvecklingsprocessens b\\u00F6rjan\",\"Development process beginning\",\"Udviklingsprocess start\",\"Beginn des Entwicklungsprozesses\",\"Inicio del proceso de desarrollo\",\"D\\u00E9but du processus de d\\u00E9veloppement\",\"Inizio del processo di sviluppo\",\"Begynnelse av utviklingsprosessen\"]" },
                    { 5008, "[\"Planera utvecklingsm\\u00F6te\",\"Plan development meeting\",\"Planl\\u00E6g udviklingsm\\u00F8de\",\"Planen Sie ein Entwicklungstreffen\",\"Plan reuni\\u00F3n de desarrollo\",\"Planifier une r\\u00E9union de d\\u00E9veloppement\",\"Pianifica una riunione di sviluppo\",\"Planlegg utviklingsm\\u00F8te\"]" },
                    { 5009, "[\"Projektet g\\u00E5r vidare till produktion\",\"Project moving to production\",\"Projektet g\\u00E5r videre til produktionen\",\"Projekt geht in die Produktion\",\"Proyecto en movimiento a producci\\u00F3n\",\"Projet en cours de production\",\"Progetto che si sposta in produzione\",\"Prosjekt beveger seg til produksjon\"]" },
                    { 5010, "[\"Skapa godk\\u00E4nnandebrev\",\"Create approved letter\",\"Opret godkendt brev\",\"Genehmigungsschreiben erstellen\",\"Crear carta de aprobaci\\u00F3n\",\"Cr\\u00E9er une lettre d\\u0027approbation\",\"Creare una lettera di approvazione\",\"Opprett godkjennelsesbrev\"]" },
                    { 5011, "[\"Informera producenten att projektet \\u00E4r klart f\\u00F6r gr\\u00F6nt ljus\",\"Tell producer project is ready for greenlite.\",\"Fort\\u00E6l producenten, at projektet er klar til gr\\u00F8nt lys.\",\"Sagen Sie dem Produzenten, dass das Projekt bereit f\\u00FCr gr\\u00FCnes Licht ist.\",\"Dile al productor que el proyecto est\\u00E1 listo para la luz verde.\",\"Dites au producteur que le projet est pr\\u00EAt pour le feu vert.\",\"Dite al produttore che il progetto \\u00E8 pronto per il via libera.\",\"Fortell produsenten at prosjektet er klart for gr\\u00F8nt lys.\"]" },
                    { 5012, "[\"Tilldela avtalsroll\",\"Assign agreement role.\",\"Tildel aftalerollen.\",\"Weisen Sie die Vertragsrolle zu.\",\"Asignar rol de acuerdo.\",\"Attribuer le r\\u00F4le d\\u0027accord.\",\"Assegna il ruolo di accordo.\",\"Tildele avtalerollen.\"]" },
                    { 5013, "[\"Godk\\u00E4nt brev skickat till producent\",\"Approved letter sent to producer\",\"Godkendt brev sendt til producenten\",\"Genehmigungsbrief an Produzenten gesendet\",\"Carta de aprobaci\\u00F3n enviada al productor\",\"Lettre d\\u0027approbation envoy\\u00E9e au producteur\",\"Lettera di approvazione inviata al produttore\",\"Godkjennelsesbrev sendt til produsenten\"]" },
                    { 5014, "[\"V\\u00E4ntar p\\u00E5 andra finansi\\u00E4rer\",\"Await other financiers\",\"Afventer andre finansfolk\",\"Warten auf andere Finanziers\",\"A la espera de otros financiadores\",\"En attente des autres financiers\",\"In attesa di altri finanziatori\",\"Venter p\\u00E5 andre finansfolk\"]" },
                    { 5015, "[\"Avtalsprocess p\\u00E5b\\u00F6rjad\",\"Agreement process started.\",\"Aftaleproces startet.\",\"Vertragsprozess gestartet.\",\"Proceso de acuerdo iniciado.\",\"Processus d\\u0027accord commenc\\u00E9.\",\"Processo di accordo avviato.\",\"Avtaleprosessen startet.\"]" },
                    { 5016, "[\"Avtalsprocess p\\u00E5b\\u00F6rjad, samla in ytterligare material.\",\"Agreement process started, collect additional material.\",\"Aftaleproces p\\u00E5begyndt, samle yderligere materiale.\",\"Vertragsprozess gestartet, zus\\u00E4tzliches Material sammeln.\",\"Proceso de acuerdo iniciado, recopilar material adicional.\",\"Processus d\\u0027accord commenc\\u00E9, recueillir du mat\\u00E9riel suppl\\u00E9mentaire.\",\"Processo di accordo iniziato, raccogliere materiale aggiuntivo.\",\"Avtaleprosessen startet, samle ytterligere materiale.\"]" },
                    { 5017, "[\"Kontrollera om avtalsprocessen \\u00E4r avslutad meddelande.\",\"Check if agreement process is finished message.\",\"Kontroller, om aftaleprocessen er f\\u00E6rdig.\",\"\\u00DCberpr\\u00FCfen Sie, ob der Vertragsprozess abgeschlossen ist.\",\"Verifique si el proceso de acuerdo est\\u00E1 terminado.\",\"V\\u00E9rifiez si le processus d\\u0027accord est termin\\u00E9.\",\"Controllare se il processo di accordo \\u00E8 terminato.\",\"Kontroller, om avtaleprosessen er ferdig.\"]" },
                    { 5018, "[\"Alla delar har skrivit under avtalet meddelande.\",\"All parts have signed the agreement message.\",\"Alle dele har underskrevet aftalen.\",\"Alle Teile haben die Vereinbarung unterzeichnet.\",\"Todas las partes han firmado el acuerdo.\",\"Toutes les parties ont sign\\u00E9 l\\u0027accord.\",\"Tutte le parti hanno firmato l\\u0027accordo.\",\"Alle delene har signert avtalen.\"]" },
                    { 5019, "[\"Registrera avtalsmeddelande.\",\"Register agreement message.\",\"Registrer aftale besked.\",\"Vereinbarungsnachricht registrieren.\",\"Registrar mensaje de acuerdo.\",\"Enregistrer le message d\\u0027accord.\",\"Registrare il messaggio di accordo.\",\"Registrer avtale beskjed.\"]" },
                    { 5020, "[\"Se upp f\\u00F6r klippmeddelande.\",\"Look out for cut message.\",\"Pas p\\u00E5 klip meddelelse.\",\"Achten Sie auf Schnittnachricht.\",\"Cuidado con el mensaje de corte.\",\"Attention au message de coupe.\",\"Attenzione al messaggio di taglio.\",\"Pass p\\u00E5 klipp beskjed.\"]" },
                    { 5021, "[\"Se upp f\\u00F6r PR-meddelande.\",\"Look out for PR message.\",\"Se efter PR-besked.\",\"Achten Sie auf PR-Nachricht.\",\"Cuidado con el mensaje de RP.\",\"Attention au message RP.\",\"Attenzione al messaggio di PR.\",\"Pass p\\u00E5 PR-melding.\"]" },
                    { 5022, "[\"Se upp f\\u00F6r klippavtalsroll meddelande.\",\"Look out for cut agreement role message.\",\"Pas p\\u00E5 klipaftalerolle.\",\"Achten Sie auf Schnittvereinbarungsrolle Besch.\",\"Cuidado con el mensaje de rol de acuerdo de corte.\",\"Attention au message de r\\u00F4le d\\u0027accord de coupe.\",\"Attenzione al messaggio di ruolo dell\\u0027accordo di taglio.\",\"Pass opp for klippeavtalerolle beskjede.\"]" },
                    { 5023, "[\"Meddela filmstart.\",\"Tell filming start message.\",\"Fort\\u00E6l filmbeskedens start.\",\"Sagen Sie dem filming Startmeldung.\",\"Decir mensaje de inicio de filmaci\\u00F3n.\",\"Dites message de d\\u00E9but de tournage.\",\"Dite messaggio di inizio delle riprese.\",\"Fortell filming start melding.\"]" },
                    { 5024, "[\"Fotograferingsstartdatum skickat meddelande.\",\"Photography startdate sent message.\",\"Fotograferingsstartdato sendt meddelelse.\",\"Fotografie startdatum gesendet Nachricht.\",\"Fecha de inicio de la fotograf\\u00EDa enviada mensaje.\",\"Date de d\\u00E9but de la photographie envoy\\u00E9 message.\",\"Data di inizio della fotografia inviata messaggio.\",\"Fotostartdato sendt melding.\"]" },
                    { 5025, "[\"Skicka uppdaterad teamlista meddelande.\",\"Sent updated team list message.\",\"Sendt opdateret holdliste besked.\",\"Gesendete aktualisierte Teamliste Nachricht.\",\"Mensaje de lista de equipo actualizado enviado.\",\"Message de liste d\\u0027\\u00E9quipe mis \\u00E0 jour envoy\\u00E9.\",\"Messaggio dell\\u0027elenco del team aggiornato inviato.\",\"Sendt oppdatert teamliste.\"]" },
                    { 5026, "[\"Fotograferingsstartdatum skickat l\\u00F6nemeddelande.\",\"Photography startdate sent pay rate message.\",\"Fotograferingsstartdato sendt l\\u00F8nbesked.\",\"Fotografie Startdatum gesendet Gehalt Nachricht.\",\"Fecha de inicio de la fotograf\\u00EDa enviada mensaje de tarifa de pago.\",\"Date de d\\u00E9but de la photographie envoy\\u00E9 taux de r\\u00E9mun\\u00E9ration message.\",\"Data di inizio della fotografia inviata messaggio della tariffa di pagamento.\",\"Fotostartdato sendt l\\u00F8nnsbeskjede.\"]" },
                    { 5027, "[\"Milsten 1 betalad meddelande.\",\"Milestone 1 payed message.\",\"Milep\\u00E6l 1 betalt besked.\",\"Meilenstein 1 bezahlt Nachricht.\",\"Mensaje de hito 1 pagado.\",\"Message de jalon 1 pay\\u00E9.\",\"Messaggio di pagamento della pietra miliare 1.\",\"Milep\\u00E6l 1 betalt melding.\"]" },
                    { 5028, "[\"Meddela filmavslutning.\",\"Tell filming end message.\",\"Fort\\u00E6l filmbeskedent slut.\",\"Sagen Sie dem Ende der Drehmeldung.\",\"Decir mensaje de finalizaci\\u00F3n de filmaci\\u00F3n.\",\"Dites message de fin de tournage.\",\"Dite messaggio di fine riprese.\",\"Fortell filming slutt melding.\"]" },
                    { 5029, "[\"Fotograferings slutdatum skickat meddelande.\",\"Photography enddate sent message.\",\"Fotograferings slutdato sendt besked.\",\"Fotografie Enddatum gesendet Nachricht.\",\"Fecha de finalizaci\\u00F3n de la fotograf\\u00EDa enviada mensaje.\",\"Date de fin de la photographie envoy\\u00E9 message.\",\"Data di fine della fotografia inviata messaggio.\",\"Foto sluttdato sendt melding.\"]" },
                    { 5030, "[\"Fotograferings slutdatum skickat l\\u00F6nemeddelande.\",\"Photography enddate sent pay rate message.\",\"Fotograferings slutdato sendt l\\u00F8nsbesked.\",\"Fotografie Enddatum gesendet Gehalt Nachricht.\",\"Fecha de finalizaci\\u00F3n de la fotograf\\u00EDa enviada mensaje de tarifa de pago.\",\"Date de fin de la photographie envoy\\u00E9 taux de r\\u00E9mun\\u00E9ration message.\",\"Data di fine della fotografia inviata messaggio della tariffa di pagamento.\",\"Foto sluttdato sendt l\\u00F8nnsbeskjede.\"]" },
                    { 5031, "[\"Milsten 2 betalad meddelande.\",\"Milestone 2 payed message.\",\"Milep\\u00E6l 2 betalt besked.\",\"Meilenstein 2 bezahlt Nachricht.\",\"Mensaje de hito 2 pagado.\",\"Message de jalon 2 pay\\u00E9.\",\"Messaggio di pagamento della pietra miliare 2.\",\"Milep\\u00E6l 2 betalt melding.\"]" },
                    { 5032, "[\"Skicka r\\u00E5klippsmeddelande.\",\"Send rough cut message.\",\"Send r\\u00E5klip besked.\",\"Schicken Sie Rough Cut Nachricht.\",\"Enviar mensaje de corte bruto.\",\"Envoyer un message de coupe grossi\\u00E8re.\",\"Invia messaggio di taglio grezzo.\",\"Sendt r\\u00F8ff klipp melding.\"]" },
                    { 5033, "[\"H\\u00E5ll koll p\\u00E5 r\\u00E5klippsmeddelande.\",\"Keep watch for rough cut message.\",\"Hold \\u00F8je med r\\u00E5klipsbesked.\",\"Beobachten Sie Rough Cut Nachricht.\",\"Vigile sobre el mensaje de corte en bruto.\",\"Surveiller le message de coupe grossi\\u00E8re.\",\"Tenere d\\u0027occhio il messaggio di taglio grezzo.\",\"Hold \\u00F8ye med r\\u00E5klippsmeldingen.\"]" },
                    { 5034, "[\"F\\u00F6lj upp r\\u00E5klippsmeddelande.\",\"Follow rough cut message.\",\"F\\u00F8lg r\\u00E5klipsbesked.\",\"Folgen Sie Rough Cut Nachricht.\",\"Siga el mensaje de corte en bruto.\",\"Suivez le message de coupe grossi\\u00E8re.\",\"Seguire il messaggio di taglio grezzo.\",\"F\\u00F8lg r\\u00E5klippsmeldingen.\"]" },
                    { 5035, "[\"DCP-kopiameddelandet skickades.\",\"DCP copy message sent message.\",\"DCP kopi besked sendt.\",\"DCP-Kopie Nachricht gesendet.\",\"Mensaje de copia de DCP enviado.\",\"Message de copie DCP envoy\\u00E9.\",\"Messaggio della copia DCP inviato.\",\"DCP kopi melding sendt.\"]" },
                    { 5036, "[\"Skicka DCP-kopiameddelande.\",\"Send DCP copy message.\",\"Send DCP kopi besked.\",\"DCP-Kopie Nachricht senden.\",\"Enviar mensaje de copia de DCP.\",\"Envoyer le message de copie DCP.\",\"Inviare un messaggio di copia DCP.\",\"Send DCP kopi melding.\"]" },
                    { 5037, "[\"Slutlig klippning godk\\u00E4nd meddelande.\",\"Final cut approved message.\",\"Endelig klipning godkendt besked.\",\"Letzter Schnitt Nachricht genehmigt.\",\"Mensaje de corte final aprobado.\",\"Message de coupe finale approuv\\u00E9.\",\"Messaggio di approvazione finale del taglio.\",\"Endelig klipp godkjent melding.\"]" },
                    { 5038, "[\"Skicka utgiftsrapport meddelande.\",\"Send spend report message.\",\"Send udgiftsrapport besked.\",\"Ausgabenbericht Nachricht senden.\",\"Enviar mensaje de informe de gastos.\",\"Envoyer un message de rapport de d\\u00E9penses.\",\"Inviare un messaggio del rapporto di spesa.\",\"Send utgiftsrapport melding.\"]" },
                    { 5039, "[\"Utgiftsrapport skickad meddelande.\",\"Spend report sent message.\",\"Udgiftsrapport sendt besked.\",\"Ausgabenbericht gesendet Nachricht.\",\"Informe de gastos enviado mensaje.\",\"Message de rapport de d\\u00E9penses envoy\\u00E9.\",\"Messaggio di spesa inviato rapporto.\",\"Utgiftsrapport sendt melding.\"]" },
                    { 5040, "[\"Skicka premi\\u00E4rdatummeddelande.\",\"Send premiere date message.\",\"Send premieredato besked.\",\"Premierenachricht senden.\",\"Enviar mensaje de fecha de estreno.\",\"Envoyer un message de date de premi\\u00E8re.\",\"Inviare il messaggio della data di prima visione.\",\"Send premieredato melding.\"]" },
                    { 5041, "[\"Registrera utgiftsrapport meddelande.\",\"Register spend report message.\",\"Registrer udgiftsrapport besked.\",\"Ausgabenbericht Nachricht registrieren.\",\"Registrar mensaje de informe de gastos.\",\"Enregistrer un message de rapport de d\\u00E9penses.\",\"Registrare un messaggio del rapporto di spesa.\",\"Registrer utgiftsrapport melding.\"]" },
                    { 5042, "[\"Premi\\u00E4rdatum skickat meddelande.\",\"Premiere date sent message.\",\"Premieredato sendt besked.\",\"Premierenachricht gesendet.\",\"Mensaje de fecha de estreno enviado.\",\"Message de date de premi\\u00E8re envoy\\u00E9.\",\"Messaggio di data della prima visione inviato.\",\"Premiere dato sendt melding.\"]" },
                    { 5043, "[\"Premi\\u00E4rdatum skickat PR-meddelande.\",\"Premiere date sent PR message.\",\"Premieredato sendt PR besked.\",\"Premiere PR-Nachricht gesendet.\",\"Mensaje de RP de fecha de estreno enviado.\",\"Message PR de date de premi\\u00E8re envoy\\u00E9.\",\"Messaggio PR della data di prima visione inviato.\",\"Premiere dato sendt PR melding.\"]" },
                    { 5044, "[\"Godk\\u00E4nda utgifter, krediter och DCP-kopia, l\\u00F6nemeddelande.\",\"Approved spend, credits and DCP copy, pay rate message.\",\"Godkendte udgifter, kreditter og DCP kopi, l\\u00F8nbesked.\",\"Genehmigte Ausgaben, Gutschriften und DCP-Kopie, Gehaltsnachricht.\",\"Gastos aprobados, cr\\u00E9ditos y copia de DCP, mensaje de tarifa de pago.\",\"D\\u00E9penses approuv\\u00E9es, cr\\u00E9dits et copie DCP, message de taux de r\\u00E9mun\\u00E9ration.\",\"Spese approvate, crediti e copia di DCP, messaggio di tariffa.\",\"Godkjente utgifter, kreditter og DCP kopi, l\\u00F8nnsbeskjede.\"]" },
                    { 5045, "[\"\\u00D6va PR och planeringsmeddelande.\",\"Go over PR and planning message.\",\"Praksis PR og planl\\u00E6gning besked.\",\"PR und Planungsnachricht durchgehen.\",\"Practicar mensaje de planificaci\\u00F3n y relaciones p\\u00FAblicas.\",\"Pratiquer le message de planification et de RP.\",\"Praticare il messaggio di pianificazione e PR.\",\"\\u00D8ve p\\u00E5 PR og planleggingsmelding.\"]" },
                    { 5046, "[\"Projektet redo f\\u00F6r slutrapporten meddelande.\",\"Project ready for final report message.\",\"Projekt klar til slutrapport besked.\",\"Projekt bereit f\\u00FCr den endg\\u00FCltigen Bericht.\",\"Proyecto listo para el informe final.\",\"Projet pr\\u00EAt pour le rapport final.\",\"Progetto pronto per il rapporto finale.\",\"Prosjekt klar for sluttrapporteringsmelding.\"]" },
                    { 5047, "[\"Be om DVD-meddelande.\",\"Ask for DVD message.\",\"Bed om DVD besked.\",\"DVD-Nachricht anfordern.\",\"Pedir mensaje de DVD.\",\"Demander un message de DVD.\",\"Chiedere un messaggio del DVD.\",\"Be om DVD-melding.\"]" },
                    { 5048, "[\"Registrera DVD-meddelande.\",\"Register DVD message.\",\"Registrer DVD besked.\",\"DVD-Nachricht registrieren.\",\"Registrar mensaje de DVD.\",\"Enregistrer un message de DVD.\",\"Registrare un messaggio del DVD.\",\"Registrer DVD-melding.\"]" },
                    { 5049, "[\"Registrera slutrapporten meddelande.\",\"Register end report message.\",\"Registrer slutrapport besked.\",\"Endbericht Nachricht registrieren.\",\"Registrar mensaje de informe final.\",\"Enregistrer un message de rapport de fin.\",\"Registrare un messaggio del rapporto finale.\",\"Registrer sluttrapporteringsmelding.\"]" },
                    { 5050, "[\"Nytt ans\\u00F6kningsmeddelande.\",\"New application message.\",\"Ny ans\\u00F8gning besked.\",\"Neue Bewerbungsnachricht.\",\"Nuevo mensaje de solicitud.\",\"Nouveau message de candidature.\",\"Nuovo messaggio di richiesta.\",\"Ny s\\u00F8knads melding.\"]" },
                    { 5051, "[\"Lokalisering inte giltig l\\u00E4ngre meddelande.\",\"Loc not valid anymore message.\",\"Lok ikke l\\u00E6ngere gyldig besked.\",\"Ort nicht mehr g\\u00FCltig Nachricht.\",\"Ubicaci\\u00F3n ya no v\\u00E1lida mensaje.\",\"Emplacement non valide message.\",\"Posizione non pi\\u00F9 valida messaggio.\",\"Plass ikke lenger gyldig melding.\"]" },
                    { 5052, "[\"Kontrollera om allt material \\u00E4r mottaget meddelande.\",\"Check if all material is received message.\",\"Kontroller, om alt materiale er modtaget.\",\"\\u00DCberpr\\u00FCfen Sie, ob alle Materialien erhalten wurden.\",\"Verifique si se ha recibido todo el material.\",\"V\\u00E9rifiez si tout le mat\\u00E9riel a \\u00E9t\\u00E9 re\\u00E7u.\",\"Verificare se tutto il materiale \\u00E8 stato ricevuto.\",\"Kontroller om alt materiale er mottatt melding.\"]" },
                    { 5053, "[\"V\\u00E4ntar p\\u00E5 producent meddelande\",\"Waiting for producer message.\",\"Afventer producent besked.\",\"Auf Produzent Nachricht warten.\",\"Esperando mensaje del productor.\",\"En attente de message du producteur.\",\"In attesa di un messaggio dal produttore.\",\"Venter p\\u00E5 produsent melding.\"]" },
                    { 5054, "[\"Beh\\u00F6ver Public 360 id meddelande.\",\"Need Public 360 id message.\",\"Brug for Public 360 id besked.\",\"Ben\\u00F6tigen Public 360 ID Nachricht.\",\"Necesita mensaje de id p\\u00FAblico 360.\",\"Besoin de message d\\u0027identification publique 360.\",\"Necessario messaggio di ID pubblico 360.\",\"Trenger Public 360 ID melding.\"]" },
                    { 5055, "[\"Arkivera applikationsmeddelande.\",\"Archive app message.\",\"Arkiv app besked.\",\"Anwendungsnachricht archivieren.\",\"Mensaje de aplicaci\\u00F3n de archivo.\",\"Archiver le message d\\u0027application.\",\"Messaggio dell\\u0027applicazione di archivio.\",\"Arkivere app melding.\"]" },
                    { 5056, "[\"Slutrapport godk\\u00E4nd meddelande.\",\"Final report approved message.\",\"Slutrapport godkendt besked.\",\"Endbericht genehmigt Nachricht.\",\"Informe final aprobado mensaje.\",\"Rapport final approuv\\u00E9 message.\",\"Rapporto finale approvato messaggio.\",\"Sluttrapport godkjent melding.\"]" },
                    { 5057, "[\"Fj\\u00E4rde ratten betald meddelande.\",\"Fourth rat payed message.\",\"Fjerde rate betalt besked.\",\"Vierte Rate bezahlt Nachricht.\",\"Cuarta rata pagada mensaje.\",\"Quatri\\u00E8me mensualit\\u00E9 pay\\u00E9e message.\",\"Quarta rata pagata messaggio.\",\"Fjerde avdrag betalt melding.\"]" },
                    { 5058, "[\"Projektet avslutat meddelande.\",\"Project finished message.\",\"Projekt f\\u00E6rdig besked.\",\"Projekt abgeschlossen Nachricht.\",\"Proyecto terminado mensaje.\",\"Projet termin\\u00E9 message.\",\"Progetto finito messaggio.\",\"Prosjekt ferdig melding.\"]" },
                    { 5059, "[\"Slutf\\u00F6r Public 360.\",\"Complete Public 360.\",\"Afslut Public 360.\",\"Vervollst\\u00E4ndigen Sie Public 360.\",\"Completar Public 360.\",\"Compl\\u00E9ter Public 360.\",\"Completare Public 360.\",\"Fullf\\u00F8r Public 360.\"]" },
                    { 5060, "[\"Skicka faktura meddelande.\",\"Send invoice message.\",\"Send faktura besked.\",\"Rechnung Nachricht senden.\",\"Enviar mensaje de factura.\",\"Envoyer un message de facture.\",\"Inviare un messaggio di fattura.\",\"Send faktura melding.\"]" },
                    { 5061, "[\"V\\u00E4ntar p\\u00E5 utgiftsrapport meddelande.\",\"Await spend report message.\",\"Afventer udgiftsrapport besked.\",\"Ausgabenbericht Nachricht erwarten.\",\"Esperar mensaje de informe de gastos.\",\"En attente de message de rapport de d\\u00E9penses.\",\"In attesa del messaggio del rapporto di spesa.\",\"Venter p\\u00E5 utgiftsrapport melding.\"]" },
                    { 5062, "[\"V\\u00E4ntar p\\u00E5 premi\\u00E4rdatum meddelande.\",\"Await premier dates message.\",\"Afventer premierdatoer besked.\",\"Premiere Daten Nachricht erwarten.\",\"Esperar mensaje de fechas de estreno.\",\"En attente de message de dates de premi\\u00E8re.\",\"In attesa del messaggio delle date di prima visione.\",\"Venter p\\u00E5 premierdatoer melding.\"]" },
                    { 5063, "[\"Kontrollera slutrapport meddelande.\",\"Check end report message.\",\"Kontrol slutrapport besked.\",\"Endbericht Nachricht \\u00FCberpr\\u00FCfen.\",\"Verificar mensaje de informe final.\",\"V\\u00E9rifier le message de rapport de fin.\",\"Verificare il messaggio del rapporto finale.\",\"Kontroller sluttrapport melding.\"]" },
                    { 5064, "[\"Namn \\u00E4ndrat meddelande.\",\"Name changed message.\",\"Navn \\u00E6ndret besked.\",\"Name ge\\u00E4ndert Nachricht.\",\"Nombre cambiado mensaje.\",\"Nom chang\\u00E9 message.\",\"Nome cambiato messaggio.\",\"Navn endret melding.\"]" },
                    { 5065, "[\"Boka visionsm\\u00F6te meddelande.\",\"Book vision meeting message.\",\"Book vision m\\u00F8de besked.\",\"Visionsmeeting Nachricht buchen.\",\"Reservar mensaje de reuni\\u00F3n de visi\\u00F3n.\",\"R\\u00E9server un message de r\\u00E9union de vision.\",\"Prenotare il messaggio della riunione di visione.\",\"Bestill visjonsm\\u00F8te melding.\"]" },
                    { 5066, "[\"Projekt delegerat meddelande.\",\"Project delegated message.\",\"Projekt delegeret besked.\",\"Projekt delegiert Nachricht.\",\"Proyecto delegado mensaje.\",\"Projet d\\u00E9l\\u00E9gu\\u00E9 message.\",\"Progetto delegato messaggio.\",\"Prosjekt delegert melding.\"]" },
                    { 5067, "[\"Skriv beslutsdokument meddelande.\",\"Write decision doc message.\",\"Skriv beslutningsdokument besked.\",\"Entscheidung Dokument Nachricht schreiben.\",\"Escribir mensaje de documento de decisi\\u00F3n.\",\"\\u00C9crire un message de document de d\\u00E9cision.\",\"Scrivere un messaggio di documento di decisione.\",\"Skriv vedtaksdokument melding.\"]" },
                    { 5068, "[\"Arkivera beslutsdokument meddelande.\",\"Archive decision doc message.\",\"Arkiver beslutningsdokument besked.\",\"Entscheidung dokument Nachricht archivieren.\",\"Archivar mensaje de documento de decisi\\u00F3n.\",\"Archiver un message de document de d\\u00E9cision.\",\"Archiviare un messaggio di documento di decisione.\",\"Arkiver vedtaksdokument melding.\"]" },
                    { 5069, "[\"Visionsm\\u00F6te bokat meddelande.\",\"Vision meeting booked message.\",\"Vision m\\u00F8de booket besked.\",\"Vision Meeting gebucht Nachricht.\",\"Mensaje de reuni\\u00F3n de visi\\u00F3n reservado.\",\"Message de r\\u00E9union de vision r\\u00E9serv\\u00E9.\",\"Messaggio di riunione di visione prenotato.\",\"Visjonsm\\u00F8te booket melding.\"]" },
                    { 5070, "[\"DCP godk\\u00E4nt meddelande.\",\"DCP approved message.\",\"DCP godkendt besked.\",\"DCP genehmigt Nachricht.\",\"Mensaje aprobado por DCP.\",\"Message approuv\\u00E9 par le DCP.\",\"Messaggio approvato da DCP.\",\"DCP godkjent melding.\"]" },
                    { 5071, "[\"Betala r\\u00E5tta meddelande.\",\"Pay rat message.\",\"Betale rotte besked.\",\"Ratte Nachricht bezahlen.\",\"Mensaje de pagar rata.\",\"Message de payer le rat.\",\"Messaggio di pagamento ratto.\",\"Betale rotte melding.\"]" },
                    { 5072, "[\"Detalj PR-material meddelande.\",\"Detail PR material message.\",\"Detaljer PR materiale besked.\",\"Detail PR Material Nachricht.\",\"Mensaje de material de RP detallado.\",\"Message de mat\\u00E9riel de RP en d\\u00E9tail.\",\"Messaggio di materiale PR dettagliato.\",\"Detalj PR-material melding.\"]" },
                    { 5073, "[\"PR-material levererat meddelande.\",\"PR material delivered message.\",\"PR materiale leveret besked.\",\"PR Material geliefert Nachricht.\",\"Mensaje de material de RP entregado.\",\"Message de mat\\u00E9riel de RP livr\\u00E9.\",\"Messaggio di materiale PR consegnato.\",\"PR-material levert melding.\"]" },
                    { 5074, "[\"Greenlight spam meddelande.\",\"Greenlight spam message.\",\"Greenlight spam besked.\",\"Greenlight Spam Nachricht.\",\"Mensaje de spam de Greenlight.\",\"Message de spam Greenlight.\",\"Messaggio di spam di Greenlight.\",\"Greenlight spam melding.\"]" },
                    { 5075, "[\"Skriv under beslut.\",\"Sign decision.\",\"Underskriv beslutning.\",\"Entscheidung unterzeichnen.\",\"Firmar decisi\\u00F3n.\",\"Signer la d\\u00E9cision.\",\"Firmare la decisione.\",\"Signere beslutning.\"]" },
                    { 5076, "[\"Arkivera beslut.\",\"Archive decision.\",\"Arkiver beslutning.\",\"Entscheidung archivieren.\",\"Archivar decisi\\u00F3n.\",\"Archiver la d\\u00E9cision.\",\"Archiviare la decisione.\",\"Arkivere beslutning.\"]" },
                    { 5077, "[\"Avtal undertecknat l\\u00F6nebesked.\",\"Agreement signed pay rate message.\",\"Aftale underskrevet l\\u00F8nbesked.\",\"Vereinbarung unterzeichnet Lohnnachricht.\",\"Mensaje de tarifa de pago firmado.\",\"Message de taux de r\\u00E9mun\\u00E9ration sign\\u00E9.\",\"Messaggio di tariffa di pagamento firmato.\",\"Avtale signert l\\u00F8nnsbeskjed.\"]" },
                    { 5078, "[\"Skicka arbetskopia meddelande.\",\"Send work copy message.\",\"Send arbejds kopi besked.\",\"Arbeitskopie Nachricht senden.\",\"Enviar mensaje de copia de trabajo.\",\"Envoyer un message de copie de travail.\",\"Inviare un messaggio di copia di lavoro.\",\"Send arbeids kopi melding.\"]" },
                    { 5079, "[\"Arkivera utgifts- och slutrapport meddelande.\",\"Archive spend and final report message.\",\"Arkiver brug og slutrapport besked.\",\"Ausgaben- und Abschlussbericht Nachricht archivieren.\",\"Archivar mensaje de gastos e informe final.\",\"Archiver un message de d\\u00E9penses et rapport final.\",\"Archiviare un messaggio di spese e rapporto finale.\",\"Arkiver forbruk og sluttrapporteringsmelding.\"]" },
                    { 5080, "[\"Utgifter och slutrapport godk\\u00E4nd l\\u00F6nebesked.\",\"Spend and final report approved pay rate message.\",\"Brug og slutrapport godkendt l\\u00F8nbesked.\",\"Ausgaben- und Abschlussbericht genehmigt Lohnnachricht.\",\"Gastos e informe final aprobado mensaje de tarifa de pago.\",\"D\\u00E9penses et rapport final approuv\\u00E9 message de taux de r\\u00E9mun\\u00E9ration.\",\"Spese e rapporto finale approvato messaggio di tariffa di pagamento.\",\"Forbruk og sluttrapport godkjent l\\u00F8nnsbeskjed.\"]" },
                    { 5081, "[\"Tredje ratten betald meddelande.\",\"Third rat payed message.\",\"Tredje rate betalt besked.\",\"Dritte Rate bezahlt Nachricht.\",\"Mensaje de tercer pago de rata.\",\"Message de troisi\\u00E8me mensualit\\u00E9 pay\\u00E9e.\",\"Messaggio di terza rata pagata.\",\"Tredje avdrag betalt melding.\"]" },
                    { 5082, "[\"Skicka premi\\u00E4rdatummeddelande - kortfilm.\",\"Send premiere date message - short film.\",\"Send premieredato besked - kortfilm.\",\"Premiere Datum Nachricht senden - Kurzfilm.\",\"Enviar mensaje de fecha de estreno - cortometraje.\",\"Envoyer un message de date de premi\\u00E8re - court m\\u00E9trage.\",\"Inviare il messaggio di data di prima visione - cortometraggio.\",\"Send premierdato melding - kortfilm.\"]" },
                    { 5083, "[\"V\\u00E4ntar p\\u00E5 premi\\u00E4rdatum meddelande - kortfilm.\",\"Await premier dates message - short film.\",\"Afventer premieredatoer besked - kortfilm.\",\"Premiere Daten Nachricht erwarten - Kurzfilm.\",\"Esperar mensaje de fechas de estreno - cortometraje.\",\"En attente de message de dates de premi\\u00E8re - court m\\u00E9trage.\",\"In attesa del messaggio delle date di prima visione - cortometraggio.\",\"Venter p\\u00E5 premierdatoer melding - kortfilm.\"]" },
                    { 5084, "[\"Projekt avslutat utan produktion meddelande.\",\"Project closed without production message.\",\"Projekt afsluttet uden produktion besked.\",\"Projekt ohne Produktion abgeschlossen Nachricht.\",\"Proyecto cerrado sin producci\\u00F3n mensaje.\",\"Projet ferm\\u00E9 sans production message.\",\"Progetto chiuso senza produzione messaggio.\",\"Prosjekt avsluttet uten produksjon melding.\"]" },
                    { 5085, "[\"Ladda upp filminformation meddelande.\",\"Upload film info message.\",\"Upload film information besked.\",\"Filminfo Nachricht hochladen.\",\"Subir mensaje de informaci\\u00F3n de pel\\u00EDcula.\",\"T\\u00E9l\\u00E9charger le message d\\u0027informations sur le film.\",\"Caricare il messaggio di informazioni sul film.\",\"Last opp filminformasjon melding.\"]" },
                    { 5086, "[\"Ladda upp film meddelande.\",\"Upload film message.\",\"Upload film besked.\",\"Film Nachricht hochladen.\",\"Subir mensaje de pel\\u00EDcula.\",\"T\\u00E9l\\u00E9charger le message du film.\",\"Caricare il messaggio del film.\",\"Last opp film melding.\"]" },
                    { 5087, "[\"Skicka ekonomisk och konstn\\u00E4rlig redovisning meddelande.\",\"Send economical and artistic accounting message.\",\"Send \\u00F8konomisk og kunstnerisk regnskab besked.\",\"Wirtschaftliche und k\\u00FCnstlerische Buchhaltung Nachricht senden.\",\"Enviar mensaje de contabilidad econ\\u00F3mica y art\\u00EDstica.\",\"Envoyer un message de comptabilit\\u00E9 \\u00E9conomique et artistique.\",\"Inviare un messaggio di contabilit\\u00E0 economica e artistica.\",\"Send \\u00F8konomisk og kunstnerisk regnskapsmelding.\"]" },
                    { 5088, "[\"Ekonomisk och konstn\\u00E4rlig redovisningsdatum passerat producent meddelande.\",\"Economical and artistic accounting date passed producer message.\",\"\\u00D8konomisk og kunstnerisk regnskab dato passeret producent besked.\",\"Wirtschaftliche und k\\u00FCnstlerische Buchhaltung Datum verpasst Produzent Nachricht.\",\"Fecha de contabilidad econ\\u00F3mica y art\\u00EDstica pasada mensaje del productor.\",\"Date de comptabilit\\u00E9 \\u00E9conomique et artistique d\\u00E9pass\\u00E9e message du producteur.\",\"Data di contabilit\\u00E0 economica e artistica passata messaggio del produttore.\",\"\\u00D8konomisk og kunstnerisk regnskapsdato passert produsent melding.\"]" },
                    { 5089, "[\"Ekonomisk och konstn\\u00E4rlig redovisningsdatum passerat AA meddelande.\",\"Economical and artistic accounting date passed AA message.\",\"\\u00D8konomisk og kunstnerisk regnskab dato passeret AA besked.\",\"Wirtschaftliche und k\\u00FCnstlerische Buchhaltung Datum verpasst AA Nachricht.\",\"Fecha de contabilidad econ\\u00F3mica y art\\u00EDstica pasada mensaje de AA.\",\"Date de comptabilit\\u00E9 \\u00E9conomique et artistique d\\u00E9pass\\u00E9e message AA.\",\"Data di contabilit\\u00E0 economica e artistica passata messaggio di AA.\",\"\\u00D8konomisk og kunstnerisk regnskapsdato passert AA melding.\"]" },
                    { 5090, "[\"Ekonomisk och konstn\\u00E4rlig redovisningsdatum passerat koordinator meddelande.\",\"Economical and artistic accounting date passed coordinator message.\",\"\\u00D8konomisk og kunstnerisk regnskab dato passeret koordinator besked.\",\"Wirtschaftliche und k\\u00FCnstlerische Buchhaltung Datum verpasst Koordinator Nachricht.\",\"Fecha de contabilidad econ\\u00F3mica y art\\u00EDstica pasada mensaje del coordinador.\",\"Date de comptabilit\\u00E9 \\u00E9conomique et artistique d\\u00E9pass\\u00E9e message du coordinateur.\",\"Data di contabilit\\u00E0 economica e artistica passata messaggio del coordinatore.\",\"\\u00D8konomisk og kunstnerisk regnskapsdato passert koordinator melding.\"]" },
                    { 9090, "[\"Motkrav godk\\u00E4nt.\",\"Counterclaim approved.\",\"Modkrav godkendt.\",\"Gegenforderung genehmigt.\",\"Reconocimiento aprobado.\",\"R\\u00E9clamation approuv\\u00E9e.\",\"Reclamo approvato.\",\"Mot krav godkjent.\"]" }
                });

            migrationBuilder.InsertData(
                table: "MilestoneRequirementTypes",
                columns: new[] { "Id", "Names" },
                values: new object[,]
                {
                    { 1, "[\"Inspelningsstart rapporterad till FiV\",\"Start of Filming Reported to FiV\",\"Indspilningsstart rapporteret til FiV\",\"Drehstart an FiV gemeldet\",\"Inicio de filmaci\\u00F3n reportado a FiV\",\"D\\u00E9but du tournage signal\\u00E9 \\u00E0 FiV\",\"Inizio delle riprese segnalato a FiV\",\"Start av innspilling rapportert til FiV\"]" },
                    { 2, "[\"FiV har mottagit samtliga bilagor och samproduktionsavtal i original f\\u00F6r projektet\",\"FiV has received all attachments and co-production agreement in original for the project\",\"FiV har modtaget alle bilag og samproduktionsaftale i original for projektet\",\"FiV hat alle Anlagen und Koproduktionsvereinbarung im Original f\\u00FCr das Projekt erhalten\",\"FiV ha recibido todos los anexos y el acuerdo de coproducci\\u00F3n original para el proyecto\",\"FiV a re\\u00E7u tous les pi\\u00E8ces jointes et l\\u0027accord de coproduction original pour le projet\",\"FiV ha ricevuto tutti gli allegati e l\\u0027accordo di coproduzione originale per il progetto\",\"FiV har mottatt alle vedlegg og samproduksjonsavtale i original for prosjektet\"]" },
                    { 3, "[\"Samproduktionsavtal i original mottaget av FiV\",\"Co-production Agreement in Original Received by FiV\",\"Samproduktionsaftale i original modtaget af FiV\",\"Koproduktionsvereinbarung im Original von FiV erhalten\",\"Acuerdo de coproducci\\u00F3n original recibido por FiV\",\"Accord de coproduction original re\\u00E7u par FiV\",\"Accordo di coproduzione originale ricevuto da FiV\",\"Samproduksjonsavtale i original mottatt av FiV\"]" },
                    { 4, "[\"Inspelningsslut rapporterad till FiV\",\"End of Filming Reported to FiV\",\"Indspilningsslut rapporteret til FiV\",\"Drehende an FiV gemeldet\",\"Finalizaci\\u00F3n de la filmaci\\u00F3n reportada a FiV\",\"Fin du tournage signal\\u00E9e \\u00E0 FiV\",\"Fine delle riprese segnalato a FiV\",\"Slutt p\\u00E5 innspilling rapportert til FiV\"]" },
                    { 5, "[\"Godk\\u00E4nd DCP/A-kopia mottagen av FiV\",\"Approved DCP/A-Copy Received by FiV\",\"Godkendt DCP/A-kopi modtaget af FiV\",\"Genehmigte DCP/A-Kopie von FiV erhalten\",\"DCP/A-copia aprobada recibida por FiV\",\"DCP/A-copie approuv\\u00E9e re\\u00E7ue par FiV\",\"DCP/A-copia approvata ricevuta da FiV\",\"Godkjent DCP/A-kopi mottatt av FiV\"]" },
                    { 6, "[\"Av revisor intygad och godk\\u00E4nd redovisning av spend mottagen av FiV\",\"Auditor Certified and Approved Spend Report Received by FiV\",\"Revisor bekr\\u00E6ftet og godkendt udgiftsrapport modtaget af FiV\",\"Vom Wirtschaftspr\\u00FCfer best\\u00E4tigter und genehmigter Ausgabenbericht von FiV erhalten\",\"Informe de gastos certificado y aprobado por auditor recibido por FiV\",\"Rapport de d\\u00E9penses certifi\\u00E9 et approuv\\u00E9 par l\\u0027auditeur re\\u00E7u par FiV\",\"Rapporto di spesa certificato e approvato dall\\u0027auditor ricevuto da FiV\",\"Revisorverifisert og godkjent utgiftsrapport mottatt av FiV\"]" },
                    { 7, "[\"Reviderad ekonomisk slutredovisning mottagen av FiV\",\"Revised Financial Final Report Received by FiV\",\"Revideret \\u00F8konomisk slutrapport modtaget af FiV\",\"\\u00DCberarbeiteter Finanzabschlussbericht von FiV erhalten\",\"Informe financiero final revisado recibido por FiV\",\"Rapport financier final r\\u00E9vis\\u00E9 re\\u00E7u par FiV\",\"Rapporto finanziario finale rivisto ricevuto da FiV\",\"Revidert \\u00F8konomisk sluttrapport mottatt av FiV\"]" },
                    { 8, "[\"Annat\",\"Other\",\"Andet\",\"Andere\",\"Otro\",\"Autres\",\"Altro\",\"Andre\"]" },
                    { 9, "[\"Faktura\",\"Invoice\",\"Faktura\",\"Rechnung\",\"Factura\",\"Facture\",\"Fattura\",\"Faktura\"]" },
                    { 14, "[\"Prelimin\\u00E4r klipp av filmen (DVD)\",\"Preliminary Cut of the Film (DVD)\",\"Forel\\u00F8big klip af filmen (DVD)\",\"Vorl\\u00E4ufiger Filmschnitt (DVD)\",\"Corte preliminar de la pel\\u00EDcula (DVD)\",\"Montage pr\\u00E9liminaire du film (DVD)\",\"Montaggio preliminare del film (DVD)\",\"Forel\\u00F8pig klipp av filmen (DVD)\"]" },
                    { 17, "[\"Reviderad r\\u00E4kenskap tillsammans med budget\",\"Revised Accounts with Budget\",\"Revideret regnskab sammen med budget\",\"\\u00DCberarbeitete Buchf\\u00FChrung mit Budget\",\"Cuentas revisadas con presupuesto\",\"Comptes r\\u00E9vis\\u00E9s avec budget\",\"Conti rivisti con budget\",\"Revidert regnskap sammen med budsjett\"]" },
                    { 18, "[\"Signerat kontrakt\",\"Signed Contract\",\"Signeret kontrakt\",\"Unterzeichneter Vertrag\",\"Contrato firmado\",\"Contrat sign\\u00E9\",\"Contratto firmato\",\"Signert kontrakt\"]" },
                    { 20, "[\"Skriven rapport inkluderat internationell f\\u00F6rs\\u00E4ljning, festivaldeltagande samt vilka priser som mottagits\",\"Written Report Including International Sales, Festival Participation and Received Awards\",\"Skriftlig rapport inklusive internationalt salg, festivaldeltagelse og modtagne priser\",\"Geschriebener Bericht einschlie\\u00DFlich internationaler Verk\\u00E4ufe, Festivalteilnahme und erhaltene Auszeichnungen\",\"Informe escrito que incluye ventas internacionales, participaci\\u00F3n en festivales y premios recibidos\",\"Rapport \\u00E9crit incluant les ventes internationales, la participation aux festivals et les prix re\\u00E7us\",\"Relazione scritta incluse vendite internazionali, partecipazione ai festival e premi ricevuti\",\"Skriftlig rapport inkludert internasjonalt salg, festivaldeltakelse og mottatte priser\"]" },
                    { 34, "[\"Vid mottaget undertecknat avtal samt bilagor\",\"Upon Receipt of Signed Agreement and Attachments\",\"Ved modtagelse af underskrevet aftale og bilag\",\"Bei Erhalt eines unterzeichneten Vertrags und Anh\\u00E4ngen\",\"Al recibir el acuerdo firmado y los anexos\",\"Lors de la r\\u00E9ception de l\\u0027accord sign\\u00E9 et des pi\\u00E8ces jointes\",\"Al ricevimento del contratto firmato e degli allegati\",\"Ved mottak av signert avtale og vedlegg\"]" },
                    { 35, "[\"Vid leverans av godk\\u00E4nd arbetskopia\",\"Upon Delivery of Approved Work Copy\",\"Ved levering af godkendt arbejdskopi\",\"Bei Lieferung einer genehmigten Arbeitskopie\",\"Al entregar una copia de trabajo aprobada\",\"Lors de la livraison d\\u0027une copie de travail approuv\\u00E9e\",\"Alla consegna della copia di lavoro approvata\",\"Ved levering av godkjent arbeidskopi\"]" },
                    { 36, "[\"Vid leverans av Filmen samt av FiV godk\\u00E4nd spendredovisning samt godk\\u00E4nd slutredovisning\",\"Upon Delivery of the Film and FiV-approved Spend Report and Final Report\",\"Ved levering af filmen og FiV-godkendt udgifts- og slutrapport\",\"Bei Lieferung des Films und von FiV genehmigten Ausgaben- und Abschlussberichten\",\"Al entregar la pel\\u00EDcula y el informe de gastos y el informe final aprobados por FiV\",\"Lors de la livraison du film et des rapports de d\\u00E9penses et final approuv\\u00E9s par FiV\",\"Alla consegna del film e del rapporto di spesa e del rapporto finale approvati da FiV\",\"Ved levering av filmen og FiV-godkjent utgifts- og sluttrapport\"]" },
                    { 37, "[\"Avtalsprocessen klar\",\"The Agreement Process is Completed\",\"Aftaleprocessen er afsluttet\",\"Der Vertragsprozess ist abgeschlossen\",\"El proceso de acuerdo est\\u00E1 completado\",\"Le processus d\\u0027accord est termin\\u00E9\",\"Il processo di accordo \\u00E8 completato\",\"Avtaleprosessen er fullf\\u00F8rt\"]" },
                    { 38, "[\"F\\u00E4rdig projektutveckling\",\"Completed Project Development\",\"Afsluttet projektudvikling\",\"Abgeschlossene Projektentwicklung\",\"Desarrollo de proyecto completado\",\"D\\u00E9veloppement de projet termin\\u00E9\",\"Sviluppo del progetto completato\",\"Avsluttet prosjektutvikling\"]" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropTable(
                name: "ApplicationBudgetTypes");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "ApplicationStates");

            migrationBuilder.DropTable(
                name: "AuditEntries");

            migrationBuilder.DropTable(
                name: "ClaimTypes");

            migrationBuilder.DropTable(
                name: "Controls");

            migrationBuilder.DropTable(
                name: "ControlTypes");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "DocumentDeliveryTypes");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "GridLayouts");

            migrationBuilder.DropTable(
                name: "MessageQueue");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "MessageTypes");

            migrationBuilder.DropTable(
                name: "MilestoneRequirementTypes");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropTable(
                name: "OpenAiCacheItems");

            migrationBuilder.DropTable(
                name: "OpenAiProjects");

            migrationBuilder.DropTable(
                name: "OpenAiUsers");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "PhoneNumberTypes");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ReactionTypes");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "Schemas");

            migrationBuilder.DropTable(
                name: "SearchIndex");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "SystemMessageDestinations");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
