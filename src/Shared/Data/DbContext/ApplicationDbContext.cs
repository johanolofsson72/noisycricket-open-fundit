using System;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.Protocol;
using OpenNLP.Tools.Util;
using Shared.Applications.Entities;
using Shared.AuditEntries.Entities;
using Shared.Controls.Entities;
using Shared.Documents.Entities;
using Shared.Global.Entities;
using Shared.GridLayouts.Enities;
using Shared.MessageQueue.Entities;
using Shared.Messages.Entities;
using Shared.Milestones.Entities;
using Shared.OpenAi.Entities;
using Shared.Organizations.Entities;
using Shared.Projects.Entities;
using Shared.Schemas.Entities;
using Shared.State.Entities;
using Shared.Statistics.Entities;
using Shared.Translations.Entities;
using Shared.Users.Entities;
using Shared.Users.Enums;

namespace Shared.Data.DbContext;


public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GridLayout> GridLayouts { get; set; }
    public DbSet<MessageQueueItem> MessageQueue { get; set; }
    public DbSet<Translation> Translations { get; set; }

    public DbSet<AuditEntry> AuditEntries { get; set; }
    public DbSet<OpenAiProject> OpenAiProjects { get; set; }
    public DbSet<OpenAiCache> OpenAiCacheItems { get; set; }
    public DbSet<OpenAiUser> OpenAiUsers { get; set; }
    public DbSet<Organization> Organizations { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Application> Applications { get; set; } = null!;
    public DbSet<ApplicationState> ApplicationStates { get; set; } = null!;
    public DbSet<Milestone> Milestones { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Control> Controls { get; set; } = null!;
    public DbSet<Schema> Schemas { get; set; } = null!;
    
    public DbSet<PhoneNumberType> PhoneNumberTypes { get; set; } = null!;
    public DbSet<ReactionType> ReactionTypes { get; set; } = null!;
    public DbSet<Status> Statuses { get; set; } = null!;
    public DbSet<SystemMessageDestination> SystemMessageDestinations { get; set; } = null!;
    public DbSet<MessageType> MessageTypes { get; set; } = null!;
    public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
    public DbSet<DocumentDeliveryType> DocumentDeliveryTypes { get; set; } = null!;
    public DbSet<ApplicationBudgetType> ApplicationBudgetTypes { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<Gender> Genders { get; set; } = null!;
    public DbSet<ControlType> ControlTypes { get; set; } = null!;
    public DbSet<EventType> EventTypes { get; set; } = null!;
    public DbSet<ClaimType> ClaimTypes { get; set; } = null!;
    public DbSet<ActionType> ActionTypes { get; set; } = null!;
    public DbSet<SearchIndex> SearchIndex { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<Statistic> Statistics { get; set; } = null!;
    public DbSet<MilestoneRequirementType> MilestoneRequirementTypes { get; set; } = null!;
    
    // 🔥 Skapa en statisk `JsonSerializerOptions` för att undvika problemet
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        try
        {
            base.OnModelCreating(builder);
            
            builder.HasAnnotation("SqlServer:DatabaseCollation", "Finnish_Swedish_CI_AS");

            builder.Entity<Schema>().OwnsMany(x => x.Events, y =>
            {
                y.OwnsMany(z => z.Actions, p => { p.ToJson(); });
                y.ToJson();
            });
            builder.Entity<Schema>().OwnsMany(x => x.Controls, y => { y.ToJson(); });
            builder.Entity<Schema>().OwnsMany(x => x.RequiredDocuments, y => { y.ToJson(); });
            builder.Entity<Schema>().OwnsMany(x => x.Progress, y =>
            {
                y.OwnsMany(z => z.Requirements, p => { p.ToJson(); });
                y.ToJson();
            });
            
            /*
            builder.Entity<Schema>().Property(x => x.Controls).HasConversion(jsonSchemaControlConverter);
            builder.Entity<Schema>().Property(x => x.RequiredDocuments).HasConversion(jsonSchemaRequiredDocumentConverter);
            builder.Entity<Schema>().Property(x => x.Progress).HasConversion(jsonSchemaProgressConverter);
            builder.Entity<SchemaProgress>().Property(x => x.Requirements).HasConversion(jsonSchemaProgressRequirementConverter);
            */
            
            builder.Entity<Schema>().Property(x => x.UpdatedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Schema>().Property(x => x.CreatedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Schema>().HasKey(x => x.Id);
            
            builder.Entity<User>(b =>
            {
                // Primary key
                b.HasKey(u => u.Id);

                // Indexes for "normalized" username and email, to allow efficient lookups
                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

                // Maps to the AspNetUsers table
                b.ToTable("Users");

                // A concurrency token for use with the optimistic concurrency checking
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                // Limit the size of columns to use efficient database types
                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                // The relationships between User and other entity types
                // Note that these relationships are configured with no navigation properties

                // Each User can have many UserClaims
                b.HasMany<IdentityUserClaim<int>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

                // Each User can have many UserLogins
                b.HasMany<IdentityUserLogin<int>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

                // Each User can have many UserTokens
                b.HasMany<IdentityUserToken<int>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany<IdentityUserRole<int>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<IdentityUserClaim<int>>(b =>
            {
                // Primary key
                b.HasKey(uc => uc.Id);

                // Maps to the AspNetUserClaims table
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<int>>(b =>
            {
                // Composite primary key consisting of the LoginProvider and the key to use
                // with that provider
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                // Limit the size of the composite key columns due to common DB restrictions
                b.Property(l => l.LoginProvider).HasMaxLength(128);
                b.Property(l => l.ProviderKey).HasMaxLength(128);

                // Maps to the AspNetUserLogins table
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<int>>(b =>
            {
                // Composite primary key consisting of the UserId, LoginProvider and Name
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                // Limit the size of the composite key columns due to common DB restrictions
                b.Property(t => t.LoginProvider).HasMaxLength(256);
                b.Property(t => t.Name).HasMaxLength(256);

                // Maps to the AspNetUserTokens table
                b.ToTable("UserTokens");
            });

            builder.Entity<IdentityRole<int>>(b =>
            {
                // Primary key
                b.HasKey(r => r.Id);

                // Index for "normalized" role name to allow efficient lookups
                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

                // Maps to the AspNetRoles table
                b.ToTable("Roles");

                // A concurrency token for use with the optimistic concurrency checking
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                // Limit the size of columns to use efficient database types
                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                // The relationships between Role and other entity types
                // Note that these relationships are configured with no navigation properties

                // Each Role can have many entries in the UserRole join table
                b.HasMany<IdentityUserRole<int>>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany<IdentityUserRole<int>>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<IdentityRoleClaim<int>>(b =>
            {
                // Primary key
                b.HasKey(rc => rc.Id);

                // Maps to the AspNetRoleClaims table
                b.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<int>>(b =>
            {
                // Primary key
                b.HasKey(r => new { r.UserId, r.RoleId });

                // Maps to the AspNetUserRoles table
                b.ToTable("UserRoles");
            });

            builder.Entity<SearchIndex>().HasKey(x => x.RowId);

            /*builder.Entity<Translation>()
                .Property(x => x.Value).ToJson();*/

            // OpenAiProject
            builder
                .Entity<OpenAiProject>()
                .OwnsMany(x => x.Data, y => 
                {
                    y.ToJson();
                })
                .HasKey(x => x.Id);

            // OpenAiUser
            builder
                .Entity<OpenAiUser>()
                .OwnsMany(x => x.Organizations, y => 
                {
                    y.ToJson();
                })
                .HasKey(x => x.Id);

            // Organization
            builder
                .Entity<Organization>()
                .OwnsMany(organizations => organizations.Addresses, addresses => 
                {
                    addresses.WithOwner(z => z.Organization);
                    addresses.Navigation(d => d.Organization).UsePropertyAccessMode(PropertyAccessMode.Property);
                    addresses.ToJson();
                })
                .OwnsMany(organizations => organizations.PhoneNumbers, phoneNumbers => 
                {
                    phoneNumbers.WithOwner(z => z.Organization);
                    phoneNumbers.Navigation(d => d.Organization).UsePropertyAccessMode(PropertyAccessMode.Property);
                    phoneNumbers.ToJson();
                })
                .OwnsMany(organizations => organizations.BankInformation, bankInformations => 
                {
                    bankInformations.WithOwner(z => z.Organization);
                    bankInformations.Navigation(d => d.Organization).UsePropertyAccessMode(PropertyAccessMode.Property);
                    bankInformations.ToJson();
                })
                .OwnsMany(o => o.Currencies, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.Statuses, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.Sections, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.ActionTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.ClaimTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.EventTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.Genders, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.MessageTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.PhoneNumberTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.ReactionTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.SystemMessageDestinations, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.MilestoneRequirementTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.ControlTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.DocumentTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.DocumentDeliveryTypes, c => 
                {
                    c.ToJson();
                })
                .OwnsMany(o => o.ApplicationBudgetTypes, c => 
                {
                    c.ToJson();
                })
                .HasKey(x => x.Id);

            builder.Entity<Organization>().Property(x => x.Name).HasDefaultValue("");
            builder.Entity<Organization>().Property(x => x.Vat).HasDefaultValue("");
            builder.Entity<Organization>().Property(x => x.Mail).HasDefaultValue("");
            builder.Entity<Organization>().Property(x => x.Url).HasDefaultValue("");
            builder.Entity<Organization>().Property(x => x.Logo).HasDefaultValue("");

            // Projects
            builder
                .Entity<Project>()
                .OwnsOne(project => project.Organization, organization => 
                {
                    organization.ToJson();
                })
                .OwnsMany(project => project.Applications, applications => 
                {
                    applications.OwnsOne(events => events.ApplicationProducer);
                    applications.OwnsOne(events => events.ApplicationApplicant);
                    applications.OwnsOne(events => events.ApplicationProjectManager);
                    applications.OwnsOne(events => events.ApplicationProductionManager);
                    applications.OwnsOne(events => events.ApplicationFinanceManager);
                    applications.OwnsOne(events => events.ApplicationScriptManager);
                    applications.OwnsOne(events => events.ApplicationDistributionManager);
                    applications.OwnsOne(events => events.ApplicationContractManager);
                    applications.ToJson();
                })
                .HasKey(x => x.Id);

            //builder.Entity<Project>().Property(x => x.Status).HasDefaultValue(Status.Default);
            builder.Entity<Project>().Property(x => x.Number).HasDefaultValue("000000");
            builder.Entity<Project>().Property(x => x.ApplicationCount).HasDefaultValue(0);
            builder.Entity<Project>().Property(x => x.UpdatedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Project>().Property(x => x.CreateDate).HasDefaultValue("0001-01-01 00:00:00");
            
            // Applications
            builder
                .Entity<Application>()
                .OwnsMany(application => application.Audits, x =>
                {
                    x.ToJson();
                })
                .OwnsMany(application => application.RequiredDocuments, x =>
                {
                    /*x.WithOwner(z => z.Schema);
                    x.Navigation(d => d.Schema).UsePropertyAccessMode(PropertyAccessMode.Property);*/
                    x.ToJson();
                })
                .OwnsMany(application => application.InternalBudgets, x =>
                {
                    x.WithOwner(z => z.Application);
                    x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsMany(application => application.Progress, x => 
                {
                    x.WithOwner(z => z.Application);
                    x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    
                    x.OwnsMany(events => events.Requirements, p =>
                    {
                        /*p.WithOwner(z => z.Event);
                        p.Navigation(d => d.Event).UsePropertyAccessMode(PropertyAccessMode.Property);*/
                    });
                    x.ToJson();
                })
                .OwnsMany(application => application.Controls, x => 
                {/*
                    x.WithOwner(z => z.Application);
                    x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);*/
                    x.ToJson();
                })
                .OwnsMany(application => application.Events, x =>
                {
                    x.WithOwner(z => z.Application);
                    x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.OwnsMany(events => events.Actions, p =>
                    {
                        p.WithOwner(z => z.Event);
                        p.Navigation(d => d.Event).UsePropertyAccessMode(PropertyAccessMode.Property);
                    });
                    x.ToJson();
                })
                .OwnsOne(application => application.Organization, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.Producer, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.ProjectManager, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.ProductionManager, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.ContractManager, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.FinanceManager, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.ScriptManager, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.DistributionManager, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .OwnsOne(application => application.Applicant, x => 
                {
                    //x.WithOwner(z => z.Application);
                    //x.Navigation(d => d.Application).UsePropertyAccessMode(PropertyAccessMode.Property);
                    x.ToJson();
                })
                .HasKey(x => x.Id);

            builder.Entity<Application>().Property(x => x.ParentId).HasDefaultValue(0);
            builder.Entity<Application>().Property(x => x.ProjectId).HasDefaultValue(0);
            builder.Entity<Application>().Property(x => x.ProjectNumber).HasDefaultValue("000000");
            //builder.Entity<Application>().Property(x => x.Status).HasDefaultValue(Status.Default);
            builder.Entity<Application>().Property(x => x.SchemaId).HasDefaultValue(0);
            builder.Entity<Application>().Property(x => x.Title).HasDefaultValue("");
            builder.Entity<Application>().Property(x => x.Number).HasDefaultValue("000000");
            builder.Entity<Application>().Property(x => x.EarlierSupportTotalAmount).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Application>().Property(x => x.InternalBudgetsTotalAmount).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Application>().Property(x => x.MilestonePayoutTotalAmount).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Application>().Property(x => x.EarlierSupportTotalAmount).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Application>().Property(x => x.InternalBudgetsApproved).HasDefaultValue(0);
            builder.Entity<Application>().Property(x => x.BudgetAmount).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Application>().Property(x => x.AppliedAmount).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Application>().Property(x => x.UpdatedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Application>().Property(x => x.CreatedDate).HasDefaultValue("0001-01-01 00:00:00");
            
            // ApplicationStates
            builder
                .Entity<ApplicationState>()
                .HasKey(x => x.Id);

            builder.Entity<ApplicationState>().Property(x => x.OrganizationId).HasDefaultValue(0);
            builder.Entity<ApplicationState>().Property(x => x.UserId).HasDefaultValue(0);
            builder.Entity<ApplicationState>().Property(x => x.ApplicationId).HasDefaultValue(0);
            //builder.Entity<ApplicationState>().Property(x => x.ApplicationType).HasDefaultValue(ApplicationType.Default);
            builder.Entity<ApplicationState>().Property(x => x.Title).HasDefaultValue("");
            builder.Entity<ApplicationState>().Property(x => x.CreatedDate).HasDefaultValue("0001-01-01 00:00:00");


            // Milestones
            builder
                .Entity<Milestone>()
                .OwnsMany(milestone => milestone.Requirements, requirements => 
                {
                    requirements.WithOwner(z => z.Milestone);
                    requirements.Navigation(d => d.Milestone).UsePropertyAccessMode(PropertyAccessMode.Property);
                    requirements.ToJson();
                })
                .OwnsMany(milestone => milestone.Payments, payments => 
                {
                    payments.WithOwner(z => z.Milestone);
                    payments.Navigation(d => d.Milestone).UsePropertyAccessMode(PropertyAccessMode.Property);
                    payments.ToJson();
                })
                .HasKey(x => x.Id);

            builder.Entity<Milestone>().Property(x => x.ApplicationId).HasDefaultValue(0);
            //builder.Entity<Milestone>().Property(x => x.Status).HasDefaultValue(Status.Default);
            builder.Entity<Milestone>().Property(x => x.Amount ).HasDefaultValue(0).HasPrecision(18,4);
            builder.Entity<Milestone>().Property(x => x.CreatedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Milestone>().Property(x => x.ExpireDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Milestone>().Property(x => x.IsLocked).HasDefaultValue(0);
            builder.Entity<Milestone>().Property(x => x.RequirementsCount).HasDefaultValue(0);
            builder.Entity<Milestone>().Property(x => x.RequirementsDeliveredCount).HasDefaultValue(0);
            builder.Entity<Milestone>().Property(x => x.RequirementsApproved).HasDefaultValue(0);
            builder.Entity<Milestone>().Property(x => x.RequirementsExpired).HasDefaultValue(0);
            builder.Entity<Milestone>().Property(x => x.TotalPayments).HasDefaultValue(0).HasPrecision(18,4);

            // Documents
            builder
                .Entity<Document>()
                .OwnsMany(document => document.Metadata, metadata => 
                {
                    metadata.WithOwner(z => z.Document);
                    metadata.Navigation(d => d.Document).UsePropertyAccessMode(PropertyAccessMode.Property);
                    metadata.ToJson();
                })
                .HasKey(x => x.Id);

            builder.Entity<Document>().Property(x => x.ApplicationId).HasDefaultValue(0);
            //builder.Entity<Document>().Property(x => x.Status).HasDefaultValue(Status.Default);
            //builder.Entity<Document>().Property(x => x.DocumentType).HasDefaultValue(DocumentType.Default);
            builder.Entity<Document>().Property(x => x.FileName).HasDefaultValue("");
            builder.Entity<Document>().Property(x => x.MimeType).HasDefaultValue("");
            builder.Entity<Document>().Property(x => x.Extension).HasDefaultValue("");
            builder.Entity<Document>().Property(x => x.Path).HasDefaultValue("");
            builder.Entity<Document>().Property(x => x.Phrases).HasDefaultValue("");
            builder.Entity<Document>().Property(x => x.Summarize).HasDefaultValue("");
            builder.Entity<Document>().Property(x => x.IsDelivered).HasDefaultValue(0);
            builder.Entity<Document>().Property(x => x.IsSigned).HasDefaultValue(0);
            builder.Entity<Document>().Property(x => x.IsCertified).HasDefaultValue(0);
            builder.Entity<Document>().Property(x => x.IsLocked).HasDefaultValue(0);
            builder.Entity<Document>().Property(x => x.DeliverDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Document>().Property(x => x.SignedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Document>().Property(x => x.CertifiedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Document>().Property(x => x.LockedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Document>().Property(x => x.CreatedDate).HasDefaultValue("0001-01-01 00:00:00");
            //builder.Entity<Document>().Property(x => x.DeliveryType).HasDefaultValue(DocumentDeliveryType.Default);

            // Users
            builder
                .Entity<User>()
                .OwnsMany(user => user.Addresses, addresses => 
                {
                    addresses.WithOwner(z => z.User);
                    addresses.Navigation(d => d.User).UsePropertyAccessMode(PropertyAccessMode.Property);
                    addresses.ToJson();
                })
                .OwnsMany(user => user.PhoneNumbers, phoneNumbers => 
                {
                    phoneNumbers.WithOwner(z => z.User);
                    phoneNumbers.Navigation(d => d.User).UsePropertyAccessMode(PropertyAccessMode.Property);
                    phoneNumbers.ToJson();
                })
                .OwnsMany(user => user.Organizations, organizations => 
                {
                    organizations.WithOwner(z => z.User);
                    organizations.Navigation(d => d.User).UsePropertyAccessMode(PropertyAccessMode.Property);
                    organizations.ToJson();
                })
                .OwnsMany(user => user.Statistics, r => 
                {
                    r.WithOwner(z => z.User);
                    r.Navigation(d => d.User).UsePropertyAccessMode(PropertyAccessMode.Property);
                    r.ToJson();
                })
                .HasKey(x => x.Id);

            //builder.Entity<User>().Property(x => x.Status).HasDefaultValue(Status.Default);
            builder.Entity<User>().Property(x => x.FirstName).HasDefaultValue("");
            builder.Entity<User>().Property(x => x.LastName).HasDefaultValue("");
            builder.Entity<User>().Property(x => x.MessageCount).HasDefaultValue(0);
            builder.Entity<User>().Property(x => x.Type).HasDefaultValue(UserType.Default);

            // Messages
            builder
                .Entity<Message>()
                .OwnsOne(message => message.Receiver, x => x.ToJson())
                .HasKey(x => x.Id);

            //builder.Entity<Message>().Property(x => x.MessageType).HasDefaultValue(MessageType.Default);
            //builder.Entity<Message>().Property(x => x.Status).HasDefaultValue(Status.Default);
            builder.Entity<Message>().Property(x => x.ProjectId).HasDefaultValue(0);
            builder.Entity<Message>().Property(x => x.ProjectNumber).HasDefaultValue("");
            builder.Entity<Message>().Property(x => x.ProjectTitle).HasDefaultValue("");
            builder.Entity<Message>().Property(x => x.ApplicationId).HasDefaultValue(0);
            builder.Entity<Message>().Property(x => x.ApplicationTitle).HasDefaultValue("");
            //builder.Entity<Message>().Property(x => x.DocumentType).HasDefaultValue(DocumentType.Default);
            builder.Entity<Message>().Property(x => x.Title).HasDefaultValue("");
            builder.Entity<Message>().Property(x => x.CreatedDate).HasDefaultValue("0001-01-01 00:00:00");
            builder.Entity<Message>().Property(x => x.ExpireDate).HasDefaultValue("0001-01-01 00:00:00");
            
            // Metric
            builder
                .Entity<Control>()
                .HasKey(x => x.Id);

            builder.Entity<Control>().Property(x => x.ControlTypeId).HasDefaultValue(1);
            builder.Entity<Control>().Property(x => x.ValueType).HasDefaultValue("");
            builder.Entity<Control>().Property(x => x.BaseStructure).HasDefaultValue("");
            

            // index
            /*builder.Entity<Application>()
                .HasIndex(a => new { a.Id, a.Title, a.UpdatedDate })
                .HasDatabaseName("idx_application");*/
            
            builder.Entity<DocumentType>().HasData(
                new () { Id = 1, Names = new List<string> { "Standard", "Default", "Standard", "Standard", "Defecto", "Par défaut", "Standard", "Standard" } },
                new () { Id = 3, Names = new List<string> { "Beskrivning av vad utvecklingen avser", "Description of what the development pertains to", "Beskrivelse af hvad udviklingen vedrører", "Beschreibung dessen, was die Entwicklung betrifft", "Descripción de a qué se refiere el desarrollo", "Description de ce que le développement concerne", "Descrizione di cosa riguarda lo sviluppo", "Beskrivelse av hva utviklingen gjelder" } },
                new () { Id = 6, Names = new List<string> { "Bekräftad finansiering dokumentation", "Documentation of confirmed financing", "Dokumentation af bekræftet finansiering", "Dokumentation der bestätigten Finanzierung", "Documentación de la financiación confirmada", "Documentation de financement confirmé", "Documentazione di finanziamento confermata", "Dokumentasjon av bekreftet finansiering" } },
                new () { Id = 10, Names = new List<string> { "Övrigt material", "Other material", "Andet materiale", "Anderes Material", "Otro material", "Autre matériel", "Altro materiale", "Annet materiale" } },
                new () { Id = 12, Names = new List<string> { "Projektbeskrivning", "Project description", "Projektbeskrivelse", "Projektbeschreibung", "Descripción del proyecto", "Description du projet", "Descrizione del progetto", "Prosjektbeskrivelse" } },
                new () { Id = 14, Names = new List<string> { "Manus", "Script", "Manuskript", "Drehbuch", "Guión", "Scénario", "Sceneggiatura", "Manus" } },
                new () { Id = 16, Names = new List<string> { "Synopsis/Treatment", "Synopsis/Treatment", "Synopsis/Behandling", "Synopsis/Behandlung", "Sinopsis/Tratamiento", "Synopsis/Traitement", "Sinossi/Trattamento", "Synopsis/Behandling" } },
                new () { Id = 17, Names = new List<string> { "Tidsplan", "Timeline", "Tidsplan", "Zeitplan", "Cronograma", "Calendrier", "Pianificazione", "Tidsplan" } },
                new () { Id = 21, Names = new List<string> { "Budget", "Budget", "Budget", "Budget", "Presupuesto", "Budget", "Bilancio", "Budsjett" } },
                new () { Id = 22, Names = new List<string> { "Finansieringsplan", "Financing Plan", "Finansieringsplan", "Finanzierungsplan", "Plan de financiación", "Plan de financement", "Piano di finanziamento", "Finansieringsplan" } },
                new () { Id = 59, Names = new List<string> { "Rekommendation till styrelsen", "Recommendation to the Board", "Anbefaling til bestyrelsen", "Empfehlung an den Vorstand", "Recomendación al consejo", "Recommendation au conseil", "Raccomandazione al consiglio", "Anbefaling til styret" } },
                new () { Id = 60, Names = new List<string> { "Avslagsbrev", "Rejection Letter", "Afslagbrev", "Ablehnungsbrief", "Carta de rechazo", "Lettre de rejet", "Lettera di rifiuto", "Avslagsbrev" } },
                new () { Id = 69, Names = new List<string> { "Uppladdade instillinger", "Uploaded Settings", "Uploadede indstillinger", "Hochgeladene Einstellungen", "Configuraciones subidas", "Paramètres téléchargés", "Impostazioni caricate", "Lastede opp innstillinger" } },
                new () { Id = 70, Names = new List<string> { "Interne vurderinger", "Internal Assessments", "Interne vurderinger", "Interne Bewertungen", "Evaluaciones internas", "Évaluations internes", "Valutazioni interne", "Interne vurderinger" } },
                new () { Id = 74, Names = new List<string> { "Disponert bevilgning", "Allocated Grant", "Bevilget tilskud", "Zugewiesener Zuschuss", "Subvención asignada", "Subvention allouée", "Contributo assegnato", "Tildelt tilskudd" } },
                new () { Id = 75, Names = new List<string> { "Team- och skådespelarlista", "Team and Cast List", "Team og skuespillerliste", "Team- und Schauspielerliste", "Lista de equipo y reparto", "Liste de l'équipe et des acteurs", "Elenco del team e del cast", "Team og skuespillerliste" } },
                new () { Id = 78, Names = new List<string> { "Spendbudget", "Expenditure Budget", "Udgiftsbudget", "Ausgabenbudget", "Presupuesto de gastos", "Budget des dépenses", "Budget delle spese", "Utgiftsbudsjett" } },
                new () { Id = 79, Names = new List<string> { "Regiavtal", "Director's Agreement", "Instruktøraftale", "Regievertrag", "Acuerdo del director", "Accord de réalisation", "Contratto di regia", "Regiavtale" } },
                new () { Id = 80, Names = new List<string> { "FIV riktlinjer", "FIV Guidelines", "FIV-retningslinjer", "FIV-Richtlinien", "Directrices FIV", "Directives FIV", "Linee guida FIV", "FIV-retningslinjer" } },
                new () { Id = 81, Names = new List<string> { "Manusavtal", "Script Agreement", "Manusaftale", "Drehbuchvertrag", "Acuerdo de guión", "Accord de scénario", "Accordo di sceneggiatura", "Manusavtale" } },
                new () { Id = 82, Names = new List<string> { "Recoupmentplan", "Recoupment Plan", "Tilbagebetalingsplan", "Rückzahlungsplan", "Plan de recuperación", "Plan de recouvrement", "Piano di recupero", "Tilbakebetalingsplan" } },
                new () { Id = 83, Names = new List<string> { "Rättighetsupplåtelse", "Grant of Rights", "Rettighedsoverlade", "Rechteeinräumung", "Cesión de derechos", "Cession de droits", "Cessione dei diritti", "Tilgang av rettigheter" } },
                new () { Id = 84, Names = new List<string> { "Distributionsavtal", "Distribution Agreement", "Distributionsaftale", "Vertriebsvereinbarung", "Acuerdo de distribución", "Accord de distribution", "Contratto di distribuzione", "Distribusjonsavtale" } },
                new () { Id = 85, Names = new List<string> { "CAMA / (LOC)", "CAMA / (LOC)", "CAMA / (LOC)", "CAMA / (LOC)", "CAMA / (LOC)", "CAMA / (LOC)", "CAMA / (LOC)", "CAMA / (LOC)" } },
                new () { Id = 86, Names = new List<string> { "Likviditetsplan", "Liquidity Plan", "Likviditetsplan", "Liquiditätsplan", "Plan de liquidez", "Plan de liquidité", "Piano di liquidità", "Likviditetsplan" } },
                new () { Id = 87, Names = new List<string> { "Tidsplan / Produktionsplan", "Timeline / Production Plan", "Tidsplan / Produktionsplan", "Zeitplan / Produktionsplan", "Cronograma / Plan de producción", "Calendrier / Plan de production", "Pianificazione / Piano di produzione", "Tidsplan / Produksjonsplan" } },
                new () { Id = 88, Names = new List<string> { "Producentens egeninsats", "Producer's own input", "Producentens egen indsats", "Eigene Leistung des Produzenten", "La propia aportación del productor", "Contribution propre du producteur", "Contributo personale del produttore", "Produsentens egen innsats" } },
                new () { Id = 89, Names = new List<string> { "Internationellt huvudavtal", "International Main Agreement", "International hovedaftale", "Internationaler Hauptvertrag", "Acuerdo principal internacional", "Accord principal international", "Accordo principale internazionale", "Internasjonal hovedavtale" } },
                new () { Id = 90, Names = new List<string> { "Salesavtal", "Sales Agreement", "Salgsafltale", "Verkaufsvertrag", "Contrato de ventas", "Contrat de vente", "Contratto di vendita", "Salgsavtale" } },
                new () { Id = 91, Names = new List<string> { "Locationlista", "Location List", "Stedliste", "Standortliste", "Lista de ubicaciones", "Liste des lieux", "Elenco delle località", "Lokasjonsliste" } },
                new () { Id = 92, Names = new List<string> { "CAMAavtal", "CAMA Agreement", "CAMA-aftale", "CAMA-Vereinbarung", "Acuerdo CAMA", "Accord CAMA", "Accordo CAMA", "CAMA-avtale" } },
                new () { Id = 93, Names = new List<string> { "Bondavtal", "Bond Agreement", "Bond-aftale", "Anleihevertrag", "Contrato de bonos", "Contrat d'obligations", "Contratto obbligazionario", "Obligasjonsavtale" } },
                new () { Id = 94, Names = new List<string> { "Side letters", "Side Letters", "Side breve", "Seitenbriefe", "Cartas marginales", "Lettres de côté", "Lettere accessorie", "Sidebrev" } },
                new () { Id = 95, Names = new List<string> { "Avtal mellan parter", "Inter-Party Agreement", "Inter-party-aftale", "Inter-party-Vereinbarung", "Acuerdo entre partes", "Accord entre parties", "Accordo tra le parti", "Avtale mellom parter" } },
                new () { Id = 96, Names = new List<string> { "Tilläggsavtal", "Supplementary Agreement", "Supplerende aftale", "Ergänzungsvereinbarung", "Acuerdo suplementario", "Accord supplémentaire", "Accordo supplementare", "Tilleggsavtale" } },
                new () { Id = 97, Names = new List<string> { "Leverantörslista", "Supplier List", "Leverandørliste", "Lieferantenliste", "Lista de proveedores", "Liste des fournisseurs", "Elenco dei fornitori", "Leverandørliste" } },
                new () { Id = 98, Names = new List<string> { "Slutredovisning", "Final Report", "Slutrapport", "Endbericht", "Informe final", "Rapport final", "Rapporto finale", "Sluttrapport" } },
                new () { Id = 99, Names = new List<string> { "Upphovsrättsavtal Manus och Regi", "Copyright Agreement Script and Direction", "Ophavsretsaftale Manuskript og Instruktion", "Urheberrechtsvereinbarung Drehbuch und Regie", "Acuerdo de derechos de autor Guión y Dirección", "Accord de droits d'auteur Scénario et Réalisation", "Accordo sui diritti d'autore Sceneggiatura e Regia", "Opphavsrettsavtale Manus og Regi" } },
                new () { Id = 100, Names = new List<string> { "Finansieringsunderlag övriga parter, LOC mm", "Financing Documentation for Other Parties, LOC etc.", "Finansieringsdokumentation for andre parter, LOC osv.", "Finanzierungsunterlagen für andere Parteien, LOC usw.", "Documentación de financiación para otras partes, LOC etc.", "Documentation de financement pour d'autres parties, LOC, etc.", "Documentazione di finanziamento per altre parti, LOC ecc.", "Finansieringsdokumentasjon for andre parter, LOC mm" } },
                new () { Id = 111, Names = new List<string> { "Distributionsplan", "Distribution Plan", "Distributionsplan", "Vertriebsplan", "Plan de distribución", "Plan de distribution", "Piano di distribuzione", "Distribusjonsplan" } },
                new () { Id = 112, Names = new List<string> { "CV på nyckelfunktioner", "CV of Key Personnel", "CV af nøglepersonale", "CV der Schlüsselpersonen", "CV del personal clave", "CV des personnels clés", "CV del personale chiave", "CV av nøkkelpersonell" } },
                new () { Id = 113, Names = new List<string> { "Ekonomisk redovisning", "Financial Report", "Økonomisk rapport", "Finanzbericht", "Informe financiero", "Rapport financier", "Rapporto finanziario", "Finansiell rapport" } },
                new () { Id = 114, Names = new List<string> { "Konstnärlig redovisning", "Artistic Report", "Kunstnerisk rapport", "Künstlerischer Bericht", "Informe artístico", "Rapport artistique", "Rapporto artistico", "Kunstnerisk rapport" } },
                new () { Id = 66, Names = new List<string> { "Standardmall", "General template", "Standardmall", "General template", "General template", "General template", "General template", "Standardmall" } },
                new () { Id = 9, Names = new List<string> { "Faktura för rat 1", "Invoice for rate 1", "Faktura för rat 1", "Invoice for rate 1", "Invoice for rate 1", "Invoice for rate 1", "Invoice for rate 1", "Faktura för rat 1" } }
            );
            
            
            builder.Entity<MessageType>().HasData(
                new () { 
                    Id = 1, 
                    Names = new List<string> 
                    { 
                        "Koppla applikation till projekt", "Connect application to project", 
                        "Tilslut applikation til projekt", "Anwendung mit Projekt verbinden", 
                        "Conectar aplicación al proyecto", "Connecter l'application au projet", 
                        "Collegare l'applicazione al progetto", "Koble applikasjonen til prosjektet" 
                    } 
                },
                new () { 
                    Id = 9, 
                    Names = new List<string> 
                    { 
                        "Skapa avslagsbrev", "Create rejection letter", 
                        "Opret afslag brev", "Ablehnungsbrief erstellen", 
                        "Crear carta de rechazo", "Créer une lettre de rejet", 
                        "Creare la lettera di rifiuto", "Opprett avslagsbrev" 
                    } 
                },
                new () { 
                    Id = 22, 
                    Names = new List<string> 
                    { 
                        "Application kopplad till projekt", "Application linked to project", 
                        "Ansøgning forbundet til projekt", "Anwendung mit Projekt verknüpft", 
                        "Aplicación vinculada al proyecto", "Application liée au projet", 
                        "Applicazione collegata al progetto", "Søknad knyttet til prosjektet" 
                    } 
                },
                new () { 
                    Id = 901, 
                    Names = new List<string> 
                    { 
                        "Bedömning ja", "Assessment yes", 
                        "Vurdering ja", "Beurteilung ja", 
                        "Evaluación sí", "Évaluation oui", 
                        "Valutazione sì", "Vurdering ja" 
                    } 
                },
                new () { 
                    Id = 902, 
                    Names = new List<string> 
                    { 
                        "Bedömning nej", "Assessment no", 
                        "Vurdering nej", "Beurteilung nein", 
                        "Evaluación no", "Évaluation non", 
                        "Valutazione no", "Vurdering nei" 
                    } 
                },
                new () { 
                    Id = 903, 
                    Names = new List<string> 
                    { 
                        "Bedömning tilldela roller", "Assessment assign roles", 
                        "Vurdering tildele roller", "Beurteilung Rollen zuweisen", 
                        "Evaluación asignar roles", "Évaluation attribuer des rôles", 
                        "Valutazione assegnare ruoli", "Vurdering tildele roller" 
                    } 
                },
                new () { 
                    Id = 916, 
                    Names = new List<string> 
                    { 
                        "Bedömning nej och stäng", "Assessment no and close", 
                        "Vurdering nei og lukk", "Beurteilung nein und schließen", 
                        "Evaluación no y cerrar", "Évaluation non et fermer", 
                        "Valutazione no e chiudi", "Vurdering nei og steng" 
                    } 
                },
                new () { 
                    Id = 1001, 
                    Names = new List<string> 
                    { 
                        "Ny användare", "New user", 
                        "Ny bruger", "Neuer Benutzer", 
                        "Nuevo usuario", "Nouvel utilisateur", 
                        "Nuovo utente", "Ny bruker" 
                    } 
                },
                new () { 
                    Id = 1002, 
                    Names = new List<string> 
                    { 
                        "Från client", "From client", 
                        "Fra klient", "Vom Client", 
                        "Desde el cliente", "Du client", 
                        "Dal client", "Fra klient" 
                    } 
                },
                new () { 
                    Id = 1003, 
                    Names = new List<string> 
                    { 
                        "Till client", "To client", 
                        "Til klient", "Zum Client", 
                        "Al cliente", "Au client", 
                        "Al client", "Til client" 
                    } 
                },
                new () { 
                    Id = 1004, 
                    Names = new List<string> 
                    { 
                        "AutoStatus1", "AutoStatus1", 
                        "AutoStatus1", "AutoStatus1", 
                        "AutoStatus1", "AutoStatus1", 
                        "AutoStatus1", "AutoStatus1" 
                    } 
                },
                new () { 
                    Id = 1005, 
                    Names = new List<string> 
                    { 
                        "AutoStatus2", "AutoStatus2", 
                        "AutoStatus2", "AutoStatus2", 
                        "AutoStatus2", "AutoStatus2", 
                        "AutoStatus2", "AutoStatus2" 
                    } 
                },
                new () { 
                    Id = 1006, 
                    Names = new List<string> 
                    { 
                        "Fra client wizard", "From client wizard", 
                        "Fra klientguide", "Vom Client-Assistenten", 
                        "Desde el asistente del cliente", "Du client assistant", 
                        "Dal wizard client", "Fra klientveiviser" 
                    } 
                },
                new () { 
                    Id = 1007, 
                    Names = new List<string> 
                    { 
                        "Til client wizard", "To client wizard", 
                        "Til klientguide", "Zum Client-Assistenten", 
                        "Al asistente del cliente", "Au client assistant", 
                        "Al wizard client", "Til klientveiviser" 
                    } 
                },
                new () { 
                    Id = 2001, 
                    Names = new List<string> 
                    { 
                        "Avancerad", "Advanced", 
                        "Avanceret", "Erweitert", 
                        "Avanzado", "Avancé", 
                        "Avanzato", "Avansert" 
                    } 
                },
                new () { 
                    Id = 2002, 
                    Names = new List<string> 
                    { 
                        "Ofullständig", "Incomplete", 
                        "Ufuldstændig", "Unvollständig", 
                        "Incompleto", "Incomplet", 
                        "Incompleto", "Ufullstendig" 
                    } 
                },
                new () { 
                    Id = 2003, 
                    Names = new List<string> 
                    { 
                        "Klar för greenlight", "Ready for greenlight", 
                        "Klar til grønt lys", "Bereit für grünes Licht", 
                        "Listo para la luz verde", "Prêt pour le feu vert", 
                        "Pronto per il via libera", "Klar til grønt lys" 
                    } 
                },
                new () { 
                    Id = 2004, 
                    Names = new List<string> 
                    { 
                        "Fler dokument", "More documents", 
                        "Flere dokumenter", "Mehr Dokumente", 
                        "Más documentos", "Plus de documents", 
                        "Più documenti", "Flere dokumenter" 
                    } 
                },
                new () { 
                    Id = 3001, 
                    Names = new List<string> 
                    { 
                        "Requirements", "Requirements", 
                        "Krav", "Anforderungen", 
                        "Requisitos", "Exigences", 
                        "Requisiti", "Krav" 
                    } 
                },
                            new () { 
                    Id = 4001, 
                    Names = new List<string> 
                    { 
                        "Nekat projekt att registrera", "Denied project to register", 
                        "Nægtet projekt at registrere", "Projekt abgelehnt zu registrieren", 
                        "Proyecto denegado para registrar", "Projet refusé à enregistrer", 
                        "Progetto negato da registrare", "Prosjekt avvist for registrering" 
                    } 
                },
                new () { 
                    Id = 4002, 
                    Names = new List<string> 
                    { 
                        "Beslutsbrev att registrera", "Decision letter to register", 
                        "Afgørelsesbrev at registrere", "Entscheidungsbrief zu registrieren", 
                        "Carta de decisión para registrar", "Lettre de décision à enregistrer", 
                        "Lettera di decisione da registrare", "Beslutningsbrev for registrering" 
                    } 
                },
                new () { 
                    Id = 4003, 
                    Names = new List<string> 
                    { 
                        "Mötesprotokoll att registrera", "Meeting protocol to register", 
                        "Mødeprotokol at registrere", "Protokoll der Sitzung zu registrieren", 
                        "Protocolo de reunión para registrar", "Protocole de réunion à enregistrer", 
                        "Protocollo della riunione da registrare", "Møteprotokoll for registrering" 
                    } 
                },
                new () { 
                    Id = 4004, 
                    Names = new List<string> 
                    { 
                        "Nekat projekt och stäng för registrering", "Denied project and close to register", 
                        "Nægtet projekt og luk for at registrere", "Projekt abgelehnt und schließen zu registrieren", 
                        "Proyecto denegado y cerrar para registrar", "Projet refusé et fermer à enregistrer", 
                        "Progetto negato e chiudi per registrare", "Prosjekt avvist og lukk for registrering" 
                    } 
                },
                new () { 
                    Id = 5001, 
                    Names = new List<string> 
                    { 
                        "Produktionsroll tilldelad", "Production role assigned", 
                        "Produktionsrolle tildelt", "Produktionsrolle zugewiesen", 
                        "Rol de producción asignado", "Rôle de production attribué", 
                        "Ruolo di produzione assegnato", "Produksjonsrolle tildelt" 
                    } 
                },
                new () { 
                    Id = 5002, 
                    Names = new List<string> 
                    { 
                        "Finansroll tilldelad", "Finance role assigned", 
                        "Finansrolle tildelt", "Finanzrolle zugewiesen", 
                        "Rol de finanzas asignado", "Rôle des finances attribué", 
                        "Ruolo finanziario assegnato", "Finansrolle tildelt" 
                    } 
                },
                new () { 
                    Id = 5003, 
                    Names = new List<string> 
                    { 
                        "Manuskriptkonsult tilldelad", "Script consult assigned", 
                        "Manuskriptkonsulent tildelt", "Drehbuchberatung zugewiesen", 
                        "Consulta de guión asignado", "Consultation de script attribué", 
                        "Consulenza per la sceneggiatura assegnata", "Manuskriptkonsulent tildelt" 
                    } 
                },
                new () { 
                    Id = 5004, 
                    Names = new List<string> 
                    { 
                        "Distributionskonsult tilldelad", "Distribution consult assigned", 
                        "Distributionskonsulent tildelt", "Verteilungsberatung zugewiesen", 
                        "Consulta de distribución asignado", "Consultation de distribution attribuée", 
                        "Consulenza per la distribuzione assegnata", "Distributionskonsulent tildelt" 
                    } 
                },
                new () { 
                    Id = 5005, 
                    Names = new List<string> 
                    { 
                        "Planera möte med produktionsgrupp", "Plan meeting with production group", 
                        "Planlæg møde med produktionsgruppen", "Planen Sie ein Treffen mit der Produktionsgruppe", 
                        "Plan reunión con el grupo de producción", "Planifier une réunion avec le groupe de production", 
                        "Pianifica una riunione con il gruppo di produzione", "Planlegg møte med produksjonsgruppen" 
                    } 
                },
                new () { 
                    Id = 5006, 
                    Names = new List<string> 
                    { 
                        "Produktionsmeddelande", "Production message", 
                        "Produktionsbesked", "Produktionsnachricht", 
                        "Mensaje de producción", "Message de production", 
                        "Messaggio di produzione", "Produksjonsmelding" 
                    } 
                },
                new () { 
                    Id = 5007, 
                    Names = new List<string> 
                    { 
                        "Utvecklingsprocessens början", "Development process beginning", 
                        "Udviklingsprocess start", "Beginn des Entwicklungsprozesses", 
                        "Inicio del proceso de desarrollo", "Début du processus de développement", 
                        "Inizio del processo di sviluppo", "Begynnelse av utviklingsprosessen" 
                    } 
                },
                new () { 
                    Id = 5008, 
                    Names = new List<string> 
                    { 
                        "Planera utvecklingsmöte", "Plan development meeting", 
                        "Planlæg udviklingsmøde", "Planen Sie ein Entwicklungstreffen", 
                        "Plan reunión de desarrollo", "Planifier une réunion de développement", 
                        "Pianifica una riunione di sviluppo", "Planlegg utviklingsmøte" 
                    } 
                },
                new () { 
                    Id = 5009, 
                    Names = new List<string> 
                    { 
                        "Projektet går vidare till produktion", "Project moving to production", 
                        "Projektet går videre til produktionen", "Projekt geht in die Produktion", 
                        "Proyecto en movimiento a producción", "Projet en cours de production", 
                        "Progetto che si sposta in produzione", "Prosjekt beveger seg til produksjon" 
                    } 
                },
                new () { 
                    Id = 5010, 
                    Names = new List<string> 
                    { 
                        "Skapa godkännandebrev", "Create approved letter", 
                        "Opret godkendt brev", "Genehmigungsschreiben erstellen", 
                        "Crear carta de aprobación", "Créer une lettre d'approbation", 
                        "Creare una lettera di approvazione", "Opprett godkjennelsesbrev" 
                    } 
                },
                            new () { 
                    Id = 5011, 
                    Names = new List<string> 
                    { 
                        "Informera producenten att projektet är klart för grönt ljus", 
                        "Tell producer project is ready for greenlite.", 
                        "Fortæl producenten, at projektet er klar til grønt lys.", 
                        "Sagen Sie dem Produzenten, dass das Projekt bereit für grünes Licht ist.", 
                        "Dile al productor que el proyecto está listo para la luz verde.", 
                        "Dites au producteur que le projet est prêt pour le feu vert.", 
                        "Dite al produttore che il progetto è pronto per il via libera.", 
                        "Fortell produsenten at prosjektet er klart for grønt lys." 
                    } 
                },
                new () { 
                    Id = 5012, 
                    Names = new List<string> 
                    { 
                        "Tilldela avtalsroll", 
                        "Assign agreement role.", 
                        "Tildel aftalerollen.", 
                        "Weisen Sie die Vertragsrolle zu.", 
                        "Asignar rol de acuerdo.", 
                        "Attribuer le rôle d'accord.", 
                        "Assegna il ruolo di accordo.", 
                        "Tildele avtalerollen." 
                    } 
                },
                new () { 
                    Id = 5013, 
                    Names = new List<string> 
                    { 
                        "Godkänt brev skickat till producent", 
                        "Approved letter sent to producer", 
                        "Godkendt brev sendt til producenten", 
                        "Genehmigungsbrief an Produzenten gesendet", 
                        "Carta de aprobación enviada al productor", 
                        "Lettre d'approbation envoyée au producteur", 
                        "Lettera di approvazione inviata al produttore", 
                        "Godkjennelsesbrev sendt til produsenten" 
                    } 
                },
                new () { 
                    Id = 5014, 
                    Names = new List<string> 
                    { 
                        "Väntar på andra finansiärer", 
                        "Await other financiers", 
                        "Afventer andre finansfolk", 
                        "Warten auf andere Finanziers", 
                        "A la espera de otros financiadores", 
                        "En attente des autres financiers", 
                        "In attesa di altri finanziatori", 
                        "Venter på andre finansfolk" 
                    } 
                },
                new () { 
                    Id = 5015, 
                    Names = new List<string> 
                    { 
                        "Avtalsprocess påbörjad", 
                        "Agreement process started.", 
                        "Aftaleproces startet.", 
                        "Vertragsprozess gestartet.", 
                        "Proceso de acuerdo iniciado.", 
                        "Processus d'accord commencé.", 
                        "Processo di accordo avviato.", 
                        "Avtaleprosessen startet." 
                    } 
                },
                new () { 
                    Id = 5016, 
                    Names = new List<string> 
                    { 
                        "Avtalsprocess påbörjad, samla in ytterligare material.", 
                        "Agreement process started, collect additional material.", 
                        "Aftaleproces påbegyndt, samle yderligere materiale.", 
                        "Vertragsprozess gestartet, zusätzliches Material sammeln.", 
                        "Proceso de acuerdo iniciado, recopilar material adicional.", 
                        "Processus d'accord commencé, recueillir du matériel supplémentaire.", 
                        "Processo di accordo iniziato, raccogliere materiale aggiuntivo.", 
                        "Avtaleprosessen startet, samle ytterligere materiale." 
                    } 
                },
                new () { 
                    Id = 5017, 
                    Names = new List<string> 
                    { 
                        "Kontrollera om avtalsprocessen är avslutad meddelande.", 
                        "Check if agreement process is finished message.", 
                        "Kontroller, om aftaleprocessen er færdig.", 
                        "Überprüfen Sie, ob der Vertragsprozess abgeschlossen ist.", 
                        "Verifique si el proceso de acuerdo está terminado.", 
                        "Vérifiez si le processus d'accord est terminé.", 
                        "Controllare se il processo di accordo è terminato.", 
                        "Kontroller, om avtaleprosessen er ferdig." 
                    } 
                },
                new () { 
                    Id = 5018, 
                    Names = new List<string> 
                    { 
                        "Alla delar har skrivit under avtalet meddelande.", 
                        "All parts have signed the agreement message.", 
                        "Alle dele har underskrevet aftalen.", 
                        "Alle Teile haben die Vereinbarung unterzeichnet.", 
                        "Todas las partes han firmado el acuerdo.", 
                        "Toutes les parties ont signé l'accord.", 
                        "Tutte le parti hanno firmato l'accordo.", 
                        "Alle delene har signert avtalen." 
                    } 
                },
                new () { 
                    Id = 5019, 
                    Names = new List<string> 
                    { 
                        "Registrera avtalsmeddelande.", 
                        "Register agreement message.", 
                        "Registrer aftale besked.", 
                        "Vereinbarungsnachricht registrieren.", 
                        "Registrar mensaje de acuerdo.", 
                        "Enregistrer le message d'accord.", 
                        "Registrare il messaggio di accordo.", 
                        "Registrer avtale beskjed." 
                    } 
                },
                new () { 
                    Id = 5020, 
                    Names = new List<string> 
                    { 
                        "Se upp för klippmeddelande.", 
                        "Look out for cut message.", 
                        "Pas på klip meddelelse.", 
                        "Achten Sie auf Schnittnachricht.", 
                        "Cuidado con el mensaje de corte.", 
                        "Attention au message de coupe.", 
                        "Attenzione al messaggio di taglio.", 
                        "Pass på klipp beskjed." 
                    } 
                },
                new () { 
                    Id = 5021, 
                    Names = new List<string> 
                    { 
                        "Se upp för PR-meddelande.", 
                        "Look out for PR message.", 
                        "Se efter PR-besked.", 
                        "Achten Sie auf PR-Nachricht.", 
                        "Cuidado con el mensaje de RP.", 
                        "Attention au message RP.", 
                        "Attenzione al messaggio di PR.", 
                        "Pass på PR-melding." 
                    } 
                },
                new () { 
                    Id = 5022, 
                    Names = new List<string> 
                    { 
                        "Se upp för klippavtalsroll meddelande.", 
                        "Look out for cut agreement role message.", 
                        "Pas på klipaftalerolle.", 
                        "Achten Sie auf Schnittvereinbarungsrolle Besch.", 
                        "Cuidado con el mensaje de rol de acuerdo de corte.", 
                        "Attention au message de rôle d'accord de coupe.", 
                        "Attenzione al messaggio di ruolo dell'accordo di taglio.", 
                        "Pass opp for klippeavtalerolle beskjede." 
                    } 
                },
                new () { 
                    Id = 5023, 
                    Names = new List<string> 
                    { 
                        "Meddela filmstart.", 
                        "Tell filming start message.", 
                        "Fortæl filmbeskedens start.", 
                        "Sagen Sie dem filming Startmeldung.", 
                        "Decir mensaje de inicio de filmación.", 
                        "Dites message de début de tournage.", 
                        "Dite messaggio di inizio delle riprese.", 
                        "Fortell filming start melding." 
                    } 
                },
                new () { 
                    Id = 5024, 
                    Names = new List<string> 
                    { 
                        "Fotograferingsstartdatum skickat meddelande.", 
                        "Photography startdate sent message.", 
                        "Fotograferingsstartdato sendt meddelelse.", 
                        "Fotografie startdatum gesendet Nachricht.", 
                        "Fecha de inicio de la fotografía enviada mensaje.", 
                        "Date de début de la photographie envoyé message.", 
                        "Data di inizio della fotografia inviata messaggio.", 
                        "Fotostartdato sendt melding." 
                    } 
                },
                new () { 
                    Id = 5025, 
                    Names = new List<string> 
                    { 
                        "Skicka uppdaterad teamlista meddelande.", 
                        "Sent updated team list message.", 
                        "Sendt opdateret holdliste besked.", 
                        "Gesendete aktualisierte Teamliste Nachricht.", 
                        "Mensaje de lista de equipo actualizado enviado.", 
                        "Message de liste d'équipe mis à jour envoyé.", 
                        "Messaggio dell'elenco del team aggiornato inviato.", 
                        "Sendt oppdatert teamliste." 
                    } 
                },
                new () { 
                    Id = 5026, 
                    Names = new List<string> 
                    { 
                        "Fotograferingsstartdatum skickat lönemeddelande.", 
                        "Photography startdate sent pay rate message.", 
                        "Fotograferingsstartdato sendt lønbesked.", 
                        "Fotografie Startdatum gesendet Gehalt Nachricht.", 
                        "Fecha de inicio de la fotografía enviada mensaje de tarifa de pago.", 
                        "Date de début de la photographie envoyé taux de rémunération message.", 
                        "Data di inizio della fotografia inviata messaggio della tariffa di pagamento.", 
                        "Fotostartdato sendt lønnsbeskjede." 
                    } 
                },
                new () { 
                    Id = 5027, 
                    Names = new List<string> 
                    { 
                        "Milsten 1 betalad meddelande.", 
                        "Milestone 1 payed message.", 
                        "Milepæl 1 betalt besked.", 
                        "Meilenstein 1 bezahlt Nachricht.", 
                        "Mensaje de hito 1 pagado.", 
                        "Message de jalon 1 payé.", 
                        "Messaggio di pagamento della pietra miliare 1.", 
                        "Milepæl 1 betalt melding." 
                    } 
                },
                new () { 
                    Id = 5028, 
                    Names = new List<string> 
                    { 
                        "Meddela filmavslutning.", 
                        "Tell filming end message.", 
                        "Fortæl filmbeskedent slut.", 
                        "Sagen Sie dem Ende der Drehmeldung.", 
                        "Decir mensaje de finalización de filmación.", 
                        "Dites message de fin de tournage.", 
                        "Dite messaggio di fine riprese.", 
                        "Fortell filming slutt melding." 
                    } 
                },
                new () { 
                    Id = 5029, 
                    Names = new List<string> 
                    { 
                        "Fotograferings slutdatum skickat meddelande.", 
                        "Photography enddate sent message.", 
                        "Fotograferings slutdato sendt besked.", 
                        "Fotografie Enddatum gesendet Nachricht.", 
                        "Fecha de finalización de la fotografía enviada mensaje.", 
                        "Date de fin de la photographie envoyé message.", 
                        "Data di fine della fotografia inviata messaggio.", 
                        "Foto sluttdato sendt melding." 
                    } 
                },
                new () { 
                    Id = 5030, 
                    Names = new List<string> 
                    { 
                        "Fotograferings slutdatum skickat lönemeddelande.", 
                        "Photography enddate sent pay rate message.", 
                        "Fotograferings slutdato sendt lønsbesked.", 
                        "Fotografie Enddatum gesendet Gehalt Nachricht.", 
                        "Fecha de finalización de la fotografía enviada mensaje de tarifa de pago.", 
                        "Date de fin de la photographie envoyé taux de rémunération message.", 
                        "Data di fine della fotografia inviata messaggio della tariffa di pagamento.", 
                        "Foto sluttdato sendt lønnsbeskjede." 
                    } 
                },
                new () { 
                    Id = 5031, 
                    Names = new List<string> 
                    { 
                        "Milsten 2 betalad meddelande.", 
                        "Milestone 2 payed message.", 
                        "Milepæl 2 betalt besked.", 
                        "Meilenstein 2 bezahlt Nachricht.", 
                        "Mensaje de hito 2 pagado.", 
                        "Message de jalon 2 payé.", 
                        "Messaggio di pagamento della pietra miliare 2.", 
                        "Milepæl 2 betalt melding." 
                    } 
                },
                new () { 
                    Id = 5032, 
                    Names = new List<string> 
                    { 
                        "Skicka råklippsmeddelande.", 
                        "Send rough cut message.", 
                        "Send råklip besked.", 
                        "Schicken Sie Rough Cut Nachricht.", 
                        "Enviar mensaje de corte bruto.", 
                        "Envoyer un message de coupe grossière.", 
                        "Invia messaggio di taglio grezzo.", 
                        "Sendt røff klipp melding." 
                    } 
                },
                            new () { 
                Id = 5033, 
                Names = new List<string> 
                { 
                    "Håll koll på råklippsmeddelande.", 
                    "Keep watch for rough cut message.", 
                    "Hold øje med råklipsbesked.", 
                    "Beobachten Sie Rough Cut Nachricht.", 
                    "Vigile sobre el mensaje de corte en bruto.", 
                    "Surveiller le message de coupe grossière.", 
                    "Tenere d'occhio il messaggio di taglio grezzo.", 
                    "Hold øye med råklippsmeldingen." 
                } 
            },
            new () { 
                Id = 5034, 
                Names = new List<string> 
                { 
                    "Följ upp råklippsmeddelande.", 
                    "Follow rough cut message.", 
                    "Følg råklipsbesked.", 
                    "Folgen Sie Rough Cut Nachricht.", 
                    "Siga el mensaje de corte en bruto.", 
                    "Suivez le message de coupe grossière.", 
                    "Seguire il messaggio di taglio grezzo.", 
                    "Følg råklippsmeldingen." 
                } 
            },
            new () { 
                Id = 5035, 
                Names = new List<string> 
                { 
                    "DCP-kopiameddelandet skickades.", 
                    "DCP copy message sent message.", 
                    "DCP kopi besked sendt.", 
                    "DCP-Kopie Nachricht gesendet.", 
                    "Mensaje de copia de DCP enviado.", 
                    "Message de copie DCP envoyé.", 
                    "Messaggio della copia DCP inviato.", 
                    "DCP kopi melding sendt."
                } 
            },
            new () { 
                Id = 5036, 
                Names = new List<string> 
                { 
                    "Skicka DCP-kopiameddelande.", 
                    "Send DCP copy message.", 
                    "Send DCP kopi besked.", 
                    "DCP-Kopie Nachricht senden.", 
                    "Enviar mensaje de copia de DCP.", 
                    "Envoyer le message de copie DCP.", 
                    "Inviare un messaggio di copia DCP.", 
                    "Send DCP kopi melding." 
                } 
            },
            new () { 
                Id = 5037, 
                Names = new List<string> 
                { 
                    "Slutlig klippning godkänd meddelande.", 
                    "Final cut approved message.", 
                    "Endelig klipning godkendt besked.", 
                    "Letzter Schnitt Nachricht genehmigt.", 
                    "Mensaje de corte final aprobado.", 
                    "Message de coupe finale approuvé.", 
                    "Messaggio di approvazione finale del taglio.", 
                    "Endelig klipp godkjent melding." 
                } 
            },
            new () { 
                Id = 5038, 
                Names = new List<string> 
                { 
                    "Skicka utgiftsrapport meddelande.", 
                    "Send spend report message.", 
                    "Send udgiftsrapport besked.", 
                    "Ausgabenbericht Nachricht senden.", 
                    "Enviar mensaje de informe de gastos.", 
                    "Envoyer un message de rapport de dépenses.", 
                    "Inviare un messaggio del rapporto di spesa.", 
                    "Send utgiftsrapport melding." 
                } 
            },
            new () { 
                Id = 5039, 
                Names = new List<string> 
                { 
                    "Utgiftsrapport skickad meddelande.", 
                    "Spend report sent message.", 
                    "Udgiftsrapport sendt besked.", 
                    "Ausgabenbericht gesendet Nachricht.", 
                    "Informe de gastos enviado mensaje.", 
                    "Message de rapport de dépenses envoyé.", 
                    "Messaggio di spesa inviato rapporto.", 
                    "Utgiftsrapport sendt melding." 
                } 
            },
            new () { 
                Id = 5040, 
                Names = new List<string> 
                { 
                    "Skicka premiärdatummeddelande.", 
                    "Send premiere date message.", 
                    "Send premieredato besked.", 
                    "Premierenachricht senden.", 
                    "Enviar mensaje de fecha de estreno.", 
                    "Envoyer un message de date de première.", 
                    "Inviare il messaggio della data di prima visione.", 
                    "Send premieredato melding." 
                } 
            },
            new () { 
                Id = 5041, 
                Names = new List<string> 
                { 
                    "Registrera utgiftsrapport meddelande.", 
                    "Register spend report message.", 
                    "Registrer udgiftsrapport besked.", 
                    "Ausgabenbericht Nachricht registrieren.", 
                    "Registrar mensaje de informe de gastos.", 
                    "Enregistrer un message de rapport de dépenses.", 
                    "Registrare un messaggio del rapporto di spesa.", 
                    "Registrer utgiftsrapport melding." 
                } 
            },
            new () { 
                Id = 5042, 
                Names = new List<string> 
                { 
                    "Premiärdatum skickat meddelande.", 
                    "Premiere date sent message.", 
                    "Premieredato sendt besked.", 
                    "Premierenachricht gesendet.", 
                    "Mensaje de fecha de estreno enviado.", 
                    "Message de date de première envoyé.", 
                    "Messaggio di data della prima visione inviato.", 
                    "Premiere dato sendt melding." 
                } 
            },
            new () { 
                Id = 5043, 
                Names = new List<string> 
                { 
                    "Premiärdatum skickat PR-meddelande.", 
                    "Premiere date sent PR message.", 
                    "Premieredato sendt PR besked.", 
                    "Premiere PR-Nachricht gesendet.", 
                    "Mensaje de RP de fecha de estreno enviado.", 
                    "Message PR de date de première envoyé.", 
                    "Messaggio PR della data di prima visione inviato.", 
                    "Premiere dato sendt PR melding." 
                } 
            },
            new () { 
                Id = 5044, 
                Names = new List<string> 
                { 
                    "Godkända utgifter, krediter och DCP-kopia, lönemeddelande.", 
                    "Approved spend, credits and DCP copy, pay rate message.", 
                    "Godkendte udgifter, kreditter og DCP kopi, lønbesked.", 
                    "Genehmigte Ausgaben, Gutschriften und DCP-Kopie, Gehaltsnachricht.", 
                    "Gastos aprobados, créditos y copia de DCP, mensaje de tarifa de pago.", 
                    "Dépenses approuvées, crédits et copie DCP, message de taux de rémunération.", 
                    "Spese approvate, crediti e copia di DCP, messaggio di tariffa.", 
                    "Godkjente utgifter, kreditter og DCP kopi, lønnsbeskjede." 
                } 
            },
            new () { 
                Id = 5045, 
                Names = new List<string> 
                { 
                    "Öva PR och planeringsmeddelande.", 
                    "Go over PR and planning message.", 
                    "Praksis PR og planlægning besked.", 
                    "PR und Planungsnachricht durchgehen.", 
                    "Practicar mensaje de planificación y relaciones públicas.", 
                    "Pratiquer le message de planification et de RP.", 
                    "Praticare il messaggio di pianificazione e PR.", 
                    "Øve på PR og planleggingsmelding." 
                } 
            },
            new () { 
                Id = 5046, 
                Names = new List<string> 
                { 
                    "Projektet redo för slutrapporten meddelande.", 
                    "Project ready for final report message.", 
                    "Projekt klar til slutrapport besked.", 
                    "Projekt bereit für den endgültigen Bericht.", 
                    "Proyecto listo para el informe final.", 
                    "Projet prêt pour le rapport final.", 
                    "Progetto pronto per il rapporto finale.", 
                    "Prosjekt klar for sluttrapporteringsmelding." 
                } 
            },
            new () { 
                Id = 5047, 
                Names = new List<string> 
                { 
                    "Be om DVD-meddelande.", 
                    "Ask for DVD message.", 
                    "Bed om DVD besked.", 
                    "DVD-Nachricht anfordern.", 
                    "Pedir mensaje de DVD.", 
                    "Demander un message de DVD.", 
                    "Chiedere un messaggio del DVD.", 
                    "Be om DVD-melding." 
                } 
            },
            new () { 
                Id = 5048, 
                Names = new List<string> 
                { 
                    "Registrera DVD-meddelande.", 
                    "Register DVD message.", 
                    "Registrer DVD besked.", 
                    "DVD-Nachricht registrieren.", 
                    "Registrar mensaje de DVD.", 
                    "Enregistrer un message de DVD.", 
                    "Registrare un messaggio del DVD.", 
                    "Registrer DVD-melding." 
                } 
            },
            new () { 
                Id = 5049, 
                Names = new List<string> 
                { 
                    "Registrera slutrapporten meddelande.", 
                    "Register end report message.", 
                    "Registrer slutrapport besked.", 
                    "Endbericht Nachricht registrieren.", 
                    "Registrar mensaje de informe final.", 
                    "Enregistrer un message de rapport de fin.", 
                    "Registrare un messaggio del rapporto finale.", 
                    "Registrer sluttrapporteringsmelding." 
                } 
            },
            new () { 
                Id = 5050, 
                Names = new List<string> 
                { 
                    "Nytt ansökningsmeddelande.", 
                    "New application message.", 
                    "Ny ansøgning besked.", 
                    "Neue Bewerbungsnachricht.", 
                    "Nuevo mensaje de solicitud.", 
                    "Nouveau message de candidature.", 
                    "Nuovo messaggio di richiesta.", 
                    "Ny søknads melding." 
                } 
            },
            new () { 
                Id = 5051, 
                Names = new List<string> 
                { 
                    "Lokalisering inte giltig längre meddelande.", 
                    "Loc not valid anymore message.", 
                    "Lok ikke længere gyldig besked.", 
                    "Ort nicht mehr gültig Nachricht.", 
                    "Ubicación ya no válida mensaje.", 
                    "Emplacement non valide message.", 
                    "Posizione non più valida messaggio.", 
                    "Plass ikke lenger gyldig melding." 
                } 
            },
            new () { 
                Id = 5052, 
                Names = new List<string> 
                { 
                    "Kontrollera om allt material är mottaget meddelande.", 
                    "Check if all material is received message.", 
                    "Kontroller, om alt materiale er modtaget.", 
                    "Überprüfen Sie, ob alle Materialien erhalten wurden.", 
                    "Verifique si se ha recibido todo el material.", 
                    "Vérifiez si tout le matériel a été reçu.", 
                    "Verificare se tutto il materiale è stato ricevuto.", 
                    "Kontroller om alt materiale er mottatt melding." 
                } 
            },
            new () { 
                Id = 5053, 
                Names = new List<string> 
                { 
                    "Väntar på producent meddelande", 
                    "Waiting for producer message.", 
                    "Afventer producent besked.", 
                    "Auf Produzent Nachricht warten.", 
                    "Esperando mensaje del productor.", 
                    "En attente de message du producteur.", 
                    "In attesa di un messaggio dal produttore.", 
                    "Venter på produsent melding." 
                } 
            },
            new () { 
                Id = 5054, 
                Names = new List<string> 
                { 
                    "Behöver Public 360 id meddelande.", 
                    "Need Public 360 id message.", 
                    "Brug for Public 360 id besked.", 
                    "Benötigen Public 360 ID Nachricht.", 
                    "Necesita mensaje de id público 360.", 
                    "Besoin de message d'identification publique 360.", 
                    "Necessario messaggio di ID pubblico 360.", 
                    "Trenger Public 360 ID melding." 
                } 
            },
            new () { 
                Id = 5055, 
                Names = new List<string> 
                { 
                    "Arkivera applikationsmeddelande.", 
                    "Archive app message.", 
                    "Arkiv app besked.", 
                    "Anwendungsnachricht archivieren.", 
                    "Mensaje de aplicación de archivo.", 
                    "Archiver le message d'application.", 
                    "Messaggio dell'applicazione di archivio.", 
                    "Arkivere app melding." 
                } 
            },
                            new () { 
                Id = 5056, 
                Names = new List<string> 
                { 
                    "Slutrapport godkänd meddelande.", 
                    "Final report approved message.", 
                    "Slutrapport godkendt besked.", 
                    "Endbericht genehmigt Nachricht.", 
                    "Informe final aprobado mensaje.", 
                    "Rapport final approuvé message.", 
                    "Rapporto finale approvato messaggio.", 
                    "Sluttrapport godkjent melding." 
                } 
            },
            new () { 
                Id = 5057, 
                Names = new List<string> 
                { 
                    "Fjärde ratten betald meddelande.", 
                    "Fourth rat payed message.", 
                    "Fjerde rate betalt besked.", 
                    "Vierte Rate bezahlt Nachricht.", 
                    "Cuarta rata pagada mensaje.", 
                    "Quatrième mensualité payée message.", 
                    "Quarta rata pagata messaggio.", 
                    "Fjerde avdrag betalt melding." 
                } 
            },
            new () { 
                Id = 5058, 
                Names = new List<string> 
                { 
                    "Projektet avslutat meddelande.", 
                    "Project finished message.", 
                    "Projekt færdig besked.", 
                    "Projekt abgeschlossen Nachricht.", 
                    "Proyecto terminado mensaje.", 
                    "Projet terminé message.", 
                    "Progetto finito messaggio.", 
                    "Prosjekt ferdig melding." 
                } 
            },
            new () { 
                Id = 5059, 
                Names = new List<string> 
                { 
                    "Slutför Public 360.", 
                    "Complete Public 360.", 
                    "Afslut Public 360.", 
                    "Vervollständigen Sie Public 360.", 
                    "Completar Public 360.", 
                    "Compléter Public 360.", 
                    "Completare Public 360.", 
                    "Fullfør Public 360." 
                } 
            },
            new () { 
                Id = 5060, 
                Names = new List<string> 
                { 
                    "Skicka faktura meddelande.", 
                    "Send invoice message.", 
                    "Send faktura besked.", 
                    "Rechnung Nachricht senden.", 
                    "Enviar mensaje de factura.", 
                    "Envoyer un message de facture.", 
                    "Inviare un messaggio di fattura.", 
                    "Send faktura melding." 
                } 
            },
            new () { 
                Id = 5061, 
                Names = new List<string> 
                { 
                    "Väntar på utgiftsrapport meddelande.", 
                    "Await spend report message.", 
                    "Afventer udgiftsrapport besked.", 
                    "Ausgabenbericht Nachricht erwarten.", 
                    "Esperar mensaje de informe de gastos.", 
                    "En attente de message de rapport de dépenses.", 
                    "In attesa del messaggio del rapporto di spesa.", 
                    "Venter på utgiftsrapport melding." 
                } 
            },
            new () { 
                Id = 5062, 
                Names = new List<string> 
                { 
                    "Väntar på premiärdatum meddelande.", 
                    "Await premier dates message.", 
                    "Afventer premierdatoer besked.", 
                    "Premiere Daten Nachricht erwarten.", 
                    "Esperar mensaje de fechas de estreno.", 
                    "En attente de message de dates de première.", 
                    "In attesa del messaggio delle date di prima visione.", 
                    "Venter på premierdatoer melding." 
                } 
            },
            new () { 
                Id = 5063, 
                Names = new List<string> 
                { 
                    "Kontrollera slutrapport meddelande.", 
                    "Check end report message.", 
                    "Kontrol slutrapport besked.", 
                    "Endbericht Nachricht überprüfen.", 
                    "Verificar mensaje de informe final.", 
                    "Vérifier le message de rapport de fin.", 
                    "Verificare il messaggio del rapporto finale.", 
                    "Kontroller sluttrapport melding." 
                } 
            },
            new () { 
                Id = 5064, 
                Names = new List<string> 
                { 
                    "Namn ändrat meddelande.", 
                    "Name changed message.", 
                    "Navn ændret besked.", 
                    "Name geändert Nachricht.", 
                    "Nombre cambiado mensaje.", 
                    "Nom changé message.", 
                    "Nome cambiato messaggio.", 
                    "Navn endret melding." 
                } 
            },
            new () { 
                Id = 5065, 
                Names = new List<string> 
                { 
                    "Boka visionsmöte meddelande.", 
                    "Book vision meeting message.", 
                    "Book vision møde besked.", 
                    "Visionsmeeting Nachricht buchen.", 
                    "Reservar mensaje de reunión de visión.", 
                    "Réserver un message de réunion de vision.", 
                    "Prenotare il messaggio della riunione di visione.", 
                    "Bestill visjonsmøte melding." 
                } 
            },
            new () { 
                Id = 5066, 
                Names = new List<string> 
                { 
                    "Projekt delegerat meddelande.", 
                    "Project delegated message.", 
                    "Projekt delegeret besked.", 
                    "Projekt delegiert Nachricht.", 
                    "Proyecto delegado mensaje.", 
                    "Projet délégué message.", 
                    "Progetto delegato messaggio.", 
                    "Prosjekt delegert melding." 
                } 
            },
            new () { 
                Id = 5067, 
                Names = new List<string> 
                { 
                    "Skriv beslutsdokument meddelande.", 
                    "Write decision doc message.", 
                    "Skriv beslutningsdokument besked.", 
                    "Entscheidung Dokument Nachricht schreiben.", 
                    "Escribir mensaje de documento de decisión.", 
                    "Écrire un message de document de décision.", 
                    "Scrivere un messaggio di documento di decisione.", 
                    "Skriv vedtaksdokument melding." 
                } 
            },
            new () { 
                Id = 5068, 
                Names = new List<string> 
                { 
                    "Arkivera beslutsdokument meddelande.", 
                    "Archive decision doc message.", 
                    "Arkiver beslutningsdokument besked.", 
                    "Entscheidung dokument Nachricht archivieren.", 
                    "Archivar mensaje de documento de decisión.", 
                    "Archiver un message de document de décision.", 
                    "Archiviare un messaggio di documento di decisione.", 
                    "Arkiver vedtaksdokument melding." 
                } 
            },
            new () { 
                Id = 5069, 
                Names = new List<string> 
                { 
                    "Visionsmöte bokat meddelande.", 
                    "Vision meeting booked message.", 
                    "Vision møde booket besked.", 
                    "Vision Meeting gebucht Nachricht.", 
                    "Mensaje de reunión de visión reservado.", 
                    "Message de réunion de vision réservé.", 
                    "Messaggio di riunione di visione prenotato.", 
                    "Visjonsmøte booket melding." 
                } 
            },
            new () { 
                Id = 5070, 
                Names = new List<string> 
                { 
                    "DCP godkänt meddelande.", 
                    "DCP approved message.", 
                    "DCP godkendt besked.", 
                    "DCP genehmigt Nachricht.", 
                    "Mensaje aprobado por DCP.", 
                    "Message approuvé par le DCP.", 
                    "Messaggio approvato da DCP.", 
                    "DCP godkjent melding." 
                } 
            },
            new () { 
                Id = 5071, 
                Names = new List<string> 
                { 
                    "Betala råtta meddelande.", 
                    "Pay rat message.", 
                    "Betale rotte besked.", 
                    "Ratte Nachricht bezahlen.", 
                    "Mensaje de pagar rata.", 
                    "Message de payer le rat.", 
                    "Messaggio di pagamento ratto.", 
                    "Betale rotte melding." 
                } 
            },
            new () { 
                Id = 5072, 
                Names = new List<string> 
                { 
                    "Detalj PR-material meddelande.", 
                    "Detail PR material message.", 
                    "Detaljer PR materiale besked.", 
                    "Detail PR Material Nachricht.", 
                    "Mensaje de material de RP detallado.", 
                    "Message de matériel de RP en détail.", 
                    "Messaggio di materiale PR dettagliato.", 
                    "Detalj PR-material melding." 
                } 
            },
            new () { 
                Id = 5073, 
                Names = new List<string> 
                { 
                    "PR-material levererat meddelande.", 
                    "PR material delivered message.", 
                    "PR materiale leveret besked.", 
                    "PR Material geliefert Nachricht.", 
                    "Mensaje de material de RP entregado.", 
                    "Message de matériel de RP livré.", 
                    "Messaggio di materiale PR consegnato.", 
                    "PR-material levert melding." 
                } 
            },
            new () { 
                Id = 5074, 
                Names = new List<string> 
                { 
                    "Greenlight spam meddelande.", 
                    "Greenlight spam message.", 
                    "Greenlight spam besked.", 
                    "Greenlight Spam Nachricht.", 
                    "Mensaje de spam de Greenlight.", 
                    "Message de spam Greenlight.", 
                    "Messaggio di spam di Greenlight.", 
                    "Greenlight spam melding."  
                } 
            },
            new () { 
                Id = 5075, 
                Names = new List<string> 
                { 
                    "Skriv under beslut.", 
                    "Sign decision.", 
                    "Underskriv beslutning.", 
                    "Entscheidung unterzeichnen.", 
                    "Firmar decisión.", 
                    "Signer la décision.", 
                    "Firmare la decisione.", 
                    "Signere beslutning." 
                } 
            },
            new () { 
                Id = 5076, 
                Names = new List<string> 
                { 
                    "Arkivera beslut.", 
                    "Archive decision.", 
                    "Arkiver beslutning.", 
                    "Entscheidung archivieren.", 
                    "Archivar decisión.", 
                    "Archiver la décision.", 
                    "Archiviare la decisione.", 
                    "Arkivere beslutning." 
                } 
            },
                            new () { 
                Id = 5077, 
                Names = new List<string> 
                { 
                    "Avtal undertecknat lönebesked.", 
                    "Agreement signed pay rate message.", 
                    "Aftale underskrevet lønbesked.", 
                    "Vereinbarung unterzeichnet Lohnnachricht.", 
                    "Mensaje de tarifa de pago firmado.", 
                    "Message de taux de rémunération signé.", 
                    "Messaggio di tariffa di pagamento firmato.", 
                    "Avtale signert lønnsbeskjed." 
                } 
            },
            new () { 
                Id = 5078, 
                Names = new List<string> 
                { 
                    "Skicka arbetskopia meddelande.", 
                    "Send work copy message.", 
                    "Send arbejds kopi besked.", 
                    "Arbeitskopie Nachricht senden.", 
                    "Enviar mensaje de copia de trabajo.", 
                    "Envoyer un message de copie de travail.", 
                    "Inviare un messaggio di copia di lavoro.", 
                    "Send arbeids kopi melding." 
                } 
            },
            new () { 
                Id = 5079, 
                Names = new List<string> 
                { 
                    "Arkivera utgifts- och slutrapport meddelande.", 
                    "Archive spend and final report message.", 
                    "Arkiver brug og slutrapport besked.", 
                    "Ausgaben- und Abschlussbericht Nachricht archivieren.", 
                    "Archivar mensaje de gastos e informe final.", 
                    "Archiver un message de dépenses et rapport final.", 
                    "Archiviare un messaggio di spese e rapporto finale.", 
                    "Arkiver forbruk og sluttrapporteringsmelding." 
                } 
            },
            new () { 
                Id = 5080, 
                Names = new List<string> 
                { 
                    "Utgifter och slutrapport godkänd lönebesked.", 
                    "Spend and final report approved pay rate message.", 
                    "Brug og slutrapport godkendt lønbesked.", 
                    "Ausgaben- und Abschlussbericht genehmigt Lohnnachricht.", 
                    "Gastos e informe final aprobado mensaje de tarifa de pago.", 
                    "Dépenses et rapport final approuvé message de taux de rémunération.", 
                    "Spese e rapporto finale approvato messaggio di tariffa di pagamento.", 
                    "Forbruk og sluttrapport godkjent lønnsbeskjed." 
                } 
            },
            new () { 
                Id = 5081, 
                Names = new List<string> 
                { 
                    "Tredje ratten betald meddelande.", 
                    "Third rat payed message.", 
                    "Tredje rate betalt besked.", 
                    "Dritte Rate bezahlt Nachricht.", 
                    "Mensaje de tercer pago de rata.", 
                    "Message de troisième mensualité payée.", 
                    "Messaggio di terza rata pagata.", 
                    "Tredje avdrag betalt melding." 
                } 
            },
            new () { 
                Id = 5082, 
                Names = new List<string> 
                { 
                    "Skicka premiärdatummeddelande - kortfilm.", 
                    "Send premiere date message - short film.", 
                    "Send premieredato besked - kortfilm.", 
                    "Premiere Datum Nachricht senden - Kurzfilm.", 
                    "Enviar mensaje de fecha de estreno - cortometraje.", 
                    "Envoyer un message de date de première - court métrage.", 
                    "Inviare il messaggio di data di prima visione - cortometraggio.", 
                    "Send premierdato melding - kortfilm." 
                } 
            },
            new () { 
                Id = 5083, 
                Names = new List<string> 
                { 
                    "Väntar på premiärdatum meddelande - kortfilm.", 
                    "Await premier dates message - short film.", 
                    "Afventer premieredatoer besked - kortfilm.", 
                    "Premiere Daten Nachricht erwarten - Kurzfilm.", 
                    "Esperar mensaje de fechas de estreno - cortometraje.", 
                    "En attente de message de dates de première - court métrage.", 
                    "In attesa del messaggio delle date di prima visione - cortometraggio.", 
                    "Venter på premierdatoer melding - kortfilm." 
                } 
            },
            new () { 
                Id = 5084, 
                Names = new List<string> 
                { 
                    "Projekt avslutat utan produktion meddelande.", 
                    "Project closed without production message.", 
                    "Projekt afsluttet uden produktion besked.", 
                    "Projekt ohne Produktion abgeschlossen Nachricht.", 
                    "Proyecto cerrado sin producción mensaje.", 
                    "Projet fermé sans production message.", 
                    "Progetto chiuso senza produzione messaggio.", 
                    "Prosjekt avsluttet uten produksjon melding." 
                } 
            },
            new () { 
                Id = 5085, 
                Names = new List<string> 
                { 
                    "Ladda upp filminformation meddelande.", 
                    "Upload film info message.", 
                    "Upload film information besked.", 
                    "Filminfo Nachricht hochladen.", 
                    "Subir mensaje de información de película.", 
                    "Télécharger le message d'informations sur le film.", 
                    "Caricare il messaggio di informazioni sul film.", 
                    "Last opp filminformasjon melding." 
                } 
            },
            new () { 
                Id = 5086, 
                Names = new List<string> 
                { 
                    "Ladda upp film meddelande.", 
                    "Upload film message.", 
                    "Upload film besked.", 
                    "Film Nachricht hochladen.", 
                    "Subir mensaje de película.", 
                    "Télécharger le message du film.", 
                    "Caricare il messaggio del film.", 
                    "Last opp film melding." 
                } 
            },
            new () { 
                Id = 5087, 
                Names = new List<string> 
                { 
                    "Skicka ekonomisk och konstnärlig redovisning meddelande.", 
                    "Send economical and artistic accounting message.", 
                    "Send økonomisk og kunstnerisk regnskab besked.", 
                    "Wirtschaftliche und künstlerische Buchhaltung Nachricht senden.", 
                    "Enviar mensaje de contabilidad económica y artística.", 
                    "Envoyer un message de comptabilité économique et artistique.", 
                    "Inviare un messaggio di contabilità economica e artistica.", 
                    "Send økonomisk og kunstnerisk regnskapsmelding." 
                } 
            },
            new () { 
                Id = 5088, 
                Names = new List<string> 
                { 
                    "Ekonomisk och konstnärlig redovisningsdatum passerat producent meddelande.", 
                    "Economical and artistic accounting date passed producer message.", 
                    "Økonomisk og kunstnerisk regnskab dato passeret producent besked.", 
                    "Wirtschaftliche und künstlerische Buchhaltung Datum verpasst Produzent Nachricht.", 
                    "Fecha de contabilidad económica y artística pasada mensaje del productor.", 
                    "Date de comptabilité économique et artistique dépassée message du producteur.", 
                    "Data di contabilità economica e artistica passata messaggio del produttore.", 
                    "Økonomisk og kunstnerisk regnskapsdato passert produsent melding." 
                } 
            },
            new () { 
                Id = 5089, 
                Names = new List<string> 
                { 
                    "Ekonomisk och konstnärlig redovisningsdatum passerat AA meddelande.", 
                    "Economical and artistic accounting date passed AA message.", 
                    "Økonomisk og kunstnerisk regnskab dato passeret AA besked.", 
                    "Wirtschaftliche und künstlerische Buchhaltung Datum verpasst AA Nachricht.", 
                    "Fecha de contabilidad económica y artística pasada mensaje de AA.", 
                    "Date de comptabilité économique et artistique dépassée message AA.", 
                    "Data di contabilità economica e artistica passata messaggio di AA.", 
                    "Økonomisk og kunstnerisk regnskapsdato passert AA melding." 
                } 
            },
            new () { 
                Id = 5090, 
                Names = new List<string> 
                { 
                    "Ekonomisk och konstnärlig redovisningsdatum passerat koordinator meddelande.", 
                    "Economical and artistic accounting date passed coordinator message.", 
                    "Økonomisk og kunstnerisk regnskab dato passeret koordinator besked.", 
                    "Wirtschaftliche und künstlerische Buchhaltung Datum verpasst Koordinator Nachricht.", 
                    "Fecha de contabilidad económica y artística pasada mensaje del coordinador.", 
                    "Date de comptabilité économique et artistique dépassée message du coordinateur.", 
                    "Data di contabilità economica e artistica passata messaggio del coordinatore.", 
                    "Økonomisk og kunstnerisk regnskapsdato passert koordinator melding." 
                } 
            },
            new () { 
                Id = 9090, 
                Names = new List<string> 
                { 
                    "Motkrav godkänt.", 
                    "Counterclaim approved.", 
                    "Modkrav godkendt.", 
                    "Gegenforderung genehmigt.", 
                    "Reconocimiento aprobado.", 
                    "Réclamation approuvée.", 
                    "Reclamo approvato.", 
                    "Mot krav godkjent." 
                } 
            }
            );

            builder.Entity<MilestoneRequirementType>()
                .HasData(
                new () {
                    Id = 1, 
                    Names = new List<string> 
                    { 
                        "Inspelningsstart rapporterad till FiV",
                        "Start of Filming Reported to FiV",
                        "Indspilningsstart rapporteret til FiV", 
                        "Drehstart an FiV gemeldet", 
                        "Inicio de filmación reportado a FiV", 
                        "Début du tournage signalé à FiV", 
                        "Inizio delle riprese segnalato a FiV", 
                        "Start av innspilling rapportert til FiV"
                    }
                },
                new () {
                    Id = 2, 
                    Names = new List<string> 
                    { 
                        "FiV har mottagit samtliga bilagor och samproduktionsavtal i original för projektet",
                        "FiV has received all attachments and co-production agreement in original for the project",
                        "FiV har modtaget alle bilag og samproduktionsaftale i original for projektet", 
                        "FiV hat alle Anlagen und Koproduktionsvereinbarung im Original für das Projekt erhalten", 
                        "FiV ha recibido todos los anexos y el acuerdo de coproducción original para el proyecto", 
                        "FiV a reçu tous les pièces jointes et l'accord de coproduction original pour le projet", 
                        "FiV ha ricevuto tutti gli allegati e l'accordo di coproduzione originale per il progetto", 
                        "FiV har mottatt alle vedlegg og samproduksjonsavtale i original for prosjektet"
                    }
                },
                new () {
                    Id = 3, 
                    Names = new List<string> 
                    { 
                        "Samproduktionsavtal i original mottaget av FiV",
                        "Co-production Agreement in Original Received by FiV",
                        "Samproduktionsaftale i original modtaget af FiV", 
                        "Koproduktionsvereinbarung im Original von FiV erhalten", 
                        "Acuerdo de coproducción original recibido por FiV", 
                        "Accord de coproduction original reçu par FiV", 
                        "Accordo di coproduzione originale ricevuto da FiV", 
                        "Samproduksjonsavtale i original mottatt av FiV"
                    }
                },
                new () {
                    Id = 4, 
                    Names = new List<string> 
                    { 
                        "Inspelningsslut rapporterad till FiV",
                        "End of Filming Reported to FiV",
                        "Indspilningsslut rapporteret til FiV", 
                        "Drehende an FiV gemeldet", 
                        "Finalización de la filmación reportada a FiV", 
                        "Fin du tournage signalée à FiV", 
                        "Fine delle riprese segnalato a FiV", 
                        "Slutt på innspilling rapportert til FiV"
                    }
                },
                new () {
                    Id = 5, 
                    Names = new List<string> 
                    { 
                        "Godkänd DCP/A-kopia mottagen av FiV",
                        "Approved DCP/A-Copy Received by FiV",
                        "Godkendt DCP/A-kopi modtaget af FiV", 
                        "Genehmigte DCP/A-Kopie von FiV erhalten", 
                        "DCP/A-copia aprobada recibida por FiV", 
                        "DCP/A-copie approuvée reçue par FiV", 
                        "DCP/A-copia approvata ricevuta da FiV", 
                        "Godkjent DCP/A-kopi mottatt av FiV"
                    }
                },
                new () {
                    Id = 6, 
                    Names = new List<string> 
                    { 
                        "Av revisor intygad och godkänd redovisning av spend mottagen av FiV",
                        "Auditor Certified and Approved Spend Report Received by FiV",
                        "Revisor bekræftet og godkendt udgiftsrapport modtaget af FiV", 
                        "Vom Wirtschaftsprüfer bestätigter und genehmigter Ausgabenbericht von FiV erhalten", 
                        "Informe de gastos certificado y aprobado por auditor recibido por FiV", 
                        "Rapport de dépenses certifié et approuvé par l'auditeur reçu par FiV", 
                        "Rapporto di spesa certificato e approvato dall'auditor ricevuto da FiV", 
                        "Revisorverifisert og godkjent utgiftsrapport mottatt av FiV"
                    }
                },
                new () {
                    Id = 7, 
                    Names = new List<string> 
                    { 
                        "Reviderad ekonomisk slutredovisning mottagen av FiV",
                        "Revised Financial Final Report Received by FiV",
                        "Revideret økonomisk slutrapport modtaget af FiV", 
                        "Überarbeiteter Finanzabschlussbericht von FiV erhalten", 
                        "Informe financiero final revisado recibido por FiV", 
                        "Rapport financier final révisé reçu par FiV", 
                        "Rapporto finanziario finale rivisto ricevuto da FiV", 
                        "Revidert økonomisk sluttrapport mottatt av FiV"
                    }
                },
                new () {
                    Id = 8, 
                    Names = new List<string> 
                    { 
                        "Annat",
                        "Other",
                        "Andet", 
                        "Andere", 
                        "Otro", 
                        "Autres", 
                        "Altro", 
                        "Andre"
                    }
                },
                new () {
                    Id = 9, 
                    Names = new List<string> 
                    { 
                        "Faktura",
                        "Invoice",
                        "Faktura", 
                        "Rechnung", 
                        "Factura", 
                        "Facture", 
                        "Fattura", 
                        "Faktura"
                    }
                },
                new () {
                    Id = 14, 
                    Names = new List<string> 
                    { 
                        "Preliminär klipp av filmen (DVD)",
                        "Preliminary Cut of the Film (DVD)",
                        "Foreløbig klip af filmen (DVD)", 
                        "Vorläufiger Filmschnitt (DVD)", 
                        "Corte preliminar de la película (DVD)", 
                        "Montage préliminaire du film (DVD)", 
                        "Montaggio preliminare del film (DVD)", 
                        "Foreløpig klipp av filmen (DVD)"
                    }
                },
                new () {
                    Id = 17, 
                    Names = new List<string> 
                    { 
                        "Reviderad räkenskap tillsammans med budget",
                        "Revised Accounts with Budget",
                        "Revideret regnskab sammen med budget", 
                        "Überarbeitete Buchführung mit Budget", 
                        "Cuentas revisadas con presupuesto", 
                        "Comptes révisés avec budget", 
                        "Conti rivisti con budget", 
                        "Revidert regnskap sammen med budsjett"
                    }
                },
                new () {
                    Id = 18, 
                    Names = new List<string> 
                    { 
                        "Signerat kontrakt",
                        "Signed Contract",
                        "Signeret kontrakt", 
                        "Unterzeichneter Vertrag", 
                        "Contrato firmado", 
                        "Contrat signé", 
                        "Contratto firmato", 
                        "Signert kontrakt"
                    }
                },
                new () {
                    Id = 20, 
                    Names = new List<string> 
                    { 
                        "Skriven rapport inkluderat internationell försäljning, festivaldeltagande samt vilka priser som mottagits",
                        "Written Report Including International Sales, Festival Participation and Received Awards",
                        "Skriftlig rapport inklusive internationalt salg, festivaldeltagelse og modtagne priser", 
                        "Geschriebener Bericht einschließlich internationaler Verkäufe, Festivalteilnahme und erhaltene Auszeichnungen", 
                        "Informe escrito que incluye ventas internacionales, participación en festivales y premios recibidos", 
                        "Rapport écrit incluant les ventes internationales, la participation aux festivals et les prix reçus", 
                        "Relazione scritta incluse vendite internazionali, partecipazione ai festival e premi ricevuti", 
                        "Skriftlig rapport inkludert internasjonalt salg, festivaldeltakelse og mottatte priser"
                    }
                },
                new () {
                    Id = 34, 
                    Names = new List<string> 
                    { 
                        "Vid mottaget undertecknat avtal samt bilagor",
                        "Upon Receipt of Signed Agreement and Attachments",
                        "Ved modtagelse af underskrevet aftale og bilag", 
                        "Bei Erhalt eines unterzeichneten Vertrags und Anhängen", 
                        "Al recibir el acuerdo firmado y los anexos", 
                        "Lors de la réception de l'accord signé et des pièces jointes", 
                        "Al ricevimento del contratto firmato e degli allegati", 
                        "Ved mottak av signert avtale og vedlegg"
                    }
                },
                new () {
                    Id = 35, 
                    Names = new List<string> 
                    { 
                        "Vid leverans av godkänd arbetskopia",
                        "Upon Delivery of Approved Work Copy",
                        "Ved levering af godkendt arbejdskopi", 
                        "Bei Lieferung einer genehmigten Arbeitskopie", 
                        "Al entregar una copia de trabajo aprobada", 
                        "Lors de la livraison d'une copie de travail approuvée", 
                        "Alla consegna della copia di lavoro approvata", 
                        "Ved levering av godkjent arbeidskopi"
                    }
                },
                new () {
                    Id = 36, 
                    Names = new List<string> 
                    { 
                        "Vid leverans av Filmen samt av FiV godkänd spendredovisning samt godkänd slutredovisning",
                        "Upon Delivery of the Film and FiV-approved Spend Report and Final Report",
                        "Ved levering af filmen og FiV-godkendt udgifts- og slutrapport", 
                        "Bei Lieferung des Films und von FiV genehmigten Ausgaben- und Abschlussberichten", 
                        "Al entregar la película y el informe de gastos y el informe final aprobados por FiV", 
                        "Lors de la livraison du film et des rapports de dépenses et final approuvés par FiV", 
                        "Alla consegna del film e del rapporto di spesa e del rapporto finale approvati da FiV", 
                        "Ved levering av filmen og FiV-godkjent utgifts- og sluttrapport"
                    }
                },
                new () {
                    Id = 37, 
                    Names = new List<string> 
                    { 
                        "Avtalsprocessen klar",
                        "The Agreement Process is Completed",
                        "Aftaleprocessen er afsluttet", 
                        "Der Vertragsprozess ist abgeschlossen", 
                        "El proceso de acuerdo está completado", 
                        "Le processus d'accord est terminé", 
                        "Il processo di accordo è completato", 
                        "Avtaleprosessen er fullført"
                    }
                },
                new () {
                    Id = 38, 
                    Names = new List<string> 
                    { 
                        "Färdig projektutveckling",
                        "Completed Project Development",
                        "Afsluttet projektudvikling", 
                        "Abgeschlossene Projektentwicklung", 
                        "Desarrollo de proyecto completado", 
                        "Développement de projet terminé", 
                        "Sviluppo del progetto completato", 
                        "Avsluttet prosjektutvikling"
                    }
                }
            );
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        

    }
    
}
