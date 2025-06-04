using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Applications.DTOs;
using Shared.Events.Services;
using Shared.Global.Structs;
using Xunit.Abstractions;

namespace Server.Test;

public class TestEventChainForShortAlternative1 : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public TestEventChainForShortAlternative1(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _ = Database.InitializeTestDatabase(factory, "fundit.db", testOutputHelper);
    }
    
    private static async Task ProcessShortFilmEventChain(EventService eventService, int schemaOrApplicationId, int eventId, int? managerAnswer = null, bool disableDeleteOfAlternativeChain = false)
    {
        Result<ApplicationEventDto, Exception> ch;
        Result<bool, Exception> ev;
        ch = await eventService.CheckNextEvent(schemaOrApplicationId, new CancellationToken(), managerAnswer);
        Assert.True(ch.IsOk);
        if (eventId == 1) Assert.True(ch.Value.IsFirstInChain);
        Assert.True(ch.Value.ApplicationEventIdentifier == eventId);
        ev = await eventService.TriggerNextEvent(schemaOrApplicationId, new CancellationToken(), managerAnswer, disableDeleteOfAlternativeChain);
        Assert.True(ev.IsOk);
        Assert.True(ev.Value);
    }

    private async Task TriggerStandaloneNextEvent(EventService eventService, int schemaOrApplicationId, int eventId)
    {
        Result<bool, Exception> ev;
        ev = await eventService.TriggerStandAloneEvent(schemaOrApplicationId, eventId, new CancellationToken());
        if (!ev.IsOk) _testOutputHelper.WriteLine(ev.Error.Message);
        Assert.True(ev.IsOk);
        Assert.True(ev.Value);
    }
    
    [Fact]
    public async Task Schema_2_Short()
    {
        using var scope = _factory.Services.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<EventService>();
        
        // Item1: Schema/ApplicationId
        // Item2: Handläggarens svar
        // Item3: Produktionsgruppens svar
        var list = new List<Tuple<int, int, int>>() { new(2, 2, 1) };

        foreach (var item in list)
        {
            // Event: 1 Ansökan görs av Producent eller någon hos produktionsbolaget
            await ProcessShortFilmEventChain(eventService, item.Item1, 1);
        
            // Event: 2 PK klickar på knappen "begär public 360 id"
            await ProcessShortFilmEventChain(eventService, item.Item1, 2);
        
            // Event: 3 När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            await ProcessShortFilmEventChain(eventService, item.Item1, 3);
        
            // Event: 4 PK skapar projektet. PK fördelar projektet på KFA genom att ange detta under obehandlad söknad.
            await ProcessShortFilmEventChain(eventService, item.Item1, 4);
        
            // Event: 5 När komplettknappen på översikt trycks
            await ProcessShortFilmEventChain(eventService, item.Item1, 5);
        
            // Event: 6 När handläggarens bedömning låses
            await ProcessShortFilmEventChain(eventService, item.Item1, 6);

            if (item.Item2 == 1)
            {
                // Event: 7 Nej på handläggarens bedömning
                await ProcessShortFilmEventChain(eventService, item.Item1, 7, item.Item2, true);
            }
            else
            {
                // Event: 8 Ja på handläggarens bedömning
                await ProcessShortFilmEventChain(eventService, item.Item1, 8, item.Item2, true);
            }

            if (item.Item3 == 1)
            {
                // Event: 9 Nej på produktionsgruppens bedömning
                await ProcessShortFilmEventChain(eventService, item.Item1, 9, item.Item3);
                
                // Event: 10 Efter att avslagsbrev skickats till producent
                await ProcessShortFilmEventChain(eventService, item.Item1, 10);
            }
            else
            {
                // Event: 11 Ja på produktionsgruppens bedömning
                await ProcessShortFilmEventChain(eventService, item.Item1, 11, item.Item3);
                
                // Event: 12 När Loc/Loi skickats
                await ProcessShortFilmEventChain(eventService, item.Item1, 12);
                
                // Event: 13 När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfininasierat under översikten.
                await ProcessShortFilmEventChain(eventService, item.Item1, 13);
                
                // Event: 14 Om CFO eller någon annan klickar på knappen Färdigfinansierat  under översikt
                await ProcessShortFilmEventChain(eventService, item.Item1, 14);
                
                // Event: 15 AA klickar på knappen Avtalsprocess klar under översikt
                await ProcessShortFilmEventChain(eventService, item.Item1, 15);
                
                // Event: 16 När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
                await ProcessShortFilmEventChain(eventService, item.Item1, 16);
                
                // Event: 17 När ansvarig kryssat i "Rat 1 klar".
                await ProcessShortFilmEventChain(eventService, item.Item1, 17);
                
                // Event: 18 När ansvarig kryssat i "Godkänd arbetskopia klar" under översikt.
                await ProcessShortFilmEventChain(eventService, item.Item1, 18);
                
                // Event: 19 När producenten meddelar premiärdatum i Klientvektyget och klickar på spara
                await ProcessShortFilmEventChain(eventService, item.Item1, 19);
                
                // Event: 20 När ansvarig kryssat i "Rat 2 klar".
                await ProcessShortFilmEventChain(eventService, item.Item1, 20);
                
                // Event: 21 När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
                await TriggerStandaloneNextEvent(eventService, item.Item1, 21);
                
                // Event: 22 Vid godkänd spendredovisning och slutredovisning
                await ProcessShortFilmEventChain(eventService, item.Item1, 22);
                
                // Event: 23 När PRK eller PR klickar i "PR material mottaget"
                await ProcessShortFilmEventChain(eventService, item.Item1, 23);
                
                // Event: 24 När ansvarig kryssat i "Projekt avklarat" under översikt.
                await ProcessShortFilmEventChain(eventService, item.Item1, 24);
                
                // Event: 25 När ansvarig kryssat i "Projekt avklarat" under översikt utan att "PR material mottaget" har blivit klickat.
                await ProcessShortFilmEventChain(eventService, item.Item1, 25);
                
            }
        }
    }
    
}