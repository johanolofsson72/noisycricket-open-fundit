using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class UpdateSectionDto
{
    
    public List<string> Names { get; set; } = [];
    public int Order { get; set; } = 0;
    public bool Enabled { get; set; } = false;
}