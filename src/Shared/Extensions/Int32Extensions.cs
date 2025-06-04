using System;

namespace Shared.Extensions;
public static class Int32Extensions
{
    public static string ConvertToString(this int data)
    {
        return data.ToString();
    }

    public static double ConvertToDouble(this int data)
    {
        return Convert.ToDouble(data);
    }

    public static bool ConvertToBool(this int data)
    {
        return Convert.ToBoolean(data);
    }
}
