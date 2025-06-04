using System.Collections.Generic;
using Shared.Statistics.Enums;

namespace Shared.Statistics.DTOs;

public class StatisticRenderDto
{
    public string SingleRowOneItemValue { get; set; } = string.Empty;
    public List<string> SingleRowTwoItemsValue { get; set; } = [];
    public List<string> SingleRowThreeItemsValue { get; set; } = [];
    public List<string> SingleRowFourItemsValue { get; set; } = [];
    public List<string> ManyRowsOneItem { get; set; } = [];
    public List<List<string>> ManyRowsTwoItems { get; set; } = [];
    public List<List<string>> ManyRowsThreeItems { get; set; } = [];
    public List<List<string>> ManyRowsFourItems { get; set; } = [];
    public List<List<string>> ManyRowsFiveItems { get; set; } = [];
    public List<List<string>> ManyRowsSixItems { get; set; } = [];
    public List<List<string>> ManyRowsSevenItems { get; set; } = [];
    public StatisticRenderType ReturnType { get; set; } = StatisticRenderType.NoData;
}