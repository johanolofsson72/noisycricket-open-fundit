using System;

namespace Shared.Messages.DTOs;

public class CreateMessageDto
{
    public MessageContactDto Receiver { get; set; } = new MessageContactDto();
    public int EventId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public int MessageTypeId { get; set; } = 0;
    public bool Outgoing { get; set; } = false;
    public bool Incoming { get; set; } = false;
    public int StatusId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public string OrganizationName { get; set; } = string.Empty;
    public int ProjectId { get; set; } = 0;
    public string ProjectTitle { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public int ApplicationId { get; set; } = 0;
    public string ApplicationTitle { get; set; } = string.Empty;
    public int ApplicationStatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public string DocumentLink { get; set; } = string.Empty;
    public int DocumentTypeId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public int ReactionDescriptionId { get; set; } = 0;
    public int SystemMessageDestinationId { get; set; } = 0;
    public DateTime ExecutionDate { get; set; } = DateTime.MinValue;
    public DateTime ExpireDate { get; set; } = DateTime.MinValue;
}