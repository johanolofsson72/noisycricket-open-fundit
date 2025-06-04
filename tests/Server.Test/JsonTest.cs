using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Applications.Services;
using Xunit.Abstractions;

namespace Server.Test;

public class JsonTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _ = Database.InitializeTestDatabase(factory, factory.DataBaseName(), testOutputHelper);
    }
    
    [Fact]
    public async Task JsonTesting()
    {
        using var scope = _factory.Services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<ApplicationService>();

        await applicationService.TestJson();
    }
}