using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions;
public static class DecimalsExtensions
{
    public static string ConvertToString(this decimal data)
    {
        return data.ToString().Replace(".", ",");
        //return string.Format("{0:0,00}", data);
    }
}
