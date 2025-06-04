using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class UpdateReactionTypeDto
{
    
    public List<string> Names { get; set; } = [];
    public List<string> Messages { get; set; } = [];
}