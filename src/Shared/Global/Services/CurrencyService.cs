
using System.Collections.Generic;
using Shared.Global.DTOs;

namespace Shared.Global.Services;

public static class CurrencyService
{
    public static IEnumerable<CurrencyDto> Currencies { get; set; } = default!;

    public static void SetCurrencies(IEnumerable<CurrencyDto> currencies)
    {
        Currencies = currencies;
    }
}