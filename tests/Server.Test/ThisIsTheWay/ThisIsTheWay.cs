using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Global.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace Server.Test.ThisIsTheWay;

public class ThisIsTheWay : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;   
    private readonly ITestOutputHelper _testOutputHelper;


    public ThisIsTheWay(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _ = Database.InitializeTestDatabase(factory, "fundit.db", testOutputHelper);
    }
    
    /*[Fact]*/
    public async Task Goo()
    {
        using var scope = _factory.Services.CreateScope();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("api");
        
        // Client: Skapa en publik användare
        
        // Admin:Godkänna den nya användaren
        
        // Client: Skapa en ansökan
        
        // Client: Fortsätta med ansökan
        
        // Client: Skicka in ansökan
        
        // Admin: Godkänna ansökan och koppla den till ett projekt
        
        // Admin: Gå igenom hela eventkedjan
        
        var applicationSummary = await GetApplicationsSummary(httpClient);
        
    }
    
    private async Task<List<SummaryDto>> GetApplicationsSummary(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync("api/v1/applications/summary");
        var result = await response.Content.ReadFromJsonAsync<List<SummaryDto>>();
        
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        
        return result;
    }
}