using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class ReactionTypeDto
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    
    public List<string> Messages { get; set; } = ["", "", "", "", "", "", "", ""];
}