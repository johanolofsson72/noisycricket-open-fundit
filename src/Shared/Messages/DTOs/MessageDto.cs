using System;

namespace Shared.Messages.DTOs;

public class MessageDto
{
    public int Id { get; set; } = 0;
    public MessageContactDto Receiver { get; set; } = new MessageContactDto();
    public int EventId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public int MessageTypeId { get; set; } = 0;
    public bool Outgoing { get; set; } = false;
    public bool Incoming { get; set; } = false;
    public int StatusId { get; set; } = 0;
    public int OrganizationId { get; set; } = 0;
    public string OrganizationName { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public string ApplicationTitle { get; set; } = string.Empty;
    public int ApplicationStatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string DocumentLink { get; set; } = string.Empty;
    public int DocumentType { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public int ReactionDescription { get; set; }  = 0;
    public int SystemMessageDestination { get; set; } = 0;
    public DateTime CreatedDate { get; set; } = DateTime.MinValue;
    public DateTime ExecutionDate { get; set; } = DateTime.MinValue;
    public DateTime ExpireDate { get; set; } = DateTime.MinValue;
}



