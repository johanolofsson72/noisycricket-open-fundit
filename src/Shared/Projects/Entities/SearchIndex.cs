using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Projects.Entities;

public class SearchIndex
{
    public int RowId { get; set; } = 0;
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18, 4)")] public decimal? Rank { get; set; }
}
