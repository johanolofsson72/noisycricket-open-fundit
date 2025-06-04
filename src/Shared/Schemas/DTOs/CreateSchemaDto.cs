
using System.Collections.Generic;

namespace Shared.Schemas.DTOs;

public class CreateSchemaDto
{
    public int StatusId { get; set; } = 0;
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string ClaimTag { get; set; } = "";
    public List<SchemaControlDto> Controls { get; set; } = new();
    public bool Enabled { get; set; } = false;
}