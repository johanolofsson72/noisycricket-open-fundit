using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Shared.Data.DbContext;
using Shared.Global.Structs;
using Shared.OpenAi.Services;
using Shared.Statistics.DTOs;
using Shared.Statistics.Entities;
using Shared.Statistics.Enums;

namespace Shared.Statistics.Services;

public class StatisticService(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<Result<StatisticDto, Exception>> StatisticByIdAsync(int statisticId)
    {
        try
        {
            if (statisticId == 0) throw new Exception("statisticId is required");

            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var statistic = await GetStatisticById(context, statisticId) ?? throw new Exception("Statistic not found");

            return statistic.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<IEnumerable<StatisticDto>, Exception>> AllStatisticsAsync()
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var statistics = GetAllStatistics(context) ?? throw new Exception("Statistic not found");
            return await statistics.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<IEnumerable<StatisticDto>, Exception>> AllPublicStatisticsAsync()
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var statistics = GetAllPublicStatistics(context) ?? throw new Exception("Statistic not found");
            return await statistics.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<StatisticDto, Exception>> CreateStatisticAsync(CreateStatisticDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Name.Length == 0) throw new Exception("Name is required");
            if (dto.Description.Length == 0) throw new Exception("Description is required");
            
            var statistic = new Statistic()
            {
                Name = dto.Name,
                Description = dto.Description,
                Query = dto.Query,
                Columns = dto.Columns,
                Rows = dto.Rows,
                Unit = dto.Unit,
                IsPublic = dto.IsPublic
            };

            await using var context = await factory.CreateDbContextAsync(ct);
            await context.Statistics.AddAsync(statistic, ct);
            await context.SaveChangesAsync(ct);
            
            return statistic.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateStatisticAsync(int statisticId, UpdateStatisticDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Name.Length == 0) throw new Exception("Name is required");
            if (dto.Description.Length == 0) throw new Exception("Description is required");
            if (dto.Query.Length == 0) throw new Exception("Query is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var statistic = await context.Statistics
                .AsTracking()
                .Where(x => x.Id == statisticId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Statistic not found");
            
            statistic.Name = dto.Name;
            statistic.Description = dto.Description;
            statistic.Query = dto.Query;
            statistic.Columns = dto.Columns;
            statistic.Rows = dto.Rows;
            statistic.Unit = dto.Unit;
            statistic.IsPublic = dto.IsPublic;

            context.Statistics.Update(statistic);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteStatisticAsync(int statisticId, CancellationToken ct)
    {
        try
        {
            if (statisticId == 0) throw new Exception("eventTypeId is required");
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var statistic = await context.Statistics
                .AsTracking()
                .Where(x => x.Id == statisticId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Statistic not found");

            context.Statistics.Remove(statistic);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<StatisticRenderDto, Exception>> ConvertJArrayToStatisticRenderAsync(JArray array, CancellationToken ct)
    {
        try
        {
            if (array is null) throw new Exception("array is required");

            await Task.Delay(0, ct);

            var statisticRender = new StatisticRenderDto();
            
            switch (array?.Count)
            {
                case > 1:
                {
                    var columns = array[0].Values().Select(v => v.ToString()).ToList();
                    switch (columns.Count)
                    {
                        case 1:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsOneItem.Add(t.Values().Select(v => v.ToString()).First());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsOneItem;
                            break;
                        case 2:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsTwoItems.Add(t.Values().Select(v => v.ToString()).Take(2).ToList());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsTwoItems;
                            break;
                        case  3:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsThreeItems.Add(t.Values().Select(v => v.ToString()).Take(3).ToList());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsThreeItems;
                            break;
                        case  4:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsFourItems.Add(t.Values().Select(v => v.ToString()).Take(4).ToList());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsFourItems;
                            break;
                        case 5:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsFiveItems.Add(t.Values().Select(v => v.ToString()).Take(5).ToList());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsFiveItems;
                            break;
                        case 6:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsSixItems.Add(t.Values().Select(v => v.ToString()).Take(6).ToList());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsSixItems;
                            break;
                        case >= 7:
                            foreach (var t in array)
                            {
                                statisticRender.ManyRowsSevenItems.Add(t.Values().Select(v => v.ToString()).Take(7).ToList());
                            }
                            statisticRender.ReturnType = StatisticRenderType.ManyRowsSevenItems;
                            break;
                    }

                    break;
                }
                case > 0:
                {
                    var columns = array[0].Values().Select(v => v.ToString()).ToList();
                    switch (columns.Count)
                    {
                        case 1:
                            statisticRender.SingleRowOneItemValue = array[0].Values().Select(v => v.ToString()).First();
                            statisticRender.ReturnType = StatisticRenderType.SingleRowOneItem;
                            break;
                        case 2:
                            statisticRender.SingleRowTwoItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().First());
                            statisticRender.SingleRowTwoItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().Skip(1).First());
                            statisticRender.ReturnType = StatisticRenderType.SingleRowTwoItems;
                            break;
                        case 3:
                            statisticRender.SingleRowThreeItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().First());
                            statisticRender.SingleRowThreeItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().Skip(1).First());
                            statisticRender.SingleRowThreeItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().Skip(2).First());
                            statisticRender.ReturnType = StatisticRenderType.SingleRowThreeItems;
                            break;
                        case >= 4:
                            statisticRender.SingleRowFourItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().First());
                            statisticRender.SingleRowFourItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().Skip(1).First());
                            statisticRender.SingleRowFourItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().Skip(2).First());
                            statisticRender.SingleRowFourItemsValue.Add(array[0].Values().Select(v => v.ToString()).ToList().Skip(3).First());
                            statisticRender.ReturnType = StatisticRenderType.SingleRowFourItems;
                            break;
                    }

                    break;
                }
                default:
                    statisticRender.ReturnType = StatisticRenderType.NoData;
                    break;
            }   

            return statisticRender;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    
    private static readonly Func<ApplicationDbContext, int, Task<Statistic?>> GetStatisticById = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int statisticId) => 
            context.Statistics
                .AsNoTracking()
                .TagWith("GetStatisticById")
                .FirstOrDefault(x => x.Id == statisticId));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<StatisticDto>> GetAllStatistics = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Statistics
                .AsNoTracking()
                .TagWith("GetAllStatistics")
                .OrderBy(x => x.Id)
                .Select(x => new StatisticDto()
                {
                    Id = x.Id, 
                    Name = x.Name, 
                    Description = x.Description, 
                    Query = x.Query,
                    Columns = x.Columns,
                    Rows = x.Rows,
                    Unit = x.Unit,
                    IsPublic = x.IsPublic
                }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<StatisticDto>> GetAllPublicStatistics = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Statistics
                .AsNoTracking()
                .TagWith("GetAllPublicStatistics")
                .Where(x => x.IsPublic == true)
                .OrderBy(x => x.Id)
                .Select(x => new StatisticDto()
                {
                    Id = x.Id, 
                    Name = x.Name, 
                    Description = x.Description, 
                    Query = x.Query,
                    Columns = x.Columns,
                    Rows = x.Rows,
                    Unit = x.Unit,
                    IsPublic = x.IsPublic
                }));
}