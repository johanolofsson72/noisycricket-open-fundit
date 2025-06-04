using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class ClaimTypeDto
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string Tag { get; set; } = string.Empty;
}