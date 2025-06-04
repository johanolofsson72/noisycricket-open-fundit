using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Data.DbContext;
using Shared.Global.DTOs;
using Shared.Global.Structs;
using Shared.OpenAi.Entities;
using SmartComponents.LocalEmbeddings;

namespace Shared.OpenAi.Services;

public class OpenAiService(IDbContextFactory<ApplicationDbContext> factory, IHttpClientFactory httpClientFactory, IConfiguration configuration, LocalEmbedder embedder)
{
    private readonly string _openAiEndpoint = configuration.GetValue<string>("OpenAi:Endpoint")!;
    private List<(int ProjectId, EmbeddingF32 Embedding)>? index;
    
    public async Task<Result<OpenAiResultDto, Exception>> Translate(string query)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
            var cacheResult = await context.OpenAiCacheItems.FirstOrDefaultAsync(x => x.Query.ToLower() == query.ToLower());
            if (cacheResult != null)
            {
                cacheResult.ReturnCount++;
                await context.SaveChangesAsync();
                
                return new OpenAiResultDto()
                {
                    Query = query,
                    Result = cacheResult.Answer
                };
            }

            var client = httpClientFactory.CreateClient("openai");
            var response = await client.PostAsJsonAsync($"/api/v1/translateSql", new TranslateSql() { Question = query });
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();

            context.OpenAiCacheItems.Add(new OpenAiCache()
            {
                Query = query,
                Answer = result.Trim(),
                ReturnCount = 1,
                ExpireDate = DateTime.UtcNow.AddYears(50)
            });
            await context.SaveChangesAsync();
        
            return new OpenAiResultDto()
            {
                Query = query,
                Result = result.Trim()
            };
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<string> ChatPublic(string query)
    {
        try
        {
            var client = httpClientFactory.CreateClient("openai");
            var response = await client.PostAsJsonAsync($"/api/v1/chatpublic/", new ChatPublic() { Question = query });
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            
            return result.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }
    
    public async Task<string> ChatPrivate(string preInfo, string query)
    {
        try
        {
            var client = httpClientFactory.CreateClient("openai");
            var response = await client.PostAsJsonAsync($"/api/v1/chatprivate/", new ChatPrivate() { PreInfo = preInfo, Question = query});
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            
            return result.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string[]> SearchAsync(ApplicationDbContext context, string query)
    {
        index ??= await PrepareIndexAsync(context);
        
        var searchEmbedding = embedder.Embed<EmbeddingF32>(query);

        var closestIds = LocalEmbedder.FindClosest(searchEmbedding, index, 5).ToList();
        
        var projects = await context.OpenAiProjects
            .AsNoTracking()
            .Where(x => closestIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);
        
        return closestIds.Select(x => projects[x].ProjectTitle).ToArray();
    }
    
    private static Task<List<(int ProjectId, EmbeddingF32 Embedding)>> PrepareIndexAsync(ApplicationDbContext context)
    {
        var projects = context.OpenAiProjects
            .AsNoTracking()
            .Where(x => x.Embedding != null)
            .Select(x => new { x.Id, x.Embedding })
            .ToList();

        return Task.FromResult(projects.Select(x => (x.Id, new EmbeddingF32(x.Embedding))).ToList());
    }
}