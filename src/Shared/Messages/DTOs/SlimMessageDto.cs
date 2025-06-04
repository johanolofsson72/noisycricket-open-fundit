using System;

namespace Shared.Messages.DTOs;

public class SlimMessageDto
{
    public int Id { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int SchemaId { get; set; } = 0;
    public List<string> SchemaNames { get; set; } = ["", "", "", "", "", "", "", ""];
    public string Title { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; } = DateTime.MinValue;
    public int ReactionDescription { get; set; }  = 0;
    public int ApplicationId { get; set; }
    public string ApplicationTitle { get; set; } = string.Empty;
    public bool Incoming { get; set; } = false;
    public int ProjectId { get; set; }
    public int SystemMessageDestination { get; set; } = 0;
    public string DocumentLink { get; set; } = string.Empty;
    public int OrganizationId { get; set; } = 0;
    public string OrganizationName { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public int MessageTypeId { get; set; } = 0;
}



