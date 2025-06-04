using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Global.DTOs;

[NotMapped]
public class SummaryDto
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
}

[NotMapped]
public class SummaryListDto
{
    public int Id { get; set; } = 0;
    public List<string> Names { get; set; } = [];
}
