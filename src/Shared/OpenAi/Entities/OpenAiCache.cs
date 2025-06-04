using System;

namespace Shared.OpenAi.Entities;

public class OpenAiCache
{
    public int Id { get; set; } = 0;
    public string Query { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    
    public int ReturnCount { get; set; } = 0;
    public DateTime ExpireDate { get; set; } = DateTime.MinValue;
}