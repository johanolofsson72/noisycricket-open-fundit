
namespace BlazorWebAppAdmin.Test.Infrastructure;

public static class Helper
{
    public const string AdminUrl = "https://localhost:5077";
    //public const string AdminUrl = "https://your-admin-domain.com";
    public const string AdminAuthPath = "/path/to/your/test/auth/admin-user.json";
    public const string AdminEmail = "admin@yourdomain.com";
    public const string AdminPassword = "YOUR_TEST_PASSWORD";
    public const string ClientUrl = "https://localhost:5078";
    //public const string ClientUrl = "https://your-client-domain.com";
    public const string ClientVat = "YOUR_TEST_VAT_NUMBER";
    public const string ClientAuthPath = "/path/to/your/test/auth/client-user.json";
    public const string ClientEmail = "client@yourdomain.com";
    public const string ClientPassword = "YOUR_TEST_PASSWORD";
    public const string TestFile = "/path/to/your/test/files/test.txt";
}

public class Movie
{
    public int id { get; set; } = 0;
    public string title { get; set; } = string.Empty;
}

public class MovieCatalog
{
    public List<Movie> movies { get; set; } = [];
}
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? _host;

    public string ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress.ToString();
        }
    }

    /*protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = builder.Build();
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        _host = builder.Build();
        _host.Start();

         var server = _host.Services.GetRequiredService<IServer>();
         var addresses = server.Features.Get<IServerAddressesFeature>();

        ClientOptions.BaseAddress = addresses!.Addresses
            .Select(x => new Uri(x))
            .Last();
        
        testHost.Start();
        return testHost;
    }*/
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Bygg testservern
        var testHost = builder.Build();
    
        // Konfigurera Kestrel men använd en dynamisk port
        builder.ConfigureWebHost(webHostBuilder => 
        {
            webHostBuilder.UseKestrel()
                .UseUrls("http://127.0.0.1:0"); // ":0" låter OS välja en ledig port
        });

        _host = builder.Build();
        _host.Start();

        // Hämta den dynamiskt valda porten
        var server = _host.Services.GetRequiredService<IServer>();
        var addressesFeature = server.Features.Get<IServerAddressesFeature>();
    
        if (addressesFeature != null && addressesFeature.Addresses.Any())
        {
            ClientOptions.BaseAddress = addressesFeature.Addresses
                .Select(x => new Uri(x))
                .Last();
        }
    
        testHost.Start();
        return testHost;
    }

    
    protected override void Dispose(bool disposing)
    {
        _host?.Dispose();
    }

    private void EnsureServer()
    {
        if (_host is null)
        {
            // This forces WebApplicationFactory to bootstrap the server
            using var _ = CreateDefaultClient();
        }
    }
}