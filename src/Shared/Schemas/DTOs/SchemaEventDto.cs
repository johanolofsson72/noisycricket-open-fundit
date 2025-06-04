
using System.Collections.Generic;

namespace Shared.Schemas.DTOs;

public class SchemaEventDto
{
    public SchemaEventDto(){}
    public SchemaEventDto(int identifier, int order, int eventTypeId)
    {
        Id = identifier;
        SchemaEventIdentifier = identifier;
        Order = order;
        EventTypeId = eventTypeId;
    }
    public int Id { get; set; } = 0;
    public int SchemaEventIdentifier { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int Order { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public int DependOnEventId { get; set; } = 0;
    public bool IsFirstInChain { get; set; } = false;
    public bool IsLastInChain { get; set; } = false;
    public bool IsStandAlone { get; set; } = false;
    public List<SchemaEventActionDto> Actions { get; set; } = new();
}