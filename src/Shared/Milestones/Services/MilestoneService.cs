using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Data.DbContext;
using Shared.Global.Structs;
using Shared.Milestones.DTOs;

namespace Shared.Milestones.Services;

public class MilestoneService(IDbContextFactory<ApplicationDbContext> factory, IHttpClientFactory httpClientFactory, ApplicationService applicationService)
{
    public async Task<Result<bool, Exception>> AggregateAsync(CancellationToken ct, int milestoneId = 0)
    {
        try
        {
            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");
            var response = await httpClient.PostAsJsonAsync($"/api/v1/jobs/aggregate/milestones/" + milestoneId, emptyPayload, ct);

            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<MilestoneDto, Exception>> CreateMilestoneAsync(int applicationId, CreateMilestoneDto dto, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("ApplicationId is required");
            if (dto.Amount == 0) throw new Exception("Amount is required");
            if (dto.ExpireDate == DateTime.MinValue) throw new Exception("ExpireDate is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = new Milestone
            {
                ApplicationId = applicationId,
                StatusId = dto.StatusId,
                Amount = dto.Amount,
                CreatedDate = DateTime.UtcNow,
                ExpireDate = dto.ExpireDate,
                IsLocked = dto.IsLocked,
                Requirements = dto.Requirements.Select(x => new MilestoneRequirement
                {
                    RequirementIdentifier = x.RequirementIdentifier,
                    RequirementTypeId = x.RequirementTypeId,
                    DeliveryTypeId = x.DeliveryTypeId,
                    DocumentId = x.DocumentId,
                    IsApproved = x.IsApproved,
                    ApprovedDate = x.ApprovedDate,
                    ExpireDate = x.ExpireDate,
                    Name = x.Name,
                    CreatedDate = DateTime.UtcNow
                }).ToList(),
                Payments = dto.Payments.Select(x => new MilestonePayment
                {
                    Amount = x.Amount,
                    CreatedDate = DateTime.UtcNow,
                    Note = x.Note
                }).ToList()
            };
            
            await context.Milestones.AddAsync(milestone, ct);
            await context.SaveChangesAsync(ct);

            await applicationService.AddApplicationAuditAsync(applicationId, "Milestone Created", [ milestone.Id.ToString()], "MilestoneService.CreateMilestoneAsync", ct);

            await AggregateAsync(ct, milestone.Id);

            return milestone.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> CreateMilestonesAsync(CreateMilestonesDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.ApplicationId == 0) throw new Exception("ApplicationId is required");
            if (dto.ReceivedAmount == 0) throw new Exception("ReceivedAmount is required");
            
            var milestonesResult = await MilestonesByApplicationIdAsync(dto.ApplicationId, new  CancellationToken());  
            if (!milestonesResult.IsOk) throw milestonesResult.Error;
            if (milestonesResult.Value.Any()) return false;
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var applicationResult = await context.Applications
                .AsNoTracking()
                .Where(x => x.Id == dto.ApplicationId)
                .FirstOrDefaultAsync(ct);
            if (applicationResult is null) throw new Exception("Application not found");

            var progress = applicationResult.Progress;
            var totalAmount = dto.ReceivedAmount;
            var currentAmount = 0m;
            
            foreach (var item in progress)
            {
                var statusId = 2;
                var percentageOfAmount = item.PercentageOfAmount;
                var amount = percentageOfAmount == 0 ? 0 : (percentageOfAmount / 100) * dto.ReceivedAmount;
                var requirements = new List<MilestoneRequirementDto>();
                var payments = new List<MilestonePaymentDto>();
                
                if (amount + currentAmount > totalAmount)
                {
                    amount = totalAmount - currentAmount;
                }

                foreach (var requirement in item.Requirements)
                {
                    requirements.Add(new MilestoneRequirementDto()
                    {
                        RequirementIdentifier = requirement.ApplicationProgressRequirementIdentifier,
                        DeliveryTypeId = requirement.DocumentDeliveryTypeId,
                        RequirementTypeId = requirement.MilestoneRequirementTypeId,
                        ExpireDate = DateTime.UtcNow.AddMonths(item.MonthToExpire),
                        CreatedDate = DateTime.UtcNow
                    });
                }
                
                var milestoneResult = await CreateMilestoneAsync(
                    dto.ApplicationId,
                    new CreateMilestoneDto()
                    {
                        StatusId = statusId,
                        Amount = amount,
                        ExpireDate = DateTime.UtcNow.AddMonths(item.MonthToExpire),
                        IsLocked = false,
                        Requirements = requirements,
                        Payments = payments
                    }, ct);
                
                if (!milestoneResult.IsOk) throw milestoneResult.Error;
                
                currentAmount += amount;
            }
            
            await applicationService.AddApplicationAuditAsync(dto.ApplicationId, "Milestones Created", [ dto.ReceivedAmount.ToString() ], "MilestoneService.CreateMilestonesAsync", ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
        
    public async Task<Result<bool, Exception>> UpdateMilestoneAsync(int milestoneId, UpdateMilestoneDto dto, CancellationToken ct)
    {
        try
        {
            if (milestoneId == 0) throw new Exception("milestoneId is required");
            if (dto.StatusId == 0) throw new Exception("Status is required");
            if (dto.Amount == 0) throw new Exception("Amount is required");
            if (dto.ExpireDate == DateTime.MinValue) throw new Exception("ExpireDate is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                .AsTracking()
                .Where(x => x.Id == milestoneId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");

            var requirements = new List<MilestoneRequirement>();
            var payments = new List<MilestonePayment>();

            if (dto.Requirements is not null)
            {
                foreach (var requirement in dto.Requirements)
                {
                    requirements.Add(new MilestoneRequirement
                    {
                        RequirementIdentifier = requirement.RequirementIdentifier,
                        RequirementTypeId = requirement.RequirementTypeId,
                        DeliveryTypeId = requirement.DeliveryTypeId,
                        DocumentId = requirement.DocumentId,
                        IsApproved = requirement.IsApproved,
                        ApprovedDate = requirement.ApprovedDate,
                        ExpireDate = requirement.ExpireDate,
                        Name = requirement.Name
                    });
                }
            }

            if (dto.Payments is not null)
            {
                foreach (var payment in dto.Payments)
                {
                    payments.Add(new MilestonePayment()
                    {
                        Amount = payment.Amount,
                        CreatedDate = payment.CreatedDate,
                        Note = payment.Note
                    });
                }
            }

            milestone.StatusId = dto.StatusId;
            milestone.Amount = dto.Amount;
            milestone.ExpireDate = dto.ExpireDate;
            milestone.IsLocked = dto.IsLocked;
            milestone.Requirements = requirements;
            milestone.Payments = payments;

            context.Milestones.Update(milestone);
            await context.SaveChangesAsync(ct);

            await applicationService.AddApplicationAuditAsync(milestone.ApplicationId, "Milestone Updated", [ 
                milestone.Id.ToString(),
                milestone.StatusId.ToString(),
                milestone.ExpireDate.ToString(),
                milestone.IsLocked.ToString()
            ], "MilestoneService.UpdateMilestoneAsync", ct);

            await AggregateAsync(ct, milestone.ApplicationId);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteMilestoneAsync(int milestoneId, CancellationToken ct)
    {
        try
        {
            if (milestoneId == 0) throw new Exception("milestoneId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                .AsTracking()
                .Where(x => x.Id == milestoneId && x.IsLocked == false)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");
            
            milestone.StatusId = 19;
            
            context.Milestones.Update(milestone);
            await context.SaveChangesAsync(ct);

            await applicationService.AddApplicationAuditAsync(milestone.ApplicationId, "Milestone Deleted", [ 
                milestone.Id.ToString()
            ], "MilestoneService.DeleteMilestoneAsync", ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<MilestoneDto, Exception>> MilestoneByIdAsync(int milestoneId, CancellationToken ct)
    {
        try
        {
            if (milestoneId == 0) throw new Exception("milestoneId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                    .AsNoTracking()
                    .Where(x => x.Id == milestoneId)
                    .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");

            return milestone.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<MilestoneDto, Exception>> MilestoneByMilestoneIdAndRequirementIdAsync(int milestoneId, int requirementId, CancellationToken ct)
    {
        try
        {
            if (milestoneId == 0) throw new Exception("milestoneId is required");
            if (requirementId == 0) throw new Exception("requirementId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                    .AsNoTracking()
                    .Where(x => x.Id == milestoneId && x.Requirements.Any(x => x.RequirementIdentifier == requirementId))
                    .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");

            return milestone.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<MilestonesSummaryDto>, Exception>> MilestonesSummaryByApplicationIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var summary = await context.Milestones
                    .AsNoTracking()
                    .Where(x => x.StatusId != 19 && x.ApplicationId == applicationId)
                    .Select(x => new MilestonesSummaryDto { 
                        Id = x.Id
                    })
                    .ToListAsync(ct) ?? throw new Exception("Milestones not found");

            return summary;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<MilestoneDto>, Exception>> MilestonesByApplicationIdAsync(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestones = await context.Milestones
                .AsNoTracking()
                .Where(x => x.StatusId != 19 && x.ApplicationId == applicationId)
                .ToListAsync(ct) ?? throw new Exception("Milestones not found");

            return milestones.Select(x => x.ToDto()).ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<MilestoneRequirementDto, Exception>> AddRequirementAsync(int milestoneId, CreateMilestoneRequirementDto dto, CancellationToken ct)
    {
        try
        {
            if (milestoneId == 0) throw new Exception("milestoneId is required");
            if (dto.DeliveryTypeId == 0) throw new Exception("DeliveryTypeId is required");
            if (dto.ExpireDate == DateTime.MinValue) throw new Exception("ExpireDate is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            context.ChangeTracker.Clear();
            var milestone = await context.Milestones
                .AsTracking()
                .Where(x => x.Id == milestoneId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");
            
            var requirements = milestone.Requirements.ToList();

            var requirement = new MilestoneRequirement
            {
                RequirementIdentifier = requirements.Count + 1,
                RequirementTypeId = dto.RequirementTypeId,
                DeliveryTypeId = dto.DeliveryTypeId,
                DocumentId = dto.DocumentId,
                IsApproved = dto.IsApproved,
                ApprovedDate = dto.ApprovedDate,
                ExpireDate = dto.ExpireDate,
                Name = dto.Name,
                CreatedDate = DateTime.UtcNow
            };
            requirements.Add(requirement);
            milestone.Requirements = requirements;

            context.Milestones.Update(milestone);
            await context.SaveChangesAsync(ct);
            
            await applicationService.AddApplicationAuditAsync(milestone.ApplicationId, "Milestone Requirement Created", [ 
                milestone.Id.ToString(),
                requirement.RequirementIdentifier.ToString()
            ], "MilestoneService.AddRequirementAsync", ct);

            await AggregateAsync(ct, milestone.ApplicationId);

            return requirement.ToDto();
            
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<MilestoneRequirementDto, Exception>> UpdateRequirementAsync(int milestoneId, int requirementId, UpdateMilestoneRequirementDto dto, CancellationToken ct)
    {
        try
        {
            if (requirementId == 0) throw new Exception("requirementId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                .AsTracking()
                .Where(x => x.Id == milestoneId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");

            var requirements = milestone.Requirements.ToList();

            foreach (var item in requirements)
            {
                if (item.RequirementIdentifier == requirementId)
                {
                    item.RequirementTypeId = dto.RequirementTypeId;
                    item.DeliveryTypeId = dto.DeliveryTypeId;
                    item.DocumentId = dto.DocumentId;
                    item.IsDelivered = dto.IsDelivered;
                    item.DeliveredDate = dto.DeliveredDate;
                    item.IsApproved = dto.IsApproved;
                    item.ApprovedDate = dto.ApprovedDate;
                    item.ExpireDate = dto.ExpireDate;
                    item.Name = dto.Name;
                }
            }
            
            milestone.Requirements = requirements;

            context.Milestones.Update(milestone);
            await context.SaveChangesAsync(ct);
            
            await applicationService.AddApplicationAuditAsync(milestone.ApplicationId, "Milestone Requirement Updated", [ 
                milestoneId.ToString(),
                requirementId.ToString(),
                dto.RequirementTypeId.ToString(),
                dto.DeliveryTypeId.ToString(),
                dto.DocumentId.ToString(),
                dto.IsDelivered.ToString(),
                dto.DeliveredDate.ToString(),
                dto.IsApproved.ToString(),
                dto.ApprovedDate.ToString(),
                dto.ExpireDate.ToString(),
                dto.Name
            ], "MilestoneService.UpdateRequirementAsync", ct);

            await AggregateAsync(ct, milestone.ApplicationId);
            
            var requirement = requirements
                .Where(x => x.RequirementIdentifier == requirementId)!.FirstOrDefault() ?? throw new Exception("Requirement not found");

            return requirement.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<MilestoneRequirementDto, Exception>> RequirementByIdAsync(int milestoneId, int requirementId, CancellationToken ct)
    {
        try
        {
            if (requirementId == 0) throw new Exception("requirementId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                .Where(x => x.Id == milestoneId)
                .Where(x => x.Requirements.Any(x => x.RequirementIdentifier == requirementId))
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");

            var requirement = milestone.Requirements.FirstOrDefault() ?? throw new Exception("Requirement not found");

            return requirement.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteRequirementAsync(int requirementId, CancellationToken ct)
    {
        try
        {
            if (requirementId == 0) throw new Exception("requirementId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var milestone = await context.Milestones
                .AsTracking()
                .Where(x => x.Requirements.Any(x => x.RequirementIdentifier == requirementId))
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Milestone not found");

            milestone.Requirements = milestone.Requirements.Where(x => x.RequirementIdentifier != requirementId).ToList();

            context.Milestones.Update(milestone);
            await context.SaveChangesAsync(ct);

            await applicationService.AddApplicationAuditAsync(milestone.ApplicationId, "Milestone Requirement Deleted", [ 
                milestone.Id.ToString(),
                requirementId.ToString()
            ], "MilestoneService.DeleteRequirementAsync", ct);

            await AggregateAsync(ct, milestone.Id);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    

}
