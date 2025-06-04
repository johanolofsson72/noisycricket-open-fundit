using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Applications.DTOs;
using Shared.Applications.Services;
using Xunit.Abstractions;

namespace Server.Test.Applications;

public class TestApplications : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public TestApplications(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task Create_Update_Delete_Application()
    {
        using var scope = _factory.Services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<ApplicationService>();
        
        _ = Database.InitializeTestDatabase(_factory, "fundit.db", _testOutputHelper);

        var applicationResult = await applicationService.CreateApplicationAsync(
            new CreateApplicationDto()
            {
                SchemaId = new Bogus.Randomizer().Number(1, 8),
                ApplicantId = 1,
                OrganizationId = 1,
                ParentId = 0,
                ProjectManagerId = 1
            }, new CancellationToken());
        
        Assert.True(applicationResult.IsOk);
        Assert.True(applicationResult.Value.Id > 0);

        var applicationsResult = await applicationService.AllApplicationsSummaryAsync();
        
        Assert.True(applicationsResult.IsOk);
        Assert.True(applicationsResult.Value.Count > 0);
        
        var applicationCount = applicationsResult.Value.Count;
        
        applicationResult = await applicationService.CreateApplicationAsync(
            new CreateApplicationDto()
            {
                SchemaId = new Bogus.Randomizer().Number(1, 8),
                ApplicantId = 2,
                OrganizationId = 2,
                ParentId = 0,
                ProjectManagerId = 2
            }, new CancellationToken());
        
        Assert.True(applicationResult.IsOk);
        Assert.True(applicationResult.Value.Id > 0);
        
        applicationsResult = await applicationService.AllApplicationsSummaryAsync();
        
        Assert.True(applicationsResult.IsOk);
        Assert.True(applicationsResult.Value.Count > 0);
        Assert.True(applicationsResult.Value.Count > applicationCount);

    }
}