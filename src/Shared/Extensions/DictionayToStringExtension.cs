using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions;
public static class DictionayToStringExtension
{
    public static string ToIndexString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException("dictionary");
        }

        IEnumerable<string> items = from kvp in dictionary
                                    select kvp.Key + "=" + kvp.Value;

        return "{" + string.Join(",", items) + "}";
    }
}
