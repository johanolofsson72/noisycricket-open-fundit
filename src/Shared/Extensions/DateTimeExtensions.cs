using System;

namespace Shared.Extensions;
public static class DateTimeExtensions
{
    public static DateTime ConvertToDateTime(this DateTime? data)
    {
        if (data is null)
        {
            return DateTime.MinValue;
        }

        _ = DateTime.TryParse(data!.ToString(), out DateTime d);
        return d;
    }
}
