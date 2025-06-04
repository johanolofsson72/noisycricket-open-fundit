using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Applications.Services;
using Shared.Events.Services;
using Xunit;
using Xunit.Abstractions;

namespace Server.Test;

public class HybridCacheTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public HybridCacheTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _ = Database.InitializeTestDatabase(factory, factory.DataBaseName(), testOutputHelper);
    }
    
    [Fact]
    public async Task HardTestMicrosoftHybridCache()
    {
        using var scope = _factory.Services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<ApplicationService>();

        var applicationFirst = await applicationService.TestGetApplicationAsync(1, new CancellationToken());
        var titleFirst = applicationFirst.Title;

        var applicationSecond = await applicationService.TestGetApplicationAsync(1, new CancellationToken());
        var titleSecond = applicationSecond.Title;

        var titleNew = titleFirst + " Updated";
        var updated = await applicationService.TestUpdateApplicationAsync(1, titleNew, new CancellationToken());

        var applicationThird = await applicationService.TestGetApplicationAsync(1, new CancellationToken());
        var titleThird = applicationThird.Title;

        var applicationFourth = await applicationService.TestGetApplicationAsync(1, new CancellationToken());
        var titleFourth = applicationFourth.Title;
        
        _testOutputHelper.WriteLine($"First Title: {titleFirst}");
        _testOutputHelper.WriteLine($"Second Title: {titleSecond}");
        _testOutputHelper.WriteLine($"Third Title: {titleThird}");
        _testOutputHelper.WriteLine($"Fourth Title: {titleFourth}");
        
        Assert.True(updated);
        Assert.Equal(titleFirst, titleSecond);
        Assert.Equal(titleThird, titleFourth);
        Assert.NotEqual(titleFirst, titleThird);
        Assert.Equal(titleNew, titleThird);
    }
}