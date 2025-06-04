using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class SystemMessageDestinationDto
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}