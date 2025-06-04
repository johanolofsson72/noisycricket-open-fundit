namespace Shared.Extensions;
public static class SafeStringExtension
{
    public static string SafeString(this string Source)
    {
        Source = Source.Replace("'", "´");
        return Source;
    }
}
