using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Global.DTOs;

[NotMapped]
public class CurrencyDto
{
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18, 4)")] public decimal Rate { get; set; }
    public DateTime CreatedDate { get; set; }
}