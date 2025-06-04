using Telerik.Blazor.Services;

namespace AppAdmin.Resources;

public class LocalizationService : ITelerikStringLocalizer
{
    // this is the indexer you must implement
    public string this[string name] => StringFromResource(name);

    // sample implementation - uses .resx files in the ~/Resources folder named TelerikMessages.<culture-locale>.resx
    public static string StringFromResource(string key)
    {
        return Resources.Localization.ResourceManager.GetString(key, Resources.Localization.Culture)!;
    }
}
