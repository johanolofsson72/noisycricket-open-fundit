using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Data.DbContext;
using Shared.Global.Structs;
using Shared.Messages.DTOs;
using Shared.Messages.Entities;

namespace Shared.Messages.Services;

public class MessageService(IDbContextFactory<ApplicationDbContext> factory, IHttpClientFactory httpClientFactory)
{
    
    private async Task AggregateAsync(int messageId, CancellationToken ct)
    {
        try
        {
            if (messageId == 0) return;
            var emptyPayload = new {};
            var httpClient = httpClientFactory.CreateClient("api");
            var response = await httpClient.PostAsJsonAsync($"/api/v1/jobs/aggregate/messages/" + messageId, emptyPayload, ct);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            // ignored
        }
    }

    public async Task<Result<MessageDto, Exception>> CreateMessageAsync(CreateMessageDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Receiver.ContactIdentifier == 0) throw new Exception("Receiver.ContactIdentifier is required");
            if (dto.Receiver.Name == string.Empty) throw new Exception("Receiver.Name is required");
            if (dto.Receiver.Email == string.Empty) throw new Exception("Receiver.Email is required");
            if (dto.StatusId == 1) throw new Exception("Status is required");
            if (dto.OrganizationId == 0) throw new Exception("OrganizationId is required");
            if (dto.OrganizationName == string.Empty) throw new Exception("OrganizationName is required"); 
            if (dto.ApplicationId == 0) throw new Exception("ApplicationId is required");
            if (dto.ApplicationTitle == string.Empty) throw new Exception("ApplicationTitle is required");
            if (dto.ApplicationStatusId == 1) throw new Exception("ApplicationStatus is required");
            if (dto.SchemaId == 0) throw new Exception("Schema is required");
            if (dto.Title == string.Empty) throw new Exception("Title is required");
            if (dto.ExecutionDate < DateTime.UtcNow) dto.ExecutionDate = DateTime.UtcNow;
            if (dto.ExpireDate == DateTime.MinValue) throw new Exception("ExpireDate is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var message = await context.Messages
                .Where(x => x.Receiver.ContactIdentifier == dto.Receiver.ContactIdentifier)
                .Where(x => x.ProjectId == dto.ProjectId)
                .Where(x => x.ApplicationId == dto.ApplicationId)
                .Where(x => x.EventId == dto.EventId)
                .Where(x => x.EventTypeId == dto.EventTypeId)
                .Where(x => x.Title == dto.Title)
                .FirstOrDefaultAsync(ct) ?? new Message();
            
            if (message.Id != 0) return message.ToDto();
            
            message.Receiver = dto.Receiver;
            message.EventId = dto.EventId;
            message.EventTypeId = dto.EventTypeId;
            message.Incoming = dto.Incoming;
            message.Outgoing = dto.Outgoing;
            message.StatusId = dto.StatusId;
            message.OrganizationId = dto.OrganizationId;
            message.OrganizationName = dto.OrganizationName;
            message.ProjectId = dto.ProjectId;
            message.ProjectTitle = dto.ProjectTitle;
            message.ProjectNumber = dto.ProjectNumber;
            message.ApplicationId = dto.ApplicationId;
            message.ApplicationTitle = dto.ApplicationTitle;
            message.ApplicationStatusId = dto.ApplicationStatusId;
            message.SchemaId = dto.SchemaId;
            message.DocumentLink = dto.DocumentLink;
            message.DocumentType = dto.DocumentTypeId;
            message.Title = dto.Title;
            message.ReactionDescription = dto.ReactionDescriptionId;
            message.SystemMessageDestination = dto.SystemMessageDestinationId;
            message.CreatedDate = DateTime.UtcNow;
            message.ExecutionDate = dto.ExecutionDate;
            message.ExpireDate = dto.ExpireDate;
            
            await context.Messages.AddAsync(message, ct);
            await context.SaveChangesAsync(ct);
            
            await AggregateAsync(message.Id, ct);

            return message.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<List<SlimMessageDto>, Exception>> MessagesByUserIdAsync(int userId, int statusId, CancellationToken ct)
    {
        try
        {
            if (userId == 0) throw new Exception("userId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var messages = GetMessagesByUserIdAsync(context, userId, statusId) ?? throw new Exception("Messages not found");
            return await messages.ToListAsync(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteMessageAsync(int id, CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            await context.Messages
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(
                s => s.SetProperty(b => b.StatusId, b => 19), ct);
            
            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public int AmountOfUnprocessedMessagesAsync(int applicationId)
    {
        try
        {
            using var context = factory.CreateDbContext();
            var count = context.Messages
                .AsNoTracking()
                .Where(x => x.StatusId == 3)
                .Count(x => x.ApplicationId == applicationId);
            return count;
        }
        catch
        {
            return 0;
        }
    }
    
    
    private static readonly Func<ApplicationDbContext, int, int, IAsyncEnumerable<SlimMessageDto>> GetMessagesByUserIdAsync = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int userId, int statusId) => 
            context.Messages
                .AsNoTracking()
                .TagWith("GetMessagesByUserIdAsync")
                .Where(x => x.Receiver.ContactIdentifier == userId && x.StatusId == statusId)
                .Select(x => new SlimMessageDto
                {
                    Id = x.Id,
                    StatusId = x.StatusId,
                    SchemaId = x.SchemaId,
                    SchemaNames = x.SchemaNames,
                    Title = x.Title,
                    ExpireDate = x.ExpireDate,
                    ReactionDescription = x.ReactionDescription,
                    ApplicationId = x.ApplicationId,
                    ApplicationTitle = x.ApplicationTitle,
                    Incoming = x.Incoming,
                    ProjectId = x.ProjectId,
                    SystemMessageDestination = x.SystemMessageDestination,
                    DocumentLink = x.DocumentLink,
                    OrganizationId = x.OrganizationId,
                    OrganizationName = x.OrganizationName,
                    ProjectNumber = x.ProjectNumber,
                    MessageTypeId = x.MessageTypeId
                }));
}