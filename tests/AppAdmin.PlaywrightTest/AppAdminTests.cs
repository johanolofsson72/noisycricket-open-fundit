using System.Text.RegularExpressions;
using Blazorise;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Newtonsoft.Json;
using NUnit.Framework;
using Shared.Data.DbContext;

namespace AppAdmin.PlaywrightTest;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AppAdminTests : PageTest
{
    private List<string> _titles = new();
    private CustomWebApplicationFactory? _factory;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
    }
    
    public async Task Admin_Import_And_Create_Project()
    { 
        
        _factory = new CustomWebApplicationFactory();
        // Init
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        var url = Helper.AdminUrl;
        
        // Login
        await page.GotoAsync(url);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.AdminEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.AdminPassword);
        await page.Locator("#login-submit").ClickAsync();
        
        await page.GetByRole(AriaRole.Link, new() { Name = "Ansökningar", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Öppna" }).First.ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Begär Public 360 Id" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Uppdatera public 360 id" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Skapa projekt" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Link, new() { Name = "Projekt" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Öppna" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Logga ut" }).ClickAsync();
        
        await Task.Delay(8000);
        
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_And_Admin_Import_And_Create_Project()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var title = await GenerateMovieTitle();
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Svensk långfilm", Exact = true }).ClickAsync();
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(title);
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("Hållbar produktion I rutan").ClickAsync();
        await page.GetByLabel("Hållbar produktion I rutan").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Main).Locator("div").Filter(new() { HasText = "Ansökningar Skicka in ansökan" }).Nth(1).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Teamlista").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "teamlista.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Manus/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "manus.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Finansieringsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "finansieringsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Övriga dokument").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "other.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();

        // Waiting for aggregations
        await Task.Delay(8000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Logga ut" }).ClickAsync();
        
        // Change to admin
        await Task.Delay(1000);
        url = Helper.AdminUrl;
        
        // Login
        page = await context.NewPageAsync().ConfigureAwait(false);
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.AdminEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.AdminPassword);
        await page.Locator("#login-submit").ClickAsync();
        await Task.Delay(1000);*/
        
        await page.GetByRole(AriaRole.Link, new() { Name = "Ansökningar", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Öppna" }).First.ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Begär Public 360 Id" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Uppdatera public 360 id" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Skapa projekt" }).ClickAsync();
        await Task.Delay(3000);
        await page.GetByRole(AriaRole.Link, new() { Name = "Projekt" }).ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Öppna" }).First.ClickAsync();
        await Task.Delay(1000);
        await page.GetByRole(AriaRole.Button, new() { Name = "Logga ut" }).ClickAsync();
        
        // Waiting for aggregations
        await Task.Delay(8000);
        
        // Change to client
        await Task.Delay(1000);
        url = Helper.ClientUrl;
        
        // Login
        page = await context.NewPageAsync().ConfigureAwait(false);
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/
        
        // My projects
        await page.GetByRole(AriaRole.Link, new() { Name = "Mina projekt" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = title }).First.ClickAsync();
        await page.GetByLabel("Open").Nth(1).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = title }).Last.ClickAsync();
        /*await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(title + " 2");*/
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByLabel("Lokal valuta").FillAsync("SEK");
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();
        await page.GotoAsync("https://localhost:5078/projects");
        await page.GetByRole(AriaRole.Button, new() { Name = "Logga ut" }).ClickAsync();

        // Waiting for aggregations
        await Task.Delay(8000);
        
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Development_Drama_Serie()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Projektutveckling - Dramaserie", Exact = true }).ClickAsync();
        
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Company.CompanyName());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Synopsis/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "synopsis.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Beskrivning av vad").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "utvecklingsbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Utvecklingsbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "utvecklingsbudget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Spendbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "spendbudget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Tidsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "tidsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("CV på").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "cv.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(10));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Development_Documentary_Film()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Projektutveckling - Lång dokumentär", Exact = true }).ClickAsync();
        
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Company.CompanyName());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Synopsis/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "synopsis.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Beskrivning av vad").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "utvecklingsbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Utvecklingsbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "utvecklingsbudget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Spendbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "spendbudget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Tidsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "tidsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("CV på").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "cv.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(10));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Development_Swedish_Film()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Projektutveckling - Svensk långfilm", Exact = true }).ClickAsync();
        
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Company.CompanyName());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Synopsis/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "synopsis.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Beskrivning av vad").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "utvecklingsbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Utvecklingsbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "utvecklingsbudget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Spendbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "spendbudget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Tidsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "tidsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("CV på").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "cv.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(10));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Drama_Serie()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Dramaserie", Exact = true }).First.ClickAsync();
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("Hållbar produktion I rutan").ClickAsync();
        await page.GetByLabel("Hållbar produktion I rutan").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Main).Locator("div").Filter(new() { HasText = "Ansökningar Skicka in ansökan" }).Nth(1).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Teamlista").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "teamlista.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Manus/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "manus.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Finansieringsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "finansieringsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Övriga dokument").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "other.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Short()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Korta format", Exact = true }).First.ClickAsync();
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Nth(2).ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("Hållbar produktion I rutan").ClickAsync();
        await page.GetByLabel("Hållbar produktion I rutan").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByLabel("Beräknad längd").ClickAsync();
        await page.Locator("label").Filter(new() { HasText = "Beräknad längd" }).GetByLabel("Open").ClickAsync();
        await page.GetByLabel("Hour").GetByText("2", new() { Exact = true }).ClickAsync();
        await page.GetByText("30").Last.ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Set" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").ClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Manus/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "manus.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Finansieringsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "finansieringsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Budget inkl. spendbudget").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "budget.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Tidsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "tidsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Teamlista").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "teamlista.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Distributionsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "distributionsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Övriga dokument").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "other.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Swedish_Film()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Svensk långfilm", Exact = true }).ClickAsync();
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(1000);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(1000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("Hållbar produktion I rutan").ClickAsync();
        await page.GetByLabel("Hållbar produktion I rutan").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Main).Locator("div").Filter(new() { HasText = "Ansökningar Skicka in ansökan" }).Nth(1).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Teamlista").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "teamlista.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Manus/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "manus.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Finansieringsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "finansieringsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Övriga dokument").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "other.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();

        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    [Test]
    public async Task Client_Apply_For_Documentary()
    { 
        _factory = new CustomWebApplicationFactory();
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Dokumentär långfilm", Exact = true }).First.ClickAsync();
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Nth(2).ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        
        await Task.Delay(100);
        await page.GetByLabel("Female").Nth(0).ClickAsync();
        await Task.Delay(100);
        
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await Task.Delay(100);
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await Task.Delay(100);
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await Task.Delay(000);
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        
        await Task.Delay(100);
        await page.GetByLabel("Female").Nth(1).ClickAsync();
        await Task.Delay(100);
        
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        
        await Task.Delay(100);
        await page.GetByLabel("Female").Nth(2).ClickAsync();
        await Task.Delay(100);
        
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("Hållbar produktion I rutan").ClickAsync();
        await page.GetByLabel("Hållbar produktion I rutan").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByLabel("Beräknad längd").ClickAsync();
        await page.Locator("label").Filter(new() { HasText = "Beräknad längd" }).GetByLabel("Open").ClickAsync();
        await page.GetByLabel("Hour").GetByText("2", new() { Exact = true }).ClickAsync();
        await page.GetByText("30").Last.ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Set" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").ClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByText("Teamlista").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "teamlista.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Manus/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "manus.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Finansieringsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "finansieringsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Övriga dokument").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "other.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");
    }

    public async Task Client_Start_Apply_For_Swedish_Film_And_Quit_Continue_Apply_For_Swedish_Film_And_Finish()
    { 
        // Init
        var url = Helper.ClientUrl;
        var rnd = new Random();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new List<string> { "--start-maximized" },
            SlowMo = 500
        };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        await using var context = await browser.NewContextAsync(
            new BrowserNewContextOptions
            {
                ViewportSize = ViewportSize.NoViewport,
                IgnoreHTTPSErrors = true,
                StorageStatePath = Helper.AdminAuthPath
            }).ConfigureAwait(false);
        var page = await context.NewPageAsync().ConfigureAwait(false);
        
        // Login
        await page.GotoAsync(url);
        /*await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();*/

        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.GetByLabel("Scheman").GetByRole(AriaRole.Button, new() { Name = "Open" }).ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "Svensk långfilm", Exact = true }).ClickAsync();
        await page.GetByLabel("Titel").ClickAsync();
        await page.GetByLabel("Titel").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Tidigare titlar").ClickAsync();
        await page.GetByLabel("Tidigare titlar").FillAsync(await GenerateMovieTitle());
        await page.GetByLabel("Open").Last.ClickAsync();
        await page.GetByText("Sweden").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("E-post").ClickAsync();
        await page.GetByLabel("E-post").FillAsync(new Bogus.Faker().Person.Email.ToLower());
        await page.GetByLabel("Telefon", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Telefon", new() { Exact = true }).FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByRole(AriaRole.Group, new() { Name = "Producenter" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Produktionsbolag", new() { Exact = true }).FillAsync(new Bogus.Faker().Company.CompanyName());
        await page.GetByLabel("Produktionsbolagets telefon").ClickAsync();
        await page.GetByLabel("Produktionsbolagets telefon").FillAsync(rnd.Next(1000, 9000).ToString() + "-" + rnd.Next(100000, 900000).ToString());
        await page.GetByLabel("Kommentar och länkar till rö").ClickAsync();
        await page.GetByLabel("Kommentar och länkar till rö").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Regissörer" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").ClickAsync();
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByLabel("Namn").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByRole(AriaRole.Group, new() { Name = "Manusförfattare" }).GetByRole(AriaRole.Button).ClickAsync();
        await page.GetByLabel("Distributör").ClickAsync();
        await page.GetByLabel("Distributör").FillAsync(new Bogus.Faker().Person.FullName);
        await page.GetByLabel("Hållbar produktion I rutan").ClickAsync();
        await page.GetByLabel("Hållbar produktion I rutan").FillAsync(new Bogus.Faker().Lorem.Paragraph(5));
        await page.GetByRole(AriaRole.Main).Locator("div").Filter(new() { HasText = "Ansökningar Skicka in ansökan" }).Nth(1).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByLabel("Filmens totala budget").DblClickAsync();
        await page.GetByLabel("Filmens totala budget").FillAsync(new Bogus.Faker().Finance.Amount(1000000, 10000000, 0).ToString());
        await page.GetByLabel("Steg 2 av").GetByLabel("Open").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "​ SEK" }).First.ClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").DblClickAsync();
        await page.GetByLabel("Ansökningsbelopp SEK").FillAsync(new Bogus.Faker().Finance.Amount(100000, 1000000, 0).ToString());
        await page.GetByRole(AriaRole.Button, new() { Name = "Logga ut" }).ClickAsync();
        
        // Login
        await page.GetByPlaceholder("0101015566").ClickAsync();
        await page.GetByPlaceholder("0101015566").FillAsync(Helper.ClientVat);
        await page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(Helper.ClientEmail);
        await page.GetByPlaceholder("password").ClickAsync();
        await page.GetByPlaceholder("password").FillAsync(Helper.ClientPassword);
        await page.Locator("#login-submit").ClickAsync();
        
        // Action
        await page.GetByRole(AriaRole.Link, new() { Name = "Ny ansökan" }).ClickAsync();
        await page.Locator("#p2_selectId > .k-input-value-text").ClickAsync();
        await page.GetByText("Svensk långfilm").First.ClickAsync();
        await page.GetByRole(AriaRole.Tab, new() { Name = "Budget" }).ClickAsync();
        await page.GetByRole(AriaRole.Tab, new() { Name = "Bifogade filer" }).ClickAsync();
        await page.GetByText("Teamlista").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "teamlista.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Manus/Treatment").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "manus.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Finansieringsplan").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "finansieringsplan.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Projektbeskrivning").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "projektbeskrivning.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByText("Övriga dokument").Locator("input[name=\"files\"]").First.SetInputFilesAsync(new FilePayload{ Name = "other.txt", MimeType = "text/plain", Buffer = System.Text.Encoding.UTF8.GetBytes(new Bogus.Faker().Lorem.Paragraph(10))});
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").ClickAsync();
        await page.GetByLabel("Kortsynopsis Max 600 tecken.").FillAsync(new Bogus.Faker().Lorem.Paragraph(3));
        await page.GetByRole(AriaRole.Button, new() { Name = "Nästa" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Klar" }).ClickAsync();


        await Task.Delay(8000);
        await page.GotoAsync(url);
        await Expect(Page).ToHaveURLAsync("about:blank");

    }

    
    
    private async Task<string> GenerateMovieTitle()
    {
        using var scope = _factory!.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        string title = NewTitle();

        while (_titles.Contains(title) || await context.Applications.AnyAsync(a => a.Title == title))
        {
            title = NewTitle();
        }

        _titles.Add(title);
        return title;
    }

    private string NewTitle()
    {
        var catalog = JsonConvert.DeserializeObject<MovieCatalog>(File.ReadAllText("/Users/jool/repos/noisycricket-fundit/Solution/tests/AppAdmin.PlaywrightTest/movie_list.json"));
        var rnd = new Random();
        if (catalog != null)
        {
            var index = rnd.Next(catalog.movies.Count);
            var titel = catalog.movies[index].title;
            return titel;
        }
        return "My Movie";
    }
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            ColorScheme = ColorScheme.Light,
            ViewportSize = new()
            {
                Width = 1920,
                Height = 1080
            },
            BaseURL = "https://localhost:5077/",
        };
    }
    
    public async Task AppAdmin_Show_Login_Page()
    {
        //await Page.GotoAsync("https://localhost:5077/Account/Login?ReturnUrl=%2F");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("luke.skywalker@funditbyus.com");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Zrtv6Pk!");
        await Page.Locator("#login-submit").ClickAsync();
        
        //await Expect(Page).ToHaveTitleAsync(new Regex("Hem"));
    }

    public async Task GetStartedLink()
    {
        await Page.GotoAsync("https://playwright.dev");

        // Click the get started link.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Get started" }).ClickAsync();

        // Expects page to have a heading with the name of Installation.
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" })).ToBeVisibleAsync();
    } 
}