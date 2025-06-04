using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server.Test.Helpers;
using Shared.Applications.DTOs;
using Shared.Data.DbContext;
using Shared.Events.Services;
using Shared.Global.Structs;
using Xunit.Abstractions;

namespace Server.Test;

public class TestEventChain : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public TestEventChain(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _ = Database.InitializeTestDatabase(factory, "fundit.db", testOutputHelper);
    }
    
    [Fact]
    public async Task Schema_1_3_4_5_Film_Serie_Documentary_International()
    {
        using var scope = _factory.Services.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<EventService>();
        
        // Item1: Schema/ApplicationId
        // Item2: Handläggarens svar
        // Item3: Produktionsgruppens svar
        var list = new List<Tuple<int, int, int>>() { new(1, 2, 1), new(3, 1, 2), new(4, 2, 2), new(5, 1, 1) };

        foreach (var item in list)
        {
            // Event 1: Ansökan görs av Producent eller någon hos produktionsbolaget
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 1);
            
            // Event 2: PK klickar på knappen "begär public 360 id"
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 2);
    
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 3);
        
            // Event 4: PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 4);

            // Event 5: När komplett-knappen på översikt trycks
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 5);
        
            // Event 6: När handläggarens bedömning låses
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 6);

            if (item.Item2 == 1)
            {
                // Event 7: Nej på handläggarens bedömning
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 7, item.Item2, true);
            }
            else
            {
                // Event 8: Ja på handläggarens bedömning
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 8, item.Item2, true);
            }

            if (item.Item3 == 1)
            {
                // Event 9: Nej på produktionsgruppens bedömning
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 9, item.Item3);
                
                // Event 10: Efter att avslagsbrev skickats till producent
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 10);
                
            }
            else
            {
                // Event 11: Ja på produktionsgruppens bedömning
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 11, item.Item3);
                
                // Event 12: När 360-möte är satt som bokat på översikten
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 12);
                
                // Event 13: När loc/loi skickas
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 13);
                
                // Event 14: När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 14);
                
                // Event 15: Om någon klickar på knappen Färdigfinansierat under översikt
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 15);
                
                // Event 16: När handläggaren låser beslutsunderlaget.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 16);
                
                // Event 17: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 17);
                
                // Event 18: AA klickar på knappen Avtalsprocess klar under översikt
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 18);
                
                // Event 19: När VD:n tar bort meddelandet om beslutsunderlag.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 19);
                
                // Event 20: Producenten väljer datum för inspelningsstart på projektet
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 20);
                
                // Event 21: När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 21);
                
                // Event 22: När ansvarig kryssat i "Rat 1 klar".
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 22);
                
                // Event 23: Producenten väljer datum för inspelningsslut på projektet
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 23);
                
                // Event 24: När ansvarig kryssat i "Rat 2 klar".
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 24);
                
                // Event 25: När kryssrutan "Rough Cut Klar" är ikryssad
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 25);
                
                // Event 26: När kryssrutan "Final cut / DCP Kopia klar" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 26);
                
                // Event 27: När ansvarig kryssat i "Spendredovisning godkänd" under översikt.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 27);
                
                // Event 28: När producenten meddelar premiärdatum i Klient-verktyget och klickar på spara
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 28);
                
                // Event 29: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 29);
                
                // Event 30: När ansvarig kryssat i "Slutredovisning godkänd" under översikt.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 30);
                
                // Event 31: När PRK eller PR klickar i "PR material mottaget"
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 31);
                
                // Event 32: När ansvarig kryssat i "Projekt avklarat" under översikt.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 32);
                
                // Event 33: När datumet för DCP är redan passerat.
                await TriggerStandaloneNextEvent(eventService, item.Item1, 33);
                
                // Event 34: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
                await TriggerStandaloneNextEvent(eventService, item.Item1, 34);
            }
        }
    }

    private async Task ProcessFilmSerieDocumentaryEventChain(EventService eventService, int schemaOrApplicationId, int eventId, int? managerAnswer = null, bool disableDeleteOfAlternativeChain = false)
    {
        Result<ApplicationEventDto, Exception> ch;
        Result<bool, Exception> ev;
        ch = await eventService.CheckNextEvent(schemaOrApplicationId, new CancellationToken(), managerAnswer);
        if (!ch.IsOk) _testOutputHelper.WriteLine(ch.Error.Message);
        Assert.True(ch.IsOk);
        if (eventId == 1) Assert.True(ch.Value.IsFirstInChain);
        Assert.True(ch.Value.ApplicationEventIdentifier == eventId);
        ev = await eventService.TriggerNextEvent(schemaOrApplicationId, new CancellationToken(), managerAnswer, disableDeleteOfAlternativeChain);
        if (!ev.IsOk) _testOutputHelper.WriteLine(ev.Error.Message);
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
    public async Task Schema_6_7_8_DevFilm_DevSerie_DevDocumentary()
    {
        using var scope = _factory.Services.CreateScope();
        var eventService = scope.ServiceProvider.GetRequiredService<EventService>();
        
        // Item1: Schema/ApplicationId
        // Item2: Handläggarens svar
        var list = new List<Tuple<int, int>>() { new(6, 2), new(7, 1), new(8, 2) };

        foreach (var item in list)
        {
            // Event: 1 Ansökan görs av Producent eller någon hos produktionsbolaget
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 1);
            
            // Event: 2 PK klickar på knappen "begär public 360 id"
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 2);
            
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 3);
            
            // Event 4: PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 4);
            
            // Event 5: När komplettknappen på översikt trycks
            await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 5);
            
            if (item.Item2 == 1)
            {
                // Event 6: När handläggarens bedömning låses med nej.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 6, 1);
                
                // Event 8: Efter att avslagsbrev skickats till producent
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 8);
            }
            else
            {
                // Event 7: När handläggarens bedömning låses med ja.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 7, 2);
                
                // Event 9: Om någon klickar på knappen Färdigfinansierat  under översikt
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 9);
                
                // Event 10: AA klickar på knappen Avtalsprocess klar under översikt
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 10);
                
                // Event 11: Producenten skickar in ekonomisk och konstnärlig redovisning.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 11);
                
                // Event 12: När kryssrutorna "Ekonomisk redovisning klar" och "Konstnärlig redovisning" kryssas i.
                await ProcessFilmSerieDocumentaryEventChain(eventService, item.Item1, 12);
                
                // Event 13: När datumen för redovisning har passerat.
                await TriggerStandaloneNextEvent(eventService, item.Item1, 13);
            }
        }
    }
}