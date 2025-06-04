using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class UpdateClaimTypeDto
{
    
    public List<string> Names { get; set; } = [];
    public string Tag { get; set; } = string.Empty;
}