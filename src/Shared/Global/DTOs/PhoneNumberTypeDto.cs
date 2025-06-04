using System.Collections.Generic;

namespace Shared.Global.DTOs;

public class PhoneNumberTypeDto
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string First => Names.Count > 0 ? Names[0] : "Unknown";
}