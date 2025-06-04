using System.ComponentModel;

namespace Shared.Global.Enums;

public enum CacheKeyPrefix
{
    [Description("Applications")]
    Applications = 1,
    
    [Description("Controls")]
    Controls = 2,
    
    [Description("Documents")]
    Documents = 3,
    
    [Description("Events")]
    Events = 4,
    
    [Description("Messages")]
    Messages = 5,
    
    [Description("Milestones")]
    Milestones = 6,
    
    [Description("Projects")]
    Projects = 7,
    
    [Description("Schemas")]
    Schemas = 8,
    
    [Description("Statistics")]
    Statistics = 9,
    
    [Description("Users")]
    Users = 10,
    
    [Description("Organizations")]
    Organizations = 11,
    
    [Description("ApplicationStates")]
    ApplicationStates = 12,
    
    [Description("ApplicationBudgets")]
    ApplicationBudgets = 12
}

public static class CacheKeyExtensions
{
    public static string ToDescriptionString(this CacheKeyPrefix val)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        var attributes = (DescriptionAttribute[])val
            .GetType()
            .GetField(val.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        return attributes is { Length: > 0 } ? attributes[0].Description : string.Empty;
    }
} 