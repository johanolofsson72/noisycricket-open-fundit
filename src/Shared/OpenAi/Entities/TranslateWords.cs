namespace Shared.OpenAi.Entities;

public class TranslateWords
{
    public required string TextToTranslate { get; set; }
    public required string From { get; set; }
    public required string[] To { get; set; }
}