
using System.Collections.Generic;

namespace Shared.Schemas.DTOs;

public class UpdateSchemaEventDto
{
    public int Identifier { get; set; } = 0;
    public int Order { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int EventTypeId { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = ["", "", "", "", "", "", "", ""];
    public int DependOnEventId { get; set; } = 0;
    public bool IsFirstInChain { get; set; } = false;
    public bool IsLastInChain { get; set; } = false;
    public bool IsStandAlone { get; set; } = false;
    public List<SchemaEventActionDto> Actions { get; set; } = new();
}