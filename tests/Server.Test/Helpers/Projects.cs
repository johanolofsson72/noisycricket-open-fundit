using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Applications.Entities;
using Shared.Data.DbContext;
using Shared.Documents.Entities;
using Shared.Documents.Services;
using Shared.Global.DTOs;
using Shared.Projects.Entities;
using Shared.Schemas.Entities;
using Telerik.Blazor.Components;

namespace Server.Test.Helpers;

public static class Projects
{
    public static string NextProjectNumber(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var applications = context.Applications.ToList();

        if (!applications.Any()) return "000001";

        var item = context.Applications.Count() + 1;

        return $"{item:000000}";
    }
    
    public static async Task<(int, int)> CreateApplication(WebApplicationFactory<Program> factory, int organizationId, int applicant, int projectManager, int productionManager, int contractManager, Schema schema, string number, string title)
    {
        try
        {
            using var scope = factory.Services.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var project = new Project
            {
                StatusId = 2,
                Number = number,
                Title = [title]
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();
            
            var budget = new Bogus.Faker().Finance.Amount(1000000, 9000000);
            var applied = new Bogus.Faker().Finance.Amount(100000, 5000000);
            var our = new Bogus.Faker().Finance.Amount(100000, 5000000);
            var min = new Bogus.Faker().Finance.Amount(100000, 5000000);
            
            // EA09893C
            var country = new Bogus.Faker().Address.Country();
            
            // EBA1414F
            var producenter = new List<ListboxNameEmailPhonenumberGenderDto>()
            {
                new ListboxNameEmailPhonenumberGenderDto()
                {
                    Name = new Bogus.Person().FullName,
                    Email = new Bogus.Person().Email.ToLower(),
                    Phonenumber = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999),
                    Gender = new Bogus.Person().Gender.ToString().ToLower()
                },
                new ListboxNameEmailPhonenumberGenderDto()
                {
                    Name = new Bogus.Person().FullName,
                    Email = new Bogus.Person().Email.ToLower(),
                    Phonenumber = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999),
                    Gender = new Bogus.Person().Gender.ToString().ToLower()
                }
            };
            
            // 77C2DFBD
            var company = "Star Wars AB";
            
            // B41F016E
            var phone = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999);
            
            // B784472E
            var comments = new Bogus.Faker().Lorem.Text();
            
            // 4D5B21F8
            var directors = new List<ListboxNameEmailPhonenumberGenderDto>()
            {
                new ListboxNameEmailPhonenumberGenderDto()
                {
                    Name = new Bogus.Person().FullName,
                    Email = new Bogus.Person().Email.ToLower(),
                    Phonenumber = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999),
                    Gender = new Bogus.Person().Gender.ToString().ToLower()
                },
                new ListboxNameEmailPhonenumberGenderDto()
                {
                    Name = new Bogus.Person().FullName,
                    Email = new Bogus.Person().Email.ToLower(),
                    Phonenumber = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999),
                    Gender = new Bogus.Person().Gender.ToString().ToLower()
                }
            };
            
            // BF962372
            var writers = new List<ListboxNameEmailPhonenumberGenderDto>()
            {
                new ListboxNameEmailPhonenumberGenderDto()
                {
                    Name = new Bogus.Person().FullName,
                    Email = new Bogus.Person().Email.ToLower(),
                    Phonenumber = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999),
                    Gender = new Bogus.Person().Gender.ToString().ToLower()
                },
                new ListboxNameEmailPhonenumberGenderDto()
                {
                    Name = new Bogus.Person().FullName,
                    Email = new Bogus.Person().Email.ToLower(),
                    Phonenumber = new Bogus.Faker().Random.Int(1000, 9999) + "-" + new Bogus.Faker().Random.Int(100000, 999999),
                    Gender = new Bogus.Person().Gender.ToString().ToLower()
                }
            };
            
            // F88289B5
            var distributor = new Bogus.Person().Company.Name;
            
            // 00000001
            var currency = "SEK";
            
            // F52326F1
            var shortSynopsis = new Bogus.Faker().Lorem.Text();
            
            // AA2B6D99
            var standingproduction = new Bogus.Faker().Lorem.Text();
            
            // 11100010
            var prelength = "1:20";
            
            var application = new Application
            {
                ParentId = 0,
                ProjectId = project.Id,
                ProjectNumber = project.Number,
                StatusId = 2,
                SchemaId = schema.Id,
                SchemaNames = schema.Names,
                SchemaClaimTag = schema.ClaimTag,
                Title = title,
                Number = number,
                Organization = new ApplicationContact
                {
                    ContactIdentifier = organizationId,
                    Email = "jool@me.com",
                    Name = company
                },
                Applicant = new ApplicationContact
                {
                    ContactIdentifier = applicant,
                    Email = "jool@me.com",
                    Name = "Han Solo"
                },
                Producer = producenter.Select(x => new ApplicationContact
                {
                    ContactIdentifier = 0,
                    Email = x.Email,
                    Name = x.Name
                }).FirstOrDefault()!,
                ProjectManager = new(),
                ProductionManager = new(),
                ContractManager = new(),
                InternalBudgets = new List<ApplicationBudget>()
                {
                    new ApplicationBudget()
                    {
                        BudgetIdentifier = 1,
                        ApplicationBudgetTypeId = 13,
                        Amount = our,
                        ApprovedDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        Name = "Funded by us",
                        StatusId = 2
                    },
                    new ApplicationBudget()
                    {
                        BudgetIdentifier = 2,
                        ApplicationBudgetTypeId = 11,
                        Amount = min,
                        ApprovedDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        Name = "Internal funding",
                        StatusId = 2
                    }
                },
                Controls = new List<ApplicationControl>(),
                Events = schema.Events.ToApplicationEvent(),
                Progress = schema.Progress.ToApplicationProgress(),
                RequiredDocuments = schema.RequiredDocuments.ToApplicationRequiredDocument(),
                EarlierSupportTotalAmount = 0,
                InternalBudgetsTotalAmount = 0,
                MilestonePayoutTotalAmount = 0,
                InternalBudgetsApproved = true,
                BudgetAmount = budget,
                AppliedAmount = applied,
                OurContribution = our,
                UpdatedDate = DateTime.Now,
                CreatedDate = DateTime.Now
            };

            var index = 1;
            foreach (var x in schema.Controls)
            {
                
                application.Controls.Add(
                    new ApplicationControl()
                    {
                        Id = x.Id,
                        ApplicationControlIdentifier = x.SchemaControlIdentifier,
                        ControlId = x.ControlId,
                        ControlTypeId = x.ControlTypeId,
                        ControlTypeName = x.ControlTypeName,
                        ControlValueType = x.ControlValueType,
                        BaseStructure = x.BaseStructure,
                        Order = x.Order,
                        VisibleOnApplicationForm = x.VisibleOnApplicationForm,
                        ApplicationFormPage = x.ApplicationFormPage,
                        ApplicationFormOrder = x.ApplicationFormOrder,
                        ApplicationFormRequired = x.ApplicationFormRequired,
                        Css = x.Css,
                        Labels = x.Labels,
                        ApplicationSectionId = x.ApplicationSectionId,
                        ApplicationFormSectionId = x.ApplicationFormSectionId,
                        Value = string.Empty,
                        Placeholders = x.Placeholders,
                        DataSource = x.DataSource,
                        SubLabels = x.SubLabels,
                        MaxValueLength = x.MaxValueLength,
                        UniqueId = x.UniqueId
                        
                    }
                    );
                index++;
            }

            application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("00001001"))!.Value = title;
            application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("00010001"))!.Value = budget.ToString();
            application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("00000002"))!.Value = applied.ToString();
            application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("01000001"))!.Value = our.ToString();
            application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("00100001"))!.Value = (our / budget * 100).ToString();
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("EA09893C")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("EA09893C"))!.Value = country;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("B41F016E")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("B41F016E"))!.Value = phone;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("EBA1414F")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("EBA1414F"))!.Value = JsonSerializer.Serialize(producenter);
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("77C2DFBD")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("77C2DFBD"))!.Value = company;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("B784472E")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("B784472E"))!.Value = comments;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("4D5B21F8")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("4D5B21F8"))!.Value = JsonSerializer.Serialize(directors);
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("BF962372")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("BF962372"))!.Value = JsonSerializer.Serialize(writers);
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("F88289B5")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("F88289B5"))!.Value = distributor;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("00000001")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("00000001"))!.Value = currency;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("F52326F1")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("F52326F1"))!.Value = shortSynopsis;
            
            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("AA2B6D99")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("AA2B6D99"))!.Value = standingproduction;

            if (application.Controls.Any(x => x.UniqueId.ToString().ToUpper().StartsWith("11100010")))
                application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToUpper().StartsWith("11100010"))!.Value = prelength;

            
            context.Applications.Add(application);
            await context.SaveChangesAsync();
            
            application = await AddDocuments(factory, application, schema.Id);
            
            context.Applications.Update(application);
            await context.SaveChangesAsync();
            
            return (application.ProjectId, application.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task<Application> AddDocuments(WebApplicationFactory<Program> factory, Application application, int schemaId)
    {
        try
        {
            using var scope = factory.Services.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Delete old files in this directory recursively
            var path = Path.Combine("/Users", "jool", "repos", "noisycricket-fundit", "Documents", "app", application.Id.ToString(), "att");
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }
                foreach (var subDir in dir.GetDirectories())
                {
                    subDir.Delete(true);
                }
            }

            if (schemaId is 1 or 3 or 4 or 5)
            {
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 75, "Teamlista.txt", "2694FE36");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 14, "Manus.txt", "067C33C6");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 22, "Finansieringsplan.txt", "277EEF1C");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 12, "Projektbeskrivning.txt", "33EDD2B7");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 10, "Övrigt.txt", "1528348A");
            }
            else if (schemaId is 2)
            {
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 75, "Teamlista.txt", "2694FE36");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 14, "Manus.txt", "067C33C6");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 22, "Finansieringsplan.txt", "277EEF1C");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 21, "Budget.txt", "11100001");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 17, "Tidsplan.txt", "11100003");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 12, "Projektbeskrivning.txt", "33EDD2B7");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 111, "Distributionsplan.txt", "11100002");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 10, "Övrigt.txt", "1528348A");
            }
            else if (schemaId is 6 or 7 or 8)
            {
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 16, "Synopsis.txt", "11100004");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 12, "Projektbeskrivning.txt", "11100009");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 3, "Utvecklingsbeskrivning.txt", "11100005");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 21, "Utvecklingsbudget.txt", "11100006");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 78, "Spendbudget.txt", "11100007");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 17, "Tidsplan.txt", "11100003");
                application.Controls = await CreateAttachment(application.Controls, context, application.Id, dir.ToString(), 112, "CV-Nyckelfunktioner.txt", "11100008");
            }

            return application;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new Application();
        }
    }

    private static async Task<List<ApplicationControl>> CreateAttachment(List<ApplicationControl> controls, ApplicationDbContext context, int applicationId, string basePath, int typeId, string name, string code)
    {
        try
        {
            await Task.Delay(0);
            var requirementTypeId = typeId;
            var fileName = name;
            var fileId = CreateDocument(context, applicationId, requirementTypeId, fileName, "text/plain", "txt", basePath);
            basePath = Path.Combine(basePath, fileId.ToString());
            if (Directory.Exists(basePath) == false)
            {
                Directory.CreateDirectory(basePath);
            }
            var filePath = Path.Combine(basePath, fileName);
            var file = File.CreateText(filePath);
            file.Write(new Bogus.Faker().Lorem.Paragraphs(2));
            file.Close();
            var files = new List<UploadFileInfo>() { new() { Name = fileName, Extension = "txt" } };
            var json = JsonSerializer.Serialize(files);

            if (!controls.Any(control => control.UniqueId.ToString().ToLower().StartsWith(code.ToLower())))
            {
                throw new Exception($"Control {code.ToLower()} not found in application {applicationId}");
            }
            foreach (var control in controls.Where(control => control.UniqueId.ToString().ToLower().StartsWith(code.ToLower())))
            {
                control.Value = json;
                break;
            }

            return controls;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<ApplicationControl>();
        }
    }
    
    private static int CreateDocument(ApplicationDbContext context, int applicationId, int requirementTypeId, string fileName, string mimeType, string extension, string path)
    {
        try
        {
            var document = new Document
            {
                ApplicationId = applicationId,
                StatusId = 2,
                RequirementTypeId = requirementTypeId,
                DeliveryTypeId = 2,
                FileName = fileName,
                MimeType = mimeType,
                Extension = extension,
                Path = path,
                Phrases = "",
                Summarize = "",
                Binary = [],
                Metadata = [],
                IsDelivered = false,
                IsSigned = false,
                IsCertified = false,
                IsLocked = false,
                CreatedDate = DateTime.UtcNow
            };

            context.Documents.Add(document);
            context.SaveChanges();
        
            document = context.Documents
                .AsTracking()
                .FirstOrDefault(x => x.Id == document.Id);
        
            document!.Path = Path.Combine(path, document.Id.ToString(), fileName);
        
            context.SaveChanges();

            return document.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static void UpdateProject(WebApplicationFactory<Program> factory, int projectId, int applicationId, Schema schema, string titel, string number, int organizationId)
    {
        try
        {

            using var scope = factory.Services.CreateScope(); 
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var schemaType = new SummaryListDto() { Id = schema.Id, Names = schema.Names };
            
            var project = context.Projects
                .AsTracking()
                .FirstOrDefault(p => p.Id == projectId);
            
            var applications = new List<ProjectApplication>();
            var application = new ProjectApplication()
            {
                ApplicationIdentifier = applicationId,
                ApplicationStatusId = 2,
                ApplicationSchemaId = schemaType.Id,
                ApplicationSchemaNames = schemaType.Names,
                ApplicationTitle = titel,
                ApplicationApplicant = new ApplicationContact(){ ContactIdentifier = 1, Name = "Han Solo", Email = "jool@me.com"},
                ApplicationProjectManager = new ApplicationContact(){ ContactIdentifier = 1, Name = "Han Solo", Email = "jool@me.com"},
                ApplicationProductionManager = new ApplicationContact(){ ContactIdentifier = 1, Name = "Han Solo", Email = "jool@me.com"},
                ApplicationContractManager = new ApplicationContact(){ ContactIdentifier = 1, Name = "Han Solo", Email = "jool@me.com"},
                ApplicationCreatedDate = DateTime.Now
            };
            applications.Add(application);

            if (project != null)
            {
                project.Applications = applications;
                project.StatusId = 2;
                project.Number = number;
                project.Title = new List<string>(){ titel };
                project.ApplicationCount = 1;
                project.Organization = new ProjectOrganization
                {
                    OrganizationIdentifier = organizationId,
                    OrganizationName = "Star Wars AB",
                    OrganizationVat = "5567144950",
                };

                context.Projects.Update(project);
            }

            context.SaveChanges();
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}