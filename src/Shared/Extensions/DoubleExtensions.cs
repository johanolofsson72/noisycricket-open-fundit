namespace Shared.Extensions;
public static class DoubleExtensions
{
    public static string ConvertToString(this double data)
    {
        return data.ToString().Replace(".", ",");
        //return string.Format("{0:0,00}", data);
    }
}
