namespace Shared.Extensions;
public static class BooleanExtensions
{
    public static string ConvertToString(this bool data)
    {
        return data.ToString();
    }

    public static int ConvertToInt(this bool data)
    {
        return int.TryParse(data.ToString(), out int d) ? d : 0;
    }

    public static bool ConvertToBool(this bool? data)
    {
        if (data is null) return false;
        return bool.TryParse(data.ToString(), out bool d) && d;
    }
    public static string ConvertToYesNo(this bool data)
    {
        return data ? "Yes" : "No";
    }
}
