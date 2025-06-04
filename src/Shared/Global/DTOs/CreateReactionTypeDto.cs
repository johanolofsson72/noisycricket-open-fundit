using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class CreateReactionTypeDto
{
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public List<string> Messages { get; set; } = ["", "", "", "", "", "", "", ""];
}