using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Data.DbContext;
using Shared.Global.Structs;
using Shared.Schemas.DTOs;
using Shared.Schemas.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Schemas.Services;

public class SchemaService(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<Result<List<SchemaEventActionDto>, Exception>> AddSchemaEventActionAsync(int schemaId, int eventId, AddSchemaEventActionDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (eventId == 0) throw new Exception("SchemaEventId is required");
            if (dto.StatusId == 1) throw new Exception("StatusId is required");
            if (dto.ActionTypeId == 1) throw new Exception("ActionTypeId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var ev = schema.Events
                .FirstOrDefault(x => x.SchemaEventIdentifier == eventId) ?? throw new Exception("Event not found");

            var action = new SchemaEventAction()
            {
                SchemaEventIdentifier = ev.SchemaEventIdentifier,
                SchemaEventActionIdentifier = ev.Actions.Count + 1,
                StatusId = dto.StatusId,
                ActionTypeId = dto.ActionTypeId,
                ExecutionDate = dto.ExecutionDate,
                ReceiverClaimTypeId = dto.ReceiverClaimTypeId,
                SystemMessage = dto.SystemMessage,
                SystemMessageDestinationId = dto.SystemMessageDestinationId,
                DocumentLink = dto.DocumentLink,
                EmailMessageBody = dto.EmailMessageBody,
                ReactionDescriptionId = dto.ReactionDescriptionId,
                SystemMessageIdToDelete = dto.SystemMessageIdToDelete,
                DeleteEventId = dto.DeleteEventId,
                DeleteActionId = dto.DeleteActionId,
                EventActionCombo = dto.EventActionCombo,
                ChangeStatusToId = dto.ChangeStatusToId
            };
            
            ev.Actions.Add(action);
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return ev.Actions.ToSchemaEventActionDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaEventActionDto>, Exception>> UpdateSchemaEventActionAsync(int schemaId, int eventId, UpdateSchemaEventActionDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (eventId == 0) throw new Exception("SchemaEventId is required");
            if (dto.StatusId == 1) throw new Exception("StatusId is required");
            if (dto.ActionTypeId == 1) throw new Exception("ActionTypeId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var ev = schema.Events
                .FirstOrDefault(x => x.SchemaEventIdentifier == eventId) ?? throw new Exception("Event not found");
            
            var actionToBeUpdated = ev.Actions
                .FirstOrDefault(x => x.SchemaEventActionIdentifier == dto.Identifier) ?? throw new Exception("Action not found");          
            
            schema.UpdatedDate = DateTime.UtcNow;
            
            actionToBeUpdated.ActionTypeId = dto.ActionTypeId;
            actionToBeUpdated.ChangeStatusToId = dto.ChangeStatusToId;
            actionToBeUpdated.ExecutionDate = dto.ExecutionDate;
            actionToBeUpdated.ReceiverClaimTypeId = dto.ReceiverClaimTypeId;
            actionToBeUpdated.SystemMessage = dto.SystemMessage;
            actionToBeUpdated.SystemMessageDestinationId = dto.SystemMessageDestinationId;
            actionToBeUpdated.SystemMessagesCreated = dto.SystemMessagesCreated;
            actionToBeUpdated.DocumentLink = dto.DocumentLink;
            actionToBeUpdated.EmailMessageBody = dto.EmailMessageBody;
            actionToBeUpdated.ReactionDescriptionId = dto.ReactionDescriptionId;
            actionToBeUpdated.SystemMessageIdToDelete = dto.SystemMessageIdToDelete;
            actionToBeUpdated.DeleteEventId = dto.DeleteEventId;    
            actionToBeUpdated.DeleteActionId = dto.DeleteActionId;
            actionToBeUpdated.EventActionCombo = dto.EventActionCombo;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return ev.Actions.ToSchemaEventActionDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaEventActionDto>, Exception>> DeleteSchemaEventActionAsync(int schemaId, int eventId, int actionId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (eventId == 0) throw new Exception("SchemaControlId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var ev = schema.Events
                .FirstOrDefault(x => x.SchemaEventIdentifier == eventId) ?? throw new Exception("Event not found");
            
            var actions = ev.Actions
                .Where(x => x.SchemaEventActionIdentifier != actionId)
                .ToList();
            
            ev.Actions = actions;
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return ev.Actions.ToSchemaEventActionDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaEventDto>, Exception>> AddSchemaEventAsync(int schemaId, AddSchemaEventDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (dto.EventTypeId == 1) throw new Exception("EventTypeId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            var ev = new SchemaEvent()
            {
                SchemaEventIdentifier = schema.Events.Count + 1,
                Description = dto.Description,
                Labels = dto.Labels,
                DependOnEventId = dto.DependOnEventId,
                IsFirstInChain = dto.IsFirstInChain,
                IsLastInChain = dto.IsLastInChain,
                IsStandAlone = dto.IsStandAlone,
                Order = schema.Events.Count + 1,
                EventTypeId = dto.EventTypeId,
                StatusId = 3,
                Actions = new List<SchemaEventAction>()
            };
            
            schema.Events.Add(ev);
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Events.ToSchemaEventDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaEventDto>, Exception>> UpdateSchemaEventAsync(int schemaId, UpdateSchemaEventDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (dto.Identifier == 0) throw new Exception("Identifier is required");
            if (dto.EventTypeId == 1) throw new Exception("EventTypeId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var eventToBeUpdated = schema.Events
                    .FirstOrDefault(x => x.SchemaEventIdentifier == dto.Identifier) ?? new();            
            
            schema.UpdatedDate = DateTime.UtcNow;
            
            eventToBeUpdated.SchemaEventIdentifier = dto.Identifier;
            eventToBeUpdated.Description = dto.Description;
            eventToBeUpdated.Labels = dto.Labels;
            eventToBeUpdated.DependOnEventId = dto.DependOnEventId;
            eventToBeUpdated.IsFirstInChain = dto.IsFirstInChain;
            eventToBeUpdated.IsLastInChain = dto.IsLastInChain;
            eventToBeUpdated.IsStandAlone = dto.IsStandAlone;
            eventToBeUpdated.Order = dto.Order;
            eventToBeUpdated.EventTypeId = dto.EventTypeId;
            eventToBeUpdated.StatusId = dto.StatusId;
            eventToBeUpdated.Actions = dto.Actions.ToSchemaEventAction();

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Events.ToSchemaEventDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaEventDto>, Exception>> DeleteSchemaEventAsync(int schemaId, int eventId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (eventId == 0) throw new Exception("SchemaControlId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var events = schema.Events
                .Where(x => x.SchemaEventIdentifier != eventId)
                .ToList();
            
            schema.Events = events;
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Events.ToSchemaEventDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaControlDto>, Exception>> AddSchemaControlAsync(int schemaId, AddSchemaControlDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");
            if (dto.Control == null) throw new Exception("Control is required");
            if (schemaId != dto.SchemaId) throw new Exception("SchemaId does not match");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var control = dto.Control.ToEntity();
            control.SchemaControlIdentifier = schema.Controls.Count + 1;
            
            schema.Controls.Add(control);
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Controls.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaControlDto>, Exception>> UpdateSchemaControlAsync(int schemaId, UpdateSchemaControlDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");
            if (dto.Control == null) throw new Exception("Control is required");
            if (schemaId != dto.SchemaId) throw new Exception("SchemaId does not match");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var control = dto.Control.ToEntity();
            var controlToBeUpdated = schema.Controls
                    .FirstOrDefault(x => x.SchemaControlIdentifier == dto.Control.SchemaControlIdentifier) ?? new SchemaControl();            schema.UpdatedDate = DateTime.UtcNow;
            
            controlToBeUpdated.SchemaControlIdentifier = control.SchemaControlIdentifier;
            controlToBeUpdated.ControlId = control.ControlId;
            controlToBeUpdated.ControlTypeId = control.ControlTypeId;
            controlToBeUpdated.ControlTypeName = control.ControlTypeName;
            controlToBeUpdated.ControlValueType = control.ControlValueType;
            controlToBeUpdated.BaseStructure = control.BaseStructure;
            controlToBeUpdated.Order = control.Order;
            controlToBeUpdated.VisibleOnApplicationForm = control.VisibleOnApplicationForm;
            controlToBeUpdated.ApplicationFormPage = control.ApplicationFormPage;
            controlToBeUpdated.ApplicationFormOrder = control.ApplicationFormOrder;
            controlToBeUpdated.ApplicationFormRequired = control.ApplicationFormRequired;
            controlToBeUpdated.Css = control.Css;
            controlToBeUpdated.Labels = control.Labels;
            controlToBeUpdated.ApplicationSectionId = control.ApplicationSectionId;
            controlToBeUpdated.ApplicationFormSectionId = control.ApplicationFormSectionId;
            controlToBeUpdated.Placeholders = control.Placeholders;
            controlToBeUpdated.DataSource = control.DataSource;
            controlToBeUpdated.SubLabels = control.SubLabels;
            controlToBeUpdated.MaxValueLength = control.MaxValueLength;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Controls.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaControlDto>, Exception>> DeleteSchemaControlAsync(int schemaId, int controlId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (controlId == 0) throw new Exception("SchemaControlId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var controls = schema.Controls
                .Where(x => x.SchemaControlIdentifier != controlId)
                .ToList();
            
            schema.Controls = controls;
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Controls.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaProgressDto>, Exception>> AddSchemaProgressAsync(int schemaId, AddSchemaProgressDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");
            if (dto.Progress == null) throw new Exception("Progress is required");
            if (schemaId != dto.SchemaId) throw new Exception("SchemaId does not match");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var progress = dto.Progress.ToEntity();
            progress.SchemaProgressIdentifier = schema.Progress.Count + 1;
            
            schema.Progress.Add(progress);
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Progress.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaProgressDto>, Exception>> UpdateSchemaProgressAsync(int schemaId, UpdateSchemaProgressDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (dto.SchemaId == 0) throw new Exception("SchemaId is required");
            if (dto.Progress == null) throw new Exception("Progress is required");
            if (schemaId != dto.SchemaId) throw new Exception("SchemaId does not match");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var progress = dto.Progress.ToEntity();
            var progressToBeUpdated = schema.Progress
                    .FirstOrDefault(x => x.SchemaProgressIdentifier == dto.Progress.SchemaProgressIdentifier) ?? new SchemaProgress();            
            
            schema.UpdatedDate = DateTime.UtcNow;
            
            progressToBeUpdated.SchemaProgressIdentifier = progress.SchemaProgressIdentifier;
            progressToBeUpdated.PercentageOfAmount = progress.PercentageOfAmount;
            progressToBeUpdated.Requirements = progress.Requirements;
            progressToBeUpdated.MonthToExpire = progress.MonthToExpire;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Progress.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaProgressDto>, Exception>> DeleteSchemaProgressAsync(int schemaId, int progressId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (progressId == 0) throw new Exception("progressId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var progress = schema.Progress
                .Where(x => x.SchemaProgressIdentifier != progressId)
                .ToList();
            
            schema.Progress = progress;
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Progress.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaProgressDto>, Exception>> AddSchemaProgressRequirementAsync(int schemaId, int progressId, CreateSchemaProgressRequirementDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (progressId == 0) throw new Exception("progressId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var progress = schema.Progress.FirstOrDefault(x => x.SchemaProgressIdentifier == progressId);

            if (progress is not null)
            {
                progress.Requirements.Add(new SchemaProgressRequirement()
                {
                    SchemaProgressRequirementIdentifier = progress.Requirements.Count + 1,
                    MilestoneRequirementTypeId = dto.MilestoneRequirementTypeId,
                    DocumentDeliveryTypeId = dto.DocumentDeliveryTypeId
                });
                
                schema.UpdatedDate = DateTime.UtcNow;
            }

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.Progress.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaProgressDto>, Exception>> UpdateSchemaProgressRequirementAsync(int schemaId, int progressId, int requirementId, UpdateSchemaProgressRequirementDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (progressId == 0) throw new Exception("progressId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            var progress = schema.Progress.FirstOrDefault(x => x.SchemaProgressIdentifier == progressId);

            var requirement = progress?.Requirements.FirstOrDefault(x => x.SchemaProgressRequirementIdentifier == requirementId);
                
            if (requirement != null)
            {
                requirement.MilestoneRequirementTypeId = dto.MilestoneRequirementTypeId;
                requirement.DocumentDeliveryTypeId = dto.DocumentDeliveryTypeId;
            }

            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);
            
            return schema.Progress.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaProgressDto>, Exception>> DeleteSchemaProgressRequirementAsync(int schemaId, int progressId, int requirementId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (progressId == 0) throw new Exception("progressId is required");
            if (requirementId == 0) throw new Exception("requirementId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            var progress = schema.Progress.FirstOrDefault(x => x.SchemaProgressIdentifier == progressId);

            if (progress is not null)
            {
                var requirement = progress.Requirements
                    .Where(x => x.SchemaProgressRequirementIdentifier != requirementId)
                    .ToList();
                progress.Requirements = requirement;
            }

            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);
            
            return schema.Progress.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    
    public async Task<Result<List<SchemaRequiredDocumentDto>, Exception>> AddSchemaRequiredDocumentAsync(int schemaId, int requiredDocumentId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (requiredDocumentId == 0) throw new Exception("requiredDocumentId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            schema.RequiredDocuments.Add(new SchemaRequiredDocument()
            {
                RequiredDocumentIdentifier = schema.RequiredDocuments.Count + 1,
                RequiredDocumentId = requiredDocumentId
            });
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return schema.RequiredDocuments.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaRequiredDocumentDto>, Exception>> UpdateSchemaRequiredDocumentAsync(int schemaId, int requiredDocumentIdentifier, int requiredDocumentId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (requiredDocumentIdentifier == 0) throw new Exception("requiredDocumentIdentifier is required");
            if (requiredDocumentId == 0) throw new Exception("requiredDocumentId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            var requiredDocument = schema.RequiredDocuments.FirstOrDefault(x => x.RequiredDocumentIdentifier == requiredDocumentIdentifier);
            if (requiredDocument is null) throw new Exception("RequiredDocument not found");

            requiredDocument.RequiredDocumentId = requiredDocumentId;

            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);
            
            return schema.RequiredDocuments.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<List<SchemaRequiredDocumentDto>, Exception>> DeleteSchemaRequiredDocumentAsync(int schemaId, int requiredDocumentIdentifier, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("SchemaId is required");
            if (requiredDocumentIdentifier == 0) throw new Exception("requiredDocumentIdentifier is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            var requiredDocuments = schema.RequiredDocuments.Where(x => x.RequiredDocumentIdentifier != requiredDocumentIdentifier);
            
            schema.RequiredDocuments = requiredDocuments.ToList();
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);
            
            return schema.RequiredDocuments.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    
    
    
    public async Task<Result<SchemaDto, Exception>> CopySchemaAsync(int schemaId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("schemaId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            var controlsCopy = new List<SchemaControl>();
            foreach (var control in schema.Controls)
            {
                controlsCopy.Add(new SchemaControl()
                {
                    SchemaControlIdentifier = control.SchemaControlIdentifier,
                    ControlId = control.ControlId,
                    ControlTypeId = control.ControlTypeId,
                    ControlTypeName = control.ControlTypeName,
                    ControlValueType = control.ControlValueType,
                    BaseStructure = control.BaseStructure,
                    Order = control.Order,
                    VisibleOnApplicationForm = control.VisibleOnApplicationForm,
                    ApplicationFormPage = control.ApplicationFormPage,
                    ApplicationFormOrder = control.ApplicationFormOrder,
                    ApplicationFormRequired = control.ApplicationFormRequired,
                    Css = control.Css,
                    Labels = control.Labels,
                    ApplicationSectionId = control.ApplicationSectionId,
                    ApplicationFormSectionId = control.ApplicationFormSectionId,
                    Placeholders = control.Placeholders,
                    DataSource = control.DataSource,
                    SubLabels = control.SubLabels,
                    MaxValueLength = control.MaxValueLength
                });
            }
            
            var eventsCopy = new List<SchemaEvent>();
            foreach (var ev in schema.Events)
            {
                var actionsCopy = new List<SchemaEventAction>();
                foreach (var action in ev.Actions)
                {
                    actionsCopy.Add(new SchemaEventAction()
                    {
                        SchemaEventIdentifier = action.SchemaEventIdentifier,
                        SchemaEventActionIdentifier = action.SchemaEventActionIdentifier,
                        StatusId = action.StatusId,
                        ActionTypeId = action.ActionTypeId,
                        ExecutionDate = action.ExecutionDate,
                        ReceiverClaimTypeId = action.ReceiverClaimTypeId,
                        SystemMessage = action.SystemMessage,
                        SystemMessageDestinationId = action.SystemMessageDestinationId,
                        DocumentLink = action.DocumentLink,
                        EmailMessageBody = action.EmailMessageBody,
                        ReactionDescriptionId = action.ReactionDescriptionId,
                        SystemMessageIdToDelete = action.SystemMessageIdToDelete,
                        DeleteEventId = action.DeleteEventId,
                        DeleteActionId = action.DeleteActionId,
                        EventActionCombo = action.EventActionCombo,
                        ChangeStatusToId = action.ChangeStatusToId
                    });
                }
                eventsCopy.Add(new SchemaEvent()
                {
                    SchemaEventIdentifier = ev.SchemaEventIdentifier,
                    Order = ev.Order,
                    EventTypeId = ev.EventTypeId,
                    StatusId = ev.StatusId,
                    Actions = actionsCopy
                });
            }
            
            var progressCopy = new List<SchemaProgress>();
            foreach (var progress in schema.Progress)
            {
                var requirementsCopy = new List<SchemaProgressRequirement>();
                foreach (var requirement in progress.Requirements)
                {
                    requirementsCopy.Add(new SchemaProgressRequirement()
                    {
                        SchemaProgressRequirementIdentifier = requirement.SchemaProgressRequirementIdentifier,
                        MilestoneRequirementTypeId = requirement.MilestoneRequirementTypeId,
                        DocumentDeliveryTypeId = requirement.DocumentDeliveryTypeId
                    });
                }
                progressCopy.Add(new SchemaProgress()
                {
                    SchemaProgressIdentifier = progress.SchemaProgressIdentifier,
                    PercentageOfAmount = progress.PercentageOfAmount,
                    Requirements = requirementsCopy,
                    MonthToExpire = progress.MonthToExpire
                });
            }

            var copy = new Schema()
            {
                StatusId = schema.StatusId,
                Names = schema.Names.Select(x => x + " - Copy").ToList(),
                ClaimTag = schema.ClaimTag,
                Progress = progressCopy,
                Controls = controlsCopy,
                Events = eventsCopy,
                RequiredDocuments = schema.RequiredDocuments.Select(x => 
                    new SchemaRequiredDocument()
                    {
                        RequiredDocumentIdentifier = x.RequiredDocumentIdentifier,
                        RequiredDocumentId = x.RequiredDocumentId
                    }).ToList(),
                Enabled = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            
            await context.Schemas.AddAsync(copy, ct);
            await context.SaveChangesAsync(ct);

            return copy.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<SchemaDto, Exception>> CreateSchemaAsync(CreateSchemaDto dto, CancellationToken ct)
    {
        try
        {
            var schema = new Schema
            {
                StatusId = dto.StatusId,
                Names = dto.Names,
                ClaimTag = dto.ClaimTag,
                Controls = dto.Controls.Select(x => x.ToEntity()).ToList(),
                Enabled = dto.Enabled,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            await context.Schemas.AddAsync(schema, ct);
            await context.SaveChangesAsync(ct);
            
            return schema.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> UpdateSchemaAsync(int schemaId, UpdateSchemaDto dto, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("schemaId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");
            
            schema.StatusId = dto.StatusId;
            schema.Names = dto.Names;
            schema.ClaimTag = dto.ClaimTag;
            schema.Controls = dto.Controls.Select(x => x.ToEntity()).ToList();
            schema.Enabled = dto.Enabled;
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteSchemaAsync(int schemaId, CancellationToken ct)
    {
        try
        {
            if (schemaId == 0) throw new Exception("schemaId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var schema = await context.Schemas
                .AsTracking()
                .Where(x => x.Id == schemaId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Schema not found");

            schema.StatusId = 19;
            schema.UpdatedDate = DateTime.UtcNow;

            context.Schemas.Update(schema);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<SchemaDto, Exception>> SchemaByIdAsync(int schemaId)
    {
        try
        {
            if (schemaId == 0) throw new Exception("schemaId is required");

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var schema = await GetSchemaById(context, schemaId) ?? throw new Exception("Schema not found");

            return schema.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<SchemaDto>, Exception>> SchemasAsync()
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var schemas = GetSchemas(context) ?? throw new Exception("Schemas not found");
            
            return (await schemas.ToListAsync()).ToDto().ToList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    private static readonly Func<ApplicationDbContext, int, Task<Schema?>> GetSchemaById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int schemaId) => 
            context.Schemas
                .TagWith("GetSchemaById")
                .FirstOrDefault(x => x.Id == schemaId));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<Schema>> GetSchemas = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Schemas
                .TagWith("GetSchemas")
                .Where(x => x.StatusId != 19));
    
}
