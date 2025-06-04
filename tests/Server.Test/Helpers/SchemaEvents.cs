using System.Collections.Generic;
using Shared.Schemas.Entities;

namespace Server.Test.Helpers;

public static class SchemaEvents
{
    public static List<SchemaEvent> CreateSchemaEvents(string schemaName)
    {
        var events = schemaName switch
        {
            "Svensk långfilm" => SwedishFeatureFilmSchemaEvents(),
            "Korta format" => ShortFeatureFilmEvents(),
            "Dramaserie" => DramaSeriesSchemaEvents(),
            "Internationell långfilm" => InternationalFeatureFilmSchemaEvents(),
            "Dokumentär långfilm" => DocumentaryFeatureFilmEvents(),
            "Projektutveckling - Svensk långfilm" => DevelopmentSwedishFeatureFilmSchemaEvents(),
            "Projektutveckling - Dramaserie" => DevelopmentDramaSeriesSchemaEvents(),
            "Projektutveckling - Lång dokumentär" => DevelopmentDocumentaryFeatureFilmEvents(),
            _ => SwedishFeatureFilmSchemaEvents()
        };

        return events;
    }

    private static List<SchemaEvent> SwedishFeatureFilmSchemaEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event 1: Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansök", "", "", "", "", "", "", ""], description: "Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event 2: PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description: "PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera public 360 id", "", "", "", "", "", "", ""], description: "När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplett-knappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett efter nödvändig komplettering är gjord", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses
            new SchemaEvent(6, 6, 8, 5, labels: ["Handläggaren ska nu skriva en bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses") 
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen. Om projektet verkar intressant - glöm inte boka in 360-möte med producenten."),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Skriv produktionsgruppens bedömning.")
                ]
            },
            // Event 7: Nej på handläggarens bedömning
            new SchemaEvent(7, 7, 134, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på handläggarens bedömning") // nej för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar inte vara intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 8: Ja på handläggarens bedömning
            new SchemaEvent(8, 8, 135, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description: "Ja på handläggarens bedömning") // Ja för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 9: Nej på produktionsgruppens bedömning
            new SchemaEvent(9, 9, 11, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på produktionsgruppens bedömning") // nej pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag."),
                    new SchemaEventActionDeleteMessage(2, 6, 2),
                    new SchemaEventActionDeleteMessage(3, 6, 3),
                    new SchemaEventActionDeleteMessage(4, 7, 1),
                    new SchemaEventActionDeleteMessage(5, 8, 1)
                ]
            },
            // Event 10: Efter att avslagsbrev skickats till producent
            new SchemaEvent(10, 10, 12, 9, labels: ["Skriv och skicka avslagsbrev till producent", "", "", "", "", "", "", ""], isLastInChain: true, description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 9,1)
                ]
            },
            // Event 11: Ja på produktionsgruppens bedömning
            new SchemaEvent(11, 11, 14, 6, labels: ["Produktionsgruppen svarar ja i sin bedömning", "", "", "", "", "", "", ""], description:"Ja på produktionsgruppens bedömning") // ja pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        7, 
                        "Skriv och skicka Loc/Loi för projektet där villkoren för investeringen klart framgår"),
                    new SchemaEventActionDeleteMessage(7, 6, 2),
                    new SchemaEventActionDeleteMessage(8, 6, 3),
                    new SchemaEventActionDeleteMessage(9, 7, 1),
                    new SchemaEventActionDeleteMessage(10, 8, 1)
                ]
            },
            // Event 12: När 360-möte är satt som bokat på översikten
            new SchemaEvent(12, 12, 15, 11, labels: ["Boka in 360-möte och ange här nedan", "", "", "", "", "", "", ""], description:"När 360-möte är satt som bokat på översikten")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        4, 
                        "360-möte är nu bokat.")
                ]
            },
            // Event 13: När loc/loi skickas
            new SchemaEvent(13, 13, 16, 12, labels: ["Skriv och skicka LOC/LOI till producenten", "", "", "", "", "", "", ""], description:"När loc/loi skickas")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        7, 
                        "Projektet är klart för Film i Väst Produktioner, se LOC/LOI."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        8, 
                        "LOC/LOI ska diarieföras för projektet. Ett mail har skickats ut av systemet med dokumentet bifogat."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst. Invänta besked från samtliga medfinansiärer innan avtal kan påbörjas, kontrollera status med producenten, klicka sedan på knappen Färdigfinansierat under projektöveriskt."),
                    new SchemaEventActionMessage(
                        6, 
                        22, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst."),
                    new SchemaEventActionDeleteMessage(7, 11, 5)
                ]
            },
            // Event 14: När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.
            new SchemaEvent(14, 14, 17, 13, labels: ["Nu är vi i produktionsbeslut tills någon sätter färdigfinansierat till klart", "", "", "", "", "", "", ""], description:"När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        4, 
                        "Skapa ett beslutsunderlag för projektet."),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 13, 5)
                ]
            },
            // Event 15: När handläggaren låser beslutsunderlaget.
            new SchemaEvent(15, 15, 19, 14, labels: ["Handläggaren ska skriva ett beslutsunderlag och därefter låsa detta", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        4, 
                        "Ett beslutsunderlag på projektet har skrivits. När samproduktionsavtalet för projektet är signerat, skicka vidare till VD för signering"),
                    new SchemaEventActionDeleteMessage(2, 14, 4)
                ]
            },
            // Event 16: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.
            new SchemaEvent(16, 16, 18, 15, labels: ["Avtalsprocess påbörjas, när processen är färdig sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"När handläggaren låser beslutsunderlaget.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Är allt material och dokument inskickat till FiV så som: Manus, Finansieringsplan, Budget, Regiavtal, FiV riktlinjer etc? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                ]
            },
            // Event 17: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(17, 17, 20, 16, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        3),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        4, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null, 
                        3),
                    new SchemaEventActionMessage(
                        9, 
                        15, 
                        4, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionMessage(
                        10, 
                        2, 
                        9, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionUpdateStatus(11, 9),
                    new SchemaEventActionDeleteMessage(12, 16, 2)
                ]
            },
            // Event 18: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(18, 18, 22, 17, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        1, 
                        "Beslutsunderlaget ska diarieföras för projektet. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring.")
                ]
            },
            // Event 19: När VD:n tar bort meddelandet om beslutsunderlag.
            new SchemaEvent(19, 19, 21, 17, labels: ["VD ska radera meddelandet gällande beslutsunderlaget", "", "", "", "", "", "", ""], description:"När VD:n tar bort meddelandet om beslutsunderlag.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 17, 3)
                ]
            },
            // Event 20: Producenten väljer datum för inspelningsstart på projektet
            new SchemaEvent(20, 20, 23, 18, labels: ["Producenten ska välja datum för inspelningsstart", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsstart på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsstart för projektet. Säkerställ att alla villkor för rat 1 är godkända."),
                    new SchemaEventActionDeleteMessage(6, 17, 8)
                ]
            },
            // Event 21: När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
            new SchemaEvent(21, 21, 24, 20, labels: ["Producenten ska skicka in teamlista", "", "", "", "", "", "", ""], description:"När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionDeleteMessage(3, 20, 2)
                ]
            },
            // Event 22: När ansvarig kryssat i "Rat 1 klar".
            new SchemaEvent(22, 22, 26, 21, labels: ["Sätt Rat 1 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 1 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        6, 
                        3, 
                        "Nu ska producenten ange inspelningsslut, ett meddelande gällande detta är skickat."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionDeleteMessage(7, 20, 5)
                ]
            },
            // Event 23: Producenten väljer datum för inspelningsslut på projektet
            new SchemaEvent(23, 23, 29, 22, labels: ["Producenten ska välja datum för inspelningsslut", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsslut på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsslut för projektet. Säkerställ att alla villkor för rat 2 är godkända."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionDeleteMessage(8, 22, 4)
                ]
            },
            // Event 24: När ansvarig kryssat i "Rat 2 klar".
            new SchemaEvent(24, 24, 28, 23, labels: ["Sätt Rat 2 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 2 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Producenten har fått ett meddelande gällande att de ska skicka in en rough cut för projektet. När rough cut inkommer kontrollera och godkänn den för projektet genom att kryssa i 'Rough cut godkänd' under projektöversikten."),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        7, 
                        6, 
                        3, 
                        "Glöm inte att följa klippningen mellan Rough Cut och slutversion för projektet, klicka i kryssrutan \"Rough Cut Klar\" när ni anser detta vara klart."),
                    new SchemaEventActionDeleteMessage(8, 23, 5),
                    new SchemaEventActionDeleteMessage(9, 23, 3)
                ]
            },
            // Event 25: När kryssrutan "Rough Cut Klar" är ikryssad
            new SchemaEvent(25, 25, 31, 24, labels: ["Sätt Rough Cut till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Rough Cut Klar\" är ikryssad")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Ett meddelande har skickats till producenten att meddela oss så snart DCP är klar, efter detta ska Final Cut / DCP Kopia klar klickas i under projektöversikt."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionUpdateStatus(4, 10),
                ]
            },
            // Event 26: När kryssrutan "Final cut / DCP Kopia klar" är ikryssad.
            new SchemaEvent(26, 26, 33, 25, labels: ["Sätt Final Cut/DCP Kopia till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionUpdateStatus(7, 11),
                    new SchemaEventActionDeleteMessage(8, 25, 1),
                    new SchemaEventActionDeleteMessage(9, 25, 2)
                ]
            },
            // Event 27: När ansvarig kryssat i "Spend-redovisning godkänd" under översikt.
            new SchemaEvent(27, 27, 34, 26, labels: ["Sätt spend-redovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Spendredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        3, 
                        "Alla villkor för rat 3 är nu godkända. Betala ut Rat 3 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        8, 
                        "Spend-redovisning ska diarieföras för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Ett meddelande gällande, premiärdatum för projektet har skickats till producenten, invänta informationen eller påminn om ingenting sker."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionUpdateStatus(8, 13),
                    new SchemaEventActionDeleteMessage(9, 28, 5)
                ]
            },
            // Event 28: När producenten meddelar premiärdatum i Klient-verktyget och klickar på spara
            new SchemaEvent(28, 28, 35, 27, labels: ["Producenten ska skicka in premiärdatum", "", "", "", "", "", "", ""], description:"När producenten meddelar premiärdatum i Klientvektyget och klickar på spara")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Premiärdatum projektet är skickat från producent"),
                    new SchemaEventActionMessage(
                        2, 
                        13, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        3, 
                        19, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet behöver ses över gällande PR och Planering där vi samlar information kring statistik, rapportering och hemsida. Vi behöver även publikstatistik, digital admission och festivalnärvaro samt priser, uppdatera projektet efterhand"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet är klart för slutredovisning, invänta slutredovisning och skicka vidare till diarieansvarig"),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 29: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(29, 29, 36, 28, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 30: När ansvarig kryssat i "Slutredovisning godkänd" under översikt.
            new SchemaEvent(30, 30, 38, 29, labels: ["Sätt slutredovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Slutredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Slutredovisning ska diarieföras för projekt. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        6,
                        "Slutredovisning är godkänd för projekt. Säkerställ att alla villkor för rat 4 är godkända."),
                    new SchemaEventActionUpdateStatus(5, 14),
                    new SchemaEventActionDeleteMessage(6, 32, 3)
                ]
            },
            // Event 31: När PRK eller PR klickar i "PR material mottaget"
            new SchemaEvent(31, 31, 40, 30, labels: ["Sätt PR material mottaget", "", "", "", "", "", "", ""], description:"När PRK eller PR klickar i \"PR material mottaget\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "PR material inkommit på projektet."),
                    new SchemaEventActionUpdateStatus(4, 15)
                ]
            },
            // Event 32: När ansvarig kryssat i "Projekt avklarat" under översikt.
            new SchemaEvent(32, 32, 42, 31, isLastInChain: true, labels: ["Sätt projekt till avklarat", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Projekt avklarat\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 4 är nu godkända. Betala ut Rat 4 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(4, 16),
                    new SchemaEventActionDeleteMessage(5, 33, 4)
                ]
            },
            // Event 33: När datumet för DCP är redan passerat.
            new SchemaEvent(33, 33, 33, 0, isStandAlone: true, labels: ["Projekt avklarat", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionDeleteMessage(7, 25, 1),
                    new SchemaEventActionDeleteMessage(8, 25, 2)
                ]
            },
            // Event 34: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(34, 29, 36, 0, isStandAlone: true, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            }
        };

        return events;
    }

    private static List<SchemaEvent> DramaSeriesSchemaEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event 1: Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansök", "", "", "", "", "", "", ""], description: "Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event 2: PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description: "PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera public 360 id", "", "", "", "", "", "", ""], description: "När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplett-knappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett efter nödvändig komplettering är gjord", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses
            new SchemaEvent(6, 6, 8, 5, labels: ["Handläggaren ska nu skriva en bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses") 
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen. Om projektet verkar intressant - glöm inte boka in 360-möte med producenten."),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Skriv produktionsgruppens bedömning.")
                ]
            },
            // Event 7: Nej på handläggarens bedömning
            new SchemaEvent(7, 7, 134, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på handläggarens bedömning") // nej för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar inte vara intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 8: Ja på handläggarens bedömning
            new SchemaEvent(8, 8, 135, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description: "Ja på handläggarens bedömning") // Ja för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 9: Nej på produktionsgruppens bedömning
            new SchemaEvent(9, 9, 11, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på produktionsgruppens bedömning") // nej pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag."),
                    new SchemaEventActionDeleteMessage(2, 6, 2),
                    new SchemaEventActionDeleteMessage(3, 6, 3),
                    new SchemaEventActionDeleteMessage(4, 7, 1),
                    new SchemaEventActionDeleteMessage(5, 8, 1)
                ]
            },
            // Event 10: Efter att avslagsbrev skickats till producent
            new SchemaEvent(10, 10, 12, 9, labels: ["Skriv och skicka avslagsbrev till producent", "", "", "", "", "", "", ""], isLastInChain: true, description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 9,1)
                ]
            },
            // Event 11: Ja på produktionsgruppens bedömning
            new SchemaEvent(11, 11, 14, 6, labels: ["Produktionsgruppen svarar ja i sin bedömning", "", "", "", "", "", "", ""], description:"Ja på produktionsgruppens bedömning") // ja pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        7, 
                        "Skriv och skicka Loc/Loi för projektet där villkoren för investeringen klart framgår"),
                    new SchemaEventActionDeleteMessage(7, 6, 2),
                    new SchemaEventActionDeleteMessage(8, 6, 3),
                    new SchemaEventActionDeleteMessage(9, 7, 1),
                    new SchemaEventActionDeleteMessage(10, 8, 1)
                ]
            },
            // Event 12: När 360-möte är satt som bokat på översikten
            new SchemaEvent(12, 12, 15, 11, labels: ["Boka in 360-möte och ange här nedan", "", "", "", "", "", "", ""], description:"När 360-möte är satt som bokat på översikten")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        4, 
                        "360-möte är nu bokat.")
                ]
            },
            // Event 13: När loc/loi skickas
            new SchemaEvent(13, 13, 16, 12, labels: ["Skriv och skicka LOC/LOI till producenten", "", "", "", "", "", "", ""], description:"När loc/loi skickas")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        7, 
                        "Projektet är klart för Film i Väst Produktioner, se LOC/LOI."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        8, 
                        "LOC/LOI ska diarieföras för projektet. Ett mail har skickats ut av systemet med dokumentet bifogat."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst. Invänta besked från samtliga medfinansiärer innan avtal kan påbörjas, kontrollera status med producenten, klicka sedan på knappen Färdigfinansierat under projektöveriskt."),
                    new SchemaEventActionMessage(
                        6, 
                        22, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst."),
                    new SchemaEventActionDeleteMessage(7, 11, 5)
                ]
            },
            // Event 14: När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.
            new SchemaEvent(14, 14, 17, 13, labels: ["Nu är vi i produktionsbeslut tills någon sätter färdigfinansierat till klart", "", "", "", "", "", "", ""], description:"När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        4, 
                        "Skapa ett beslutsunderlag för projektet."),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 13, 5)
                ]
            },
            // Event 15: Om någon klickar på knappen Färdigfinansierat under översikt
            new SchemaEvent(15, 15, 19, 14, labels: ["Handläggaren ska skriva ett beslutsunderlag och därefter låsa detta", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        4, 
                        "Ett beslutsunderlag på projektet har skrivits. När samproduktionsavtalet för projektet är signerat, skicka vidare till VD för signering"),
                    new SchemaEventActionDeleteMessage(2, 14, 4)
                ]
            },
            // Event 16: När handläggaren låser beslutsunderlaget.
            new SchemaEvent(16, 16, 18, 15, labels: ["Avtalsprocess påbörjas, när processen är färdig sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"När handläggaren låser beslutsunderlaget.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Är allt material och dokument inskickat till FiV så som: Manus, Finansieringsplan, Budget, Regiavtal, FiV riktlinjer etc? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                ]
            },
            // Event 17: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.
            new SchemaEvent(17, 17, 20, 16, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        3),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        4, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null, 
                        3),
                    new SchemaEventActionMessage(
                        9, 
                        15, 
                        4, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionMessage(
                        10, 
                        2, 
                        9, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionUpdateStatus(11, 9),
                    new SchemaEventActionDeleteMessage(12, 16, 2)
                ]
            },
            // Event 18: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(18, 18, 22, 17, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        1, 
                        "Beslutsunderlaget ska diarieföras för projektet. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring.")
                ]
            },
            // Event 19: När VD:n tar bort meddelandet om beslutsunderlag.
            new SchemaEvent(19, 19, 21, 17, labels: ["VD ska radera meddelandet gällande beslutsunderlaget", "", "", "", "", "", "", ""], description:"När VD:n tar bort meddelandet om beslutsunderlag.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 17, 3)
                ]
            },
            // Event 20: Producenten väljer datum för inspelningsstart på projektet
            new SchemaEvent(20, 20, 23, 18, labels: ["Producenten ska välja datum för inspelningsstart", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsstart på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsstart för projektet. Säkerställ att alla villkor för rat 1 är godkända."),
                    new SchemaEventActionDeleteMessage(6, 17, 8)
                ]
            },
            // Event 21: När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
            new SchemaEvent(21, 21, 24, 20, labels: ["Producenten ska skicka in teamlista", "", "", "", "", "", "", ""], description:"När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionDeleteMessage(3, 20, 2)
                ]
            },
            // Event 22: När ansvarig kryssat i "Rat 1 klar".
            new SchemaEvent(22, 22, 26, 21, labels: ["Sätt Rat 1 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 1 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        6, 
                        3, 
                        "Nu ska producenten ange inspelningsslut, ett meddelande gällande detta är skickat."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionDeleteMessage(7, 20, 5)
                ]
            },
            // Event 23: Producenten väljer datum för inspelningsslut på projektet
            new SchemaEvent(23, 23, 29, 22, labels: ["Producenten ska välja datum för inspelningsslut", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsslut på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsslut för projektet. Säkerställ att alla villkor för rat 2 är godkända."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionDeleteMessage(8, 22, 4)
                ]
            },
            // Event 24: När ansvarig kryssat i "Rat 2 klar".
            new SchemaEvent(24, 24, 28, 23, labels: ["Sätt Rat 2 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 2 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Producenten har fått ett meddelande gällande att de ska skicka in en rough cut för projektet. När rough cut inkommer kontrollera och godkänn den för projektet genom att kryssa i 'Rough cut godkänd' under projektöversikten."),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        7, 
                        6, 
                        3, 
                        "Glöm inte att följa klippningen mellan Rough Cut och slutversion för projektet, klicka i kryssrutan \"Rough Cut Klar\" när ni anser detta vara klart."),
                    new SchemaEventActionDeleteMessage(8, 23, 5),
                    new SchemaEventActionDeleteMessage(9, 23, 3)
                ]
            },
            // Event 25: När kryssrutan "Rough Cut Klar" är ikryssad
            new SchemaEvent(25, 25, 31, 24, labels: ["Sätt Rough Cut till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Rough Cut Klar\" är ikryssad")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Ett meddelande har skickats till producenten att meddela oss så snart DCP är klar, efter detta ska Final Cut / DCP Kopia klar klickas i under projektöversikt."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionUpdateStatus(4, 10),
                ]
            },
            // Event 26: När kryssrutan "Final cut / DCP Kopia klar" är ikryssad.
            new SchemaEvent(26, 26, 33, 25, labels: ["Sätt Final Cut/DCP Kopia till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionUpdateStatus(7, 11),
                    new SchemaEventActionDeleteMessage(8, 25, 1),
                    new SchemaEventActionDeleteMessage(9, 25, 2)
                ]
            },
            // Event 27: När ansvarig kryssat i "Spend-redovisning godkänd" under översikt.
            new SchemaEvent(27, 27, 34, 26, labels: ["Sätt spend-redovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Spendredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        3, 
                        "Alla villkor för rat 3 är nu godkända. Betala ut Rat 3 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        8, 
                        "Spend-redovisning ska diarieföras för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Ett meddelande gällande, premiärdatum för projektet har skickats till producenten, invänta informationen eller påminn om ingenting sker."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionUpdateStatus(8, 13),
                    new SchemaEventActionDeleteMessage(9, 28, 5)
                ]
            },
            // Event 28: När producenten meddelar premiärdatum i Klient-verktyget och klickar på spara
            new SchemaEvent(28, 28, 35, 27, labels: ["Producenten ska skicka in premiärdatum", "", "", "", "", "", "", ""], description:"När producenten meddelar premiärdatum i Klientvektyget och klickar på spara")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Premiärdatum projektet är skickat från producent"),
                    new SchemaEventActionMessage(
                        2, 
                        13, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        3, 
                        19, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet behöver ses över gällande PR och Planering där vi samlar information kring statistik, rapportering och hemsida. Vi behöver även publikstatistik, digital admission och festivalnärvaro samt priser, uppdatera projektet efterhand"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet är klart för slutredovisning, invänta slutredovisning och skicka vidare till diarieansvarig"),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 29: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(29, 29, 36, 28, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 30: När ansvarig kryssat i "Slutredovisning godkänd" under översikt.
            new SchemaEvent(30, 30, 38, 29, labels: ["Sätt slutredovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Slutredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Slutredovisning ska diarieföras för projekt. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        6,
                        "Slutredovisning är godkänd för projekt. Säkerställ att alla villkor för rat 4 är godkända."),
                    new SchemaEventActionUpdateStatus(5, 14),
                    new SchemaEventActionDeleteMessage(6, 32, 3)
                ]
            },
            // Event 31: När PRK eller PR klickar i "PR material mottaget"
            new SchemaEvent(31, 31, 40, 30, labels: ["Sätt PR material mottaget", "", "", "", "", "", "", ""], description:"När PRK eller PR klickar i \"PR material mottaget\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "PR material inkommit på projektet."),
                    new SchemaEventActionUpdateStatus(4, 15)
                ]
            },
            // Event 32: När ansvarig kryssat i "Projekt avklarat" under översikt.
            new SchemaEvent(32, 32, 42, 31, isLastInChain: true, labels: ["Sätt projekt till avklarat", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Projekt avklarat\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 4 är nu godkända. Betala ut Rat 4 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(4, 16),
                    new SchemaEventActionDeleteMessage(5, 33, 4)
                ]
            },
            // Event 33: När datumet för DCP är redan passerat.
            new SchemaEvent(33, 33, 33, 0, isStandAlone: true, labels: ["Projekt avklarat", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionDeleteMessage(7, 25, 1),
                    new SchemaEventActionDeleteMessage(8, 25, 2)
                ]
            },
            // Event 34: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(34, 29, 36, 0, isStandAlone: true, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            }
        };

        return events;
    }
    
    private static List<SchemaEvent> InternationalFeatureFilmSchemaEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event 1: Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansök", "", "", "", "", "", "", ""], description: "Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event 2: PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description: "PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera public 360 id", "", "", "", "", "", "", ""], description: "När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplett-knappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett efter nödvändig komplettering är gjord", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses
            new SchemaEvent(6, 6, 8, 5, labels: ["Handläggaren ska nu skriva en bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses") 
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen. Om projektet verkar intressant - glöm inte boka in 360-möte med producenten."),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Skriv produktionsgruppens bedömning.")
                ]
            },
            // Event 7: Nej på handläggarens bedömning
            new SchemaEvent(7, 7, 134, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på handläggarens bedömning") // nej för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar inte vara intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 8: Ja på handläggarens bedömning
            new SchemaEvent(8, 8, 135, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description: "Ja på handläggarens bedömning") // Ja för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 9: Nej på produktionsgruppens bedömning
            new SchemaEvent(9, 9, 11, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på produktionsgruppens bedömning") // nej pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag."),
                    new SchemaEventActionDeleteMessage(2, 6, 2),
                    new SchemaEventActionDeleteMessage(3, 6, 3),
                    new SchemaEventActionDeleteMessage(4, 7, 1),
                    new SchemaEventActionDeleteMessage(5, 8, 1)
                ]
            },
            // Event 10: Efter att avslagsbrev skickats till producent
            new SchemaEvent(10, 10, 12, 9, labels: ["Skriv och skicka avslagsbrev till producent", "", "", "", "", "", "", ""], isLastInChain: true, description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 9,1)
                ]
            },
            // Event 11: Ja på produktionsgruppens bedömning
            new SchemaEvent(11, 11, 14, 6, labels: ["Produktionsgruppen svarar ja i sin bedömning", "", "", "", "", "", "", ""], description:"Ja på produktionsgruppens bedömning") // ja pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        7, 
                        "Skriv och skicka Loc/Loi för projektet där villkoren för investeringen klart framgår"),
                    new SchemaEventActionDeleteMessage(7, 6, 2),
                    new SchemaEventActionDeleteMessage(8, 6, 3),
                    new SchemaEventActionDeleteMessage(9, 7, 1),
                    new SchemaEventActionDeleteMessage(10, 8, 1)
                ]
            },
            // Event 12: När 360-möte är satt som bokat på översikten
            new SchemaEvent(12, 12, 15, 11, labels: ["Boka in 360-möte och ange här nedan", "", "", "", "", "", "", ""], description:"När 360-möte är satt som bokat på översikten")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        4, 
                        "360-möte är nu bokat.")
                ]
            },
            // Event 13: När loc/loi skickas
            new SchemaEvent(13, 13, 16, 12, labels: ["Skriv och skicka LOC/LOI till producenten", "", "", "", "", "", "", ""], description:"När loc/loi skickas")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        7, 
                        "Projektet är klart för Film i Väst Produktioner, se LOC/LOI."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        8, 
                        "LOC/LOI ska diarieföras för projektet. Ett mail har skickats ut av systemet med dokumentet bifogat."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst. Invänta besked från samtliga medfinansiärer innan avtal kan påbörjas, kontrollera status med producenten, klicka sedan på knappen Färdigfinansierat under projektöveriskt."),
                    new SchemaEventActionMessage(
                        6, 
                        22, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst."),
                    new SchemaEventActionDeleteMessage(7, 11, 5)
                ]
            },
            // Event 14: När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.
            new SchemaEvent(14, 14, 17, 13, labels: ["Nu är vi i produktionsbeslut tills någon sätter färdigfinansierat till klart", "", "", "", "", "", "", ""], description:"När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        4, 
                        "Skapa ett beslutsunderlag för projektet."),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 13, 5)
                ]
            },
            // Event 15: Om någon klickar på knappen Färdigfinansierat under översikt
            new SchemaEvent(15, 15, 19, 14, labels: ["Handläggaren ska skriva ett beslutsunderlag och därefter låsa detta", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        4, 
                        "Ett beslutsunderlag på projektet har skrivits. När samproduktionsavtalet för projektet är signerat, skicka vidare till VD för signering"),
                    new SchemaEventActionDeleteMessage(2, 14, 4)
                ]
            },
            // Event 16: När handläggaren låser beslutsunderlaget.
            new SchemaEvent(16, 16, 18, 15, labels: ["Avtalsprocess påbörjas, när processen är färdig sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"När handläggaren låser beslutsunderlaget.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Är allt material och dokument inskickat till FiV så som: Manus, Finansieringsplan, Budget, Regiavtal, FiV riktlinjer etc? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                ]
            },
            // Event 17: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.
            new SchemaEvent(17, 17, 20, 16, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        3),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        4, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null, 
                        3),
                    new SchemaEventActionMessage(
                        9, 
                        15, 
                        4, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionMessage(
                        10, 
                        2, 
                        9, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionUpdateStatus(11, 9),
                    new SchemaEventActionDeleteMessage(12, 16, 2)
                ]
            },
            // Event 18: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(18, 18, 22, 17, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        1, 
                        "Beslutsunderlaget ska diarieföras för projektet. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring.")
                ]
            },
            // Event 19: När VD:n tar bort meddelandet om beslutsunderlag.
            new SchemaEvent(19, 19, 21, 17, labels: ["VD ska radera meddelandet gällande beslutsunderlaget", "", "", "", "", "", "", ""], description:"När VD:n tar bort meddelandet om beslutsunderlag.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 17, 3)
                ]
            },
            // Event 20: Producenten väljer datum för inspelningsstart på projektet
            new SchemaEvent(20, 20, 23, 18, labels: ["Producenten ska välja datum för inspelningsstart", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsstart på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsstart för projektet. Säkerställ att alla villkor för rat 1 är godkända."),
                    new SchemaEventActionDeleteMessage(6, 17, 8)
                ]
            },
            // Event 21: När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
            new SchemaEvent(21, 21, 24, 20, labels: ["Producenten ska skicka in teamlista", "", "", "", "", "", "", ""], description:"När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionDeleteMessage(3, 20, 2)
                ]
            },
            // Event 22: När ansvarig kryssat i "Rat 1 klar".
            new SchemaEvent(22, 22, 26, 21, labels: ["Sätt Rat 1 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 1 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        6, 
                        3, 
                        "Nu ska producenten ange inspelningsslut, ett meddelande gällande detta är skickat."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionDeleteMessage(7, 20, 5)
                ]
            },
            // Event 23: Producenten väljer datum för inspelningsslut på projektet
            new SchemaEvent(23, 23, 29, 22, labels: ["Producenten ska välja datum för inspelningsslut", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsslut på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsslut för projektet. Säkerställ att alla villkor för rat 2 är godkända."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionDeleteMessage(8, 22, 4)
                ]
            },
            // Event 24: När ansvarig kryssat i "Rat 2 klar".
            new SchemaEvent(24, 24, 28, 23, labels: ["Sätt Rat 2 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 2 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Producenten har fått ett meddelande gällande att de ska skicka in en rough cut för projektet. När rough cut inkommer kontrollera och godkänn den för projektet genom att kryssa i 'Rough cut godkänd' under projektöversikten."),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        7, 
                        6, 
                        3, 
                        "Glöm inte att följa klippningen mellan Rough Cut och slutversion för projektet, klicka i kryssrutan \"Rough Cut Klar\" när ni anser detta vara klart."),
                    new SchemaEventActionDeleteMessage(8, 23, 5),
                    new SchemaEventActionDeleteMessage(9, 23, 3)
                ]
            },
            // Event 25: När kryssrutan "Rough Cut Klar" är ikryssad
            new SchemaEvent(25, 25, 31, 24, labels: ["Sätt Rough Cut till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Rough Cut Klar\" är ikryssad")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Ett meddelande har skickats till producenten att meddela oss så snart DCP är klar, efter detta ska Final Cut / DCP Kopia klar klickas i under projektöversikt."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionUpdateStatus(4, 10),
                ]
            },
            // Event 26: När kryssrutan "Final cut / DCP Kopia klar" är ikryssad.
            new SchemaEvent(26, 26, 33, 25, labels: ["Sätt Final Cut/DCP Kopia till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionUpdateStatus(7, 11),
                    new SchemaEventActionDeleteMessage(8, 25, 1),
                    new SchemaEventActionDeleteMessage(9, 25, 2)
                ]
            },
            // Event 27: När ansvarig kryssat i "Spend-redovisning godkänd" under översikt.
            new SchemaEvent(27, 27, 34, 26, labels: ["Sätt spend-redovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Spendredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        3, 
                        "Alla villkor för rat 3 är nu godkända. Betala ut Rat 3 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        8, 
                        "Spend-redovisning ska diarieföras för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Ett meddelande gällande, premiärdatum för projektet har skickats till producenten, invänta informationen eller påminn om ingenting sker."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionUpdateStatus(8, 13),
                    new SchemaEventActionDeleteMessage(9, 28, 5)
                ]
            },
            // Event 28: När producenten meddelar premiärdatum i Klient-verktyget och klickar på spara
            new SchemaEvent(28, 28, 35, 27, labels: ["Producenten ska skicka in premiärdatum", "", "", "", "", "", "", ""], description:"När producenten meddelar premiärdatum i Klientvektyget och klickar på spara")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Premiärdatum projektet är skickat från producent"),
                    new SchemaEventActionMessage(
                        2, 
                        13, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        3, 
                        19, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet behöver ses över gällande PR och Planering där vi samlar information kring statistik, rapportering och hemsida. Vi behöver även publikstatistik, digital admission och festivalnärvaro samt priser, uppdatera projektet efterhand"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet är klart för slutredovisning, invänta slutredovisning och skicka vidare till diarieansvarig"),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 29: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(29, 29, 36, 28, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 30: När ansvarig kryssat i "Slutredovisning godkänd" under översikt.
            new SchemaEvent(30, 30, 38, 29, labels: ["Sätt slutredovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Slutredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Slutredovisning ska diarieföras för projekt. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        6,
                        "Slutredovisning är godkänd för projekt. Säkerställ att alla villkor för rat 4 är godkända."),
                    new SchemaEventActionUpdateStatus(5, 14),
                    new SchemaEventActionDeleteMessage(6, 32, 3)
                ]
            },
            // Event 31: När PRK eller PR klickar i "PR material mottaget"
            new SchemaEvent(31, 31, 40, 30, labels: ["Sätt PR material mottaget", "", "", "", "", "", "", ""], description:"När PRK eller PR klickar i \"PR material mottaget\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "PR material inkommit på projektet."),
                    new SchemaEventActionUpdateStatus(4, 15)
                ]
            },
            // Event 32: När ansvarig kryssat i "Projekt avklarat" under översikt.
            new SchemaEvent(32, 32, 42, 31, isLastInChain: true, labels: ["Sätt projekt till avklarat", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Projekt avklarat\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 4 är nu godkända. Betala ut Rat 4 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(4, 16),
                    new SchemaEventActionDeleteMessage(5, 33, 4)
                ]
            },
            // Event 33: När datumet för DCP är redan passerat.
            new SchemaEvent(33, 33, 33, 0, isStandAlone: true, labels: ["Projekt avklarat", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionDeleteMessage(7, 25, 1),
                    new SchemaEventActionDeleteMessage(8, 25, 2)
                ]
            },
            // Event 34: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(34, 29, 36, 0, isStandAlone: true, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            }
        };

        return events;
    }
    
    private static List<SchemaEvent> DocumentaryFeatureFilmEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event 1: Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansök", "", "", "", "", "", "", ""], description: "Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event 2: PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description: "PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera public 360 id", "", "", "", "", "", "", ""], description: "När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK skapar projektet. PK fördelar projektet på SFA/IFA/DFA/TVA genom att ange detta under obehandlad söknad.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplett-knappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett efter nödvändig komplettering är gjord", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses
            new SchemaEvent(6, 6, 8, 5, labels: ["Handläggaren ska nu skriva en bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses") 
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen. Om projektet verkar intressant - glöm inte boka in 360-möte med producenten."),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Skriv produktionsgruppens bedömning.")
                ]
            },
            // Event 7: Nej på handläggarens bedömning
            new SchemaEvent(7, 7, 134, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på handläggarens bedömning") // nej för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar inte vara intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 8: Ja på handläggarens bedömning
            new SchemaEvent(8, 8, 135, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description: "Ja på handläggarens bedömning") // Ja för handläggare
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet verkar intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event 9: Nej på produktionsgruppens bedömning
            new SchemaEvent(9, 9, 11, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på produktionsgruppens bedömning") // nej pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag."),
                    new SchemaEventActionDeleteMessage(2, 6, 2),
                    new SchemaEventActionDeleteMessage(3, 6, 3),
                    new SchemaEventActionDeleteMessage(4, 7, 1),
                    new SchemaEventActionDeleteMessage(5, 8, 1)
                ]
            },
            // Event 10: Efter att avslagsbrev skickats till producent
            new SchemaEvent(10, 10, 12, 9, labels: ["Skriv och skicka avslagsbrev till producent", "", "", "", "", "", "", ""], isLastInChain: true, description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 9,1)
                ]
            },
            // Event 11: Ja på produktionsgruppens bedömning
            new SchemaEvent(11, 11, 14, 6, labels: ["Produktionsgruppen svarar ja i sin bedömning", "", "", "", "", "", "", ""], description:"Ja på produktionsgruppens bedömning") // ja pg
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        7, 
                        "Skriv och skicka Loc/Loi för projektet där villkoren för investeringen klart framgår"),
                    new SchemaEventActionDeleteMessage(7, 6, 2),
                    new SchemaEventActionDeleteMessage(8, 6, 3),
                    new SchemaEventActionDeleteMessage(9, 7, 1),
                    new SchemaEventActionDeleteMessage(10, 8, 1)
                ]
            },
            // Event 12: När 360-möte är satt som bokat på översikten
            new SchemaEvent(12, 12, 15, 11, labels: ["Boka in 360-möte och ange här nedan", "", "", "", "", "", "", ""], description:"När 360-möte är satt som bokat på översikten")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        4, 
                        "360-möte är nu bokat."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        4, 
                        "360-möte är nu bokat.")
                ]
            },
            // Event 13: När loc/loi skickas
            new SchemaEvent(13, 13, 16, 12, labels: ["Skriv och skicka LOC/LOI till producenten", "", "", "", "", "", "", ""], description:"När loc/loi skickas")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        7, 
                        "Projektet är klart för Film i Väst Produktioner, se LOC/LOI."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        8, 
                        "LOC/LOI ska diarieföras för projektet. Ett mail har skickats ut av systemet med dokumentet bifogat."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst. Invänta besked från samtliga medfinansiärer innan avtal kan påbörjas, kontrollera status med producenten, klicka sedan på knappen Färdigfinansierat under projektöveriskt."),
                    new SchemaEventActionMessage(
                        6, 
                        22, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst."),
                    new SchemaEventActionDeleteMessage(7, 11, 5)
                ]
            },
            // Event 14: När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.
            new SchemaEvent(14, 14, 17, 13, labels: ["Nu är vi i produktionsbeslut tills någon sätter färdigfinansierat till klart", "", "", "", "", "", "", ""], description:"När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat under översikten.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        4, 
                        "Skapa ett beslutsunderlag för projektet."),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 13, 5)
                ]
            },
            // Event 15: Om någon klickar på knappen Färdigfinansierat under översikt
            new SchemaEvent(15, 15, 19, 14, labels: ["Handläggaren ska skriva ett beslutsunderlag och därefter låsa detta", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        4, 
                        "Ett beslutsunderlag på projektet har skrivits. När samproduktionsavtalet för projektet är signerat, skicka vidare till VD för signering"),
                    new SchemaEventActionDeleteMessage(2, 14, 4)
                ]
            },
            // Event 16: När handläggaren låser beslutsunderlaget.
            new SchemaEvent(16, 16, 18, 15, labels: ["Avtalsprocess påbörjas, när processen är färdig sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"När handläggaren låser beslutsunderlaget.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Är allt material och dokument inskickat till FiV så som: Manus, Finansieringsplan, Budget, Regiavtal, FiV riktlinjer etc? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                ]
            },
            // Event 17: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.
            new SchemaEvent(17, 17, 20, 16, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        3),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        4, 
                        "Meddela när inspelningsstart inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null, 
                        3),
                    new SchemaEventActionMessage(
                        9, 
                        15, 
                        4, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionMessage(
                        10, 
                        2, 
                        9, 
                        "Det finns ett beslutsunderlag att signera."),
                    new SchemaEventActionUpdateStatus(11, 9),
                    new SchemaEventActionDeleteMessage(12, 16, 2)
                ]
            },
            // Event 18: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(18, 18, 22, 17, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        1, 
                        "Beslutsunderlaget ska diarieföras för projektet. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring.")
                ]
            },
            // Event 19: När VD:n tar bort meddelandet om beslutsunderlag.
            new SchemaEvent(19, 19, 21, 17, labels: ["VD ska radera meddelandet gällande beslutsunderlaget", "", "", "", "", "", "", ""], description:"När VD:n tar bort meddelandet om beslutsunderlag.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 17, 3)
                ]
            },
            // Event 20: Producenten väljer datum för inspelningsstart på projektet
            new SchemaEvent(20, 20, 23, 18, labels: ["Producenten ska välja datum för inspelningsstart", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsstart på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Inspelningsstart för projektet."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsstart för projektet. Säkerställ att alla villkor för rat 1 är godkända."),
                    new SchemaEventActionDeleteMessage(6, 17, 8)
                ]
            },
            // Event 21: När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
            new SchemaEvent(21, 21, 24, 20, labels: ["Producenten ska skicka in teamlista", "", "", "", "", "", "", ""], description:"När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionDeleteMessage(3, 20, 2)
                ]
            },
            // Event 22: När ansvarig kryssat i "Rat 1 klar".
            new SchemaEvent(22, 22, 26, 21, labels: ["Sätt Rat 1 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 1 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        6, 
                        3, 
                        "Nu ska producenten ange inspelningsslut, ett meddelande gällande detta är skickat."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Meddela när inspelningsslut inträffar för projektet här i onlineverktyget genom att klicka på utför.",
                        null,
                        4),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionDeleteMessage(7, 20, 5)
                ]
            },
            // Event 23: Producenten väljer datum för inspelningsslut på projektet
            new SchemaEvent(23, 23, 29, 22, labels: ["Producenten ska välja datum för inspelningsslut", "", "", "", "", "", "", ""], description:"Producenten väljer datum för inspelningsslut på projektet")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Inspelningsslut för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        6,
                        "Inspelningsslut för projektet. Säkerställ att alla villkor för rat 2 är godkända."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionDeleteMessage(8, 22, 4)
                ]
            },
            // Event 24: När ansvarig kryssat i "Rat 2 klar".
            new SchemaEvent(24, 24, 28, 23, labels: ["Sätt Rat 2 till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 2 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni skickar in första klippversionen för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Producenten har fått ett meddelande gällande att de ska skicka in en rough cut för projektet. När rough cut inkommer kontrollera och godkänn den för projektet genom att kryssa i 'Rough cut godkänd' under projektöversikten."),
                    new SchemaEventActionMessage(
                        6, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        7, 
                        6, 
                        3, 
                        "Glöm inte att följa klippningen mellan Rough Cut och slutversion för projektet, klicka i kryssrutan \"Rough Cut Klar\" när ni anser detta vara klart."),
                    new SchemaEventActionDeleteMessage(8, 23, 5),
                    new SchemaEventActionDeleteMessage(9, 23, 3)
                ]
            },
            // Event 25: När kryssrutan "Rough Cut Klar" är ikryssad
            new SchemaEvent(25, 25, 31, 24, labels: ["Sätt Rough Cut till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Rough Cut Klar\" är ikryssad")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Ett meddelande har skickats till producenten att meddela oss så snart DCP är klar, efter detta ska Final Cut / DCP Kopia klar klickas i under projektöversikt."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Film i Väst vill att ni meddelar datumet när DCP:n blev färdigställt."),
                    new SchemaEventActionUpdateStatus(4, 10),
                ]
            },
            // Event 26: När kryssrutan "Final cut / DCP Kopia klar" är ikryssad.
            new SchemaEvent(26, 26, 33, 25, labels: ["Sätt Final Cut/DCP Kopia till klar", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionUpdateStatus(7, 11),
                    new SchemaEventActionDeleteMessage(8, 25, 1),
                    new SchemaEventActionDeleteMessage(9, 25, 2)
                ]
            },
            // Event 27: När ansvarig kryssat i "Spend-redovisning godkänd" under översikt.
            new SchemaEvent(27, 27, 34, 26, labels: ["Sätt spend-redovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Spendredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        3, 
                        "Alla villkor för rat 3 är nu godkända. Betala ut Rat 3 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        8, 
                        "Spend-redovisning ska diarieföras för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionMessage(
                        5, 
                        6, 
                        3, 
                        "Ett meddelande gällande, premiärdatum för projektet har skickats till producenten, invänta informationen eller påminn om ingenting sker."),
                    new SchemaEventActionMessage(
                        6, 
                        21, 
                        1, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionEmail(
                        7, 
                        21, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionUpdateStatus(8, 13),
                    new SchemaEventActionDeleteMessage(9, 28, 5)
                ]
            },
            // Event 28: När producenten meddelar premiärdatum i Klient-verktyget och klickar på spara
            new SchemaEvent(28, 28, 35, 27, labels: ["Producenten ska skicka in premiärdatum", "", "", "", "", "", "", ""], description:"När producenten meddelar premiärdatum i Klientvektyget och klickar på spara")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Premiärdatum projektet är skickat från producent"),
                    new SchemaEventActionMessage(
                        2, 
                        13, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        3, 
                        19, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet behöver ses över gällande PR och Planering där vi samlar information kring statistik, rapportering och hemsida. Vi behöver även publikstatistik, digital admission och festivalnärvaro samt priser, uppdatera projektet efterhand"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet är klart för slutredovisning, invänta slutredovisning och skicka vidare till diarieansvarig"),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 29: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(29, 29, 36, 28, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            },
            // Event 30: När ansvarig kryssat i "Slutredovisning godkänd" under översikt.
            new SchemaEvent(30, 30, 38, 29, labels: ["Sätt slutredovisning till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Slutredovisning godkänd\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Slutredovisning ska diarieföras för projekt. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 4"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        6,
                        "Slutredovisning är godkänd för projekt. Säkerställ att alla villkor för rat 4 är godkända."),
                    new SchemaEventActionUpdateStatus(5, 14),
                    new SchemaEventActionDeleteMessage(6, 32, 3)
                ]
            },
            // Event 31: När PRK eller PR klickar i "PR material mottaget"
            new SchemaEvent(31, 31, 40, 30, labels: ["Sätt PR material mottaget", "", "", "", "", "", "", ""], description:"När PRK eller PR klickar i \"PR material mottaget\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "PR material inkommit på projektet."),
                    new SchemaEventActionUpdateStatus(4, 15)
                ]
            },
            // Event 32: När ansvarig kryssat i "Projekt avklarat" under översikt.
            new SchemaEvent(32, 32, 42, 31, isLastInChain: true, labels: ["Sätt projekt till avklarat", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Projekt avklarat\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Projektet har nu mottagit rat 4 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 4 är nu godkända. Betala ut Rat 4 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(4, 16),
                    new SchemaEventActionDeleteMessage(5, 33, 4)
                ]
            },
            // Event 33: När datumet för DCP är redan passerat.
            new SchemaEvent(33, 33, 33, 0, isStandAlone: true, labels: ["Projekt avklarat", "", "", "", "", "", "", ""], description:"När kryssrutan \"Final cut / DCP Kopia klar\" är ikryssad eller handläggaren öppnar projektet och datumet för DCP är redan passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Film i Väst har godkänt Final Cut/DCP för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig via dennes vanliga mail."),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Ett meddelande gällande, sammanställ av revisor godkänd spend-redovisning för projektet och skicka till avtalsansvarig för godkännande är skickat till producent, invänta materialet eller påminn producent om ingenting sker"),
                    new SchemaEventActionMessage(
                        6, 
                        15, 
                        3, 
                        "Final Cut / DCP för projektet har blivit godkänd."),
                    new SchemaEventActionDeleteMessage(7, 25, 1),
                    new SchemaEventActionDeleteMessage(8, 25, 2)
                ]
            },
            // Event 34: När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(34, 29, 36, 0, isStandAlone: true, labels: ["Nu avvaktar vi tills premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När premierdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionDeleteMessage(6, 29, 3),
                    new SchemaEventActionDeleteMessage(7, 29, 5)
                ]
            }
        };

        return events;
    }
    
    private static List<SchemaEvent> ShortFeatureFilmEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event: 1 Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansök", "", "", "", "", "", "", ""], description:"Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event: 2 PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär 360 id", "", "", "", "", "", "", ""], description:"PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event: 3 När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera 360 id", "", "", "", "", "", "", ""], description:"När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event: 4 PK skapar projektet. PK fördelar projektet på KFA genom att ange detta under obehandlad söknad.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK skapar projektet. PK fördelar projektet på KFA genom att ange detta under obehandlad söknad.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event: 5 När komplettknappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett efter nödvändig komplettering är gjord", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event: 6 När handläggarens bedömning låses
            new SchemaEvent(6, 6, 8, 5, labels: ["Handläggaren ska nu skriva en bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen. Om projektet verkar intressant - glöm inte boka in 360-möte med producenten."),
                    new SchemaEventActionMessage(
                        3, 
                        8, 
                        4, 
                        "Skriv produktionsgruppens bedömning.")
                ]
            },
            // Event: 7 Nej på handläggarens bedömning
            new SchemaEvent(7, 7, 134, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på handläggarens bedömning")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        3, 
                        "Projektet verkar inte vara intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event: 8 Ja på handläggarens bedömning
            new SchemaEvent(8, 8, 135, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Ja på handläggarens bedömning")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        3, 
                        "Projektet verkar intressant, ta upp detta under det kommande produktionsmötet.")
                ]
            },
            // Event: 9 Nej på produktionsgruppens bedömning
            new SchemaEvent(9, 9, 11, 6, labels: ["Produktionsgruppen ska skriva sin bedömning och därefter låsa den", "", "", "", "", "", "", ""], description:"Nej på produktionsgruppens bedömning")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag."),
                    new SchemaEventActionDeleteMessage(2, 6, 2),
                    new SchemaEventActionDeleteMessage(3, 6, 3),
                    new SchemaEventActionDeleteMessage(4, 7, 1),
                    new SchemaEventActionDeleteMessage(5, 8, 1)
                ]
            },
            // Event: 10 Efter att avslagsbrev skickats till producent
            new SchemaEvent(10, 10, 12, 9, isLastInChain: true, labels: ["Skriv och skicka avslagsbrev till producent", "", "", "", "", "", "", "Efter att avslagsbrev skickats till producent"], description:"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 9,1)
                ]
            },
            // Event: 11 Ja på produktionsgruppens bedömning
            new SchemaEvent(11, 11, 14, 6, labels: ["Produktionsgruppen svarar ja i sin bedömning", "", "", "", "", "", "", ""], description:"Ja på produktionsgruppens bedömning")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        3, 
                        5, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        4, 
                        "Projektet går in i FiV Produktioner"),
                    new SchemaEventActionMessage(
                        5, 
                        8, 
                        7, 
                        "Skriv och skicka Loc/Loi för projektet där villkoren för investeringen klart framgår"),
                    new SchemaEventActionUpdateStatus(6, 5),
                    new SchemaEventActionDeleteMessage(7, 6, 2),
                    new SchemaEventActionDeleteMessage(8, 6, 3),
                    new SchemaEventActionDeleteMessage(9, 7, 1),
                    new SchemaEventActionDeleteMessage(10, 8, 1)
                ]
            },
            // Event: 12 När Loc/Loi skickats
            new SchemaEvent(12, 12, 16, 11, labels: ["Skriv och skicka LOC/LOI till producenten", "", "", "", "", "", "", ""], description:"När loc/loi skickas")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        7, 
                        "Projektet är klart för Film i Väst Produktioner, se LOC/LOI."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        8, 
                        "LOC/LOI ska diarieföras för projektet. Ett mail har skickats ut av systemet med dokumentet bifogat."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Ert projektet är nu: Färdig för FiV Produktionsbeslut av Film i Väst"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst. Invänta besked från samtliga medfinansiärer innan avtal kan påbörjas, kontrollera status med producenten, klicka sedan på knappen Färdigfinansierat under projektöveriskt."),
                    new SchemaEventActionMessage(
                        6, 
                        22, 
                        3, 
                        "Projektet har har nu fått FIV Produktionsbeslut av Film i Väst."),
                    new SchemaEventActionDeleteMessage(7, 11, 5)
                ]
            },
            // Event: 13 När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfininasierat under översikten.
            new SchemaEvent(13, 13, 17, 12, labels: ["Nu är vi i produktionsbeslut tills någon sätter färdigfinansierat till klart", "", "", "", "", "", "", ""], description:"När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfininasierat under översikten.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        8, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionUpdateStatus(4, 8),
                    new SchemaEventActionDeleteMessage(5, 12, 5)
                ]
            },
            // Event: 14 Om CFO eller någon annan klickar på knappen Färdigfinansierat  under översikt
            new SchemaEvent(14, 14, 18, 13, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"Om CFO eller någon annan klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Är allt material och dokument inskickat till FiV så som: Manus, Finansieringsplan, Budget, Regiavtal, FiV riktlinjer etc? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                    new SchemaEventActionMessage(
                        2, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram."),
                ]
            },
            // Event: 15 AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(15, 15, 20, 14, labels: ["Producenten ska skicka in teamlista", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Vi vill att ni skickar in en uppdaterad Teamlista, antingen via mail eller här i onlineverktyget genom att klicka på utför.",
                        null,
                        7),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        6, 
                        17, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        7, 
                        8, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        8, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        9, 
                        15, 
                        4, 
                        "Avtalet signerat för projektet. Säkerställ att alla villkor för rat 1 är godkända."),
                    new SchemaEventActionUpdateStatus(10, 9)
                ]
            },
            // Event: 16 När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.
            new SchemaEvent(16, 16, 21, 15, labels: ["Sätt Rat 1 till klar", "", "", "", "", "", "", ""], description:"När producent skickar in teamlista eller koordinator laddar upp teamlista i administrationen.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 15, 5),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1"),
                    new SchemaEventActionDeleteMessage(4, 14, 4)
                ]
            },
            // Event: 17 När ansvarig kryssat i "Rat 1 klar".
            new SchemaEvent(17, 17, 26, 16, labels: ["Sätt arbetskopia till klar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 1 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 1 är nu godkända och utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Film i Väst vill att ni skickar in godkänd arbetskopia för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Film i Väst vill att ni skickar in godkänd arbetskopia för projektet när denna finns att tillgå, spara detta meddelande som en påminnelse tills ni skickat oss en länk eller liknande."),
                    new SchemaEventActionMessage(
                        6, 
                        8, 
                        3, 
                        "Producenten har fått ett meddelande gällande att de ska skicka in en godkänd arbetskopia för projektet. När arbetskopian inkommer kontrollera och godkänn den för projektet genom att kryssa i 'Godkänd arbetskopia klar' under projektöversikten."),
                    new SchemaEventActionMessage(
                        7, 
                        8, 
                        3, 
                        "Glöm inte att följa godkänd arbetskopia för projektet, klicka i kryssrutan \"Godkänd arbetskopia klar\" när ni anser detta vara klart."),
                    new SchemaEventActionDeleteMessage(8, 14, 9)
                ]
            },
            // Event: 18 När ansvarig kryssat i "Godkänd arbetskopia klar" under översikt.
            new SchemaEvent(18, 18, 27, 17, labels: ["Producenten ska skicka in premiärdatum", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Godkänd arbetskopia klar\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2"),
                    new SchemaEventActionMessage(
                        3, 
                        21, 
                        1, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionEmail(
                        4, 
                        21, 
                        "Film i Väst vill att ni meddelar samtliga premiärdatum för projektet, eventuellt festivaldatum, svensk premiär och internationell premiär här i onlineverktyget genom att klicka på utför.",
                        null,
                        5),
                    new SchemaEventActionMessage(
                        5, 
                        8, 
                        3, 
                        "Ett meddelande gällande, premiärdatum för projektet har skickats till producenten, invänta informationen eller påminn om ingenting sker."),
                    new SchemaEventActionDeleteMessage(6, 17, 6),
                    new SchemaEventActionUpdateStatus(7, 10),
                ]
            },
            // Event: 19 När producenten meddelar premiärdatum i Klientvektyget och klickar på spara
            new SchemaEvent(19, 19, 35, 18, labels: ["Sätt Rat 2 till klar", "", "", "", "", "", "", ""], description:"När producenten meddelar premiärdatum i Klientvektyget och klickar på spara")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        3, 
                        "Premiärdatum projektet är skickat från producent"),
                    new SchemaEventActionMessage(
                        2, 
                        13, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        3, 
                        19, 
                        3, 
                        "Premiärdatum projektet är skickat från producent och projektet kommer då gå i distribution när detta datum inträffar"),
                    new SchemaEventActionMessage(
                        4, 
                        17, 
                        3, 
                        "Projektet behöver ses över gällande PR och Planering där vi samlar information kring statistik, rapportering och hemsida. Vi behöver även publikstatistik, digital admission och festivalnärvaro samt priser, uppdatera projektet efterhand"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet är klart för slutredovisning, invänta slutredovisning och skicka vidare till diarieansvarig"),
                    new SchemaEventActionDeleteMessage(1, 18, 3),
                    new SchemaEventActionDeleteMessage(2, 18, 5)
                ]
            },
            // Event: 20 När ansvarig kryssat i "Rat 2 klar".
            new SchemaEvent(20, 20, 36, 18, labels: ["Sex månaders paus efter att premiärdatum inträffar", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Rat 2 klar\".")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionDeleteMessage(4, 19, 5)
                ]
            },
            // Event: 21 När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader 
            new SchemaEvent(21, 21, 37, 0, isStandAlone: true, labels: ["Sätt spend och slutredovisning till klar", "", "", "", "", "", "", ""], description:"När premiärdatum inträffar skapas ett tidbaserat meddelande som visas först efter sex månader ")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Det har nu gått sex månader sedan premiären, kontrollera att slutredovisning mottagits och godkänts under översikt för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "AA kontrollera att slutredovisning mottagits och godkänner detta under översikt för projektet."),
                    new SchemaEventActionMessage(
                        4, 
                        13, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt."),
                    new SchemaEventActionMessage(
                        5, 
                        19, 
                        3, 
                        "PRK tar emot rörligt pressmaterial och dylikt.")
                ]
            },
            // Event: 22 Vid godkänd spendredovisning och slutredovisning
            new SchemaEvent(22, 22, 39, 21, labels: ["Sätt spend och slutredovisning till klar", "", "", "", "", "", "", ""], description:"Vid godkänd spendredovisning och slutredovisning")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        8, 
                        "Spend-redovisning och slutredovisning ska diarieföras för projekt. Gå till dokumentöversikten för projektet och klicka på den gråa knappen med D-et för att skicka dokumenten på mail för diarieföring."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 3"),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 3"),
                    new SchemaEventActionMessage(
                        4, 
                        15, 
                        6,
                        "Spend-redovisning och slutredovisning ska diarieföras för projekt. Säkerställ att alla villkor för rat 3 är godkända."),
                    new SchemaEventActionMessage(
                        5, 
                        21, 
                        1, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionEmail(
                        6, 
                        21, 
                        "Från och med nu kan du, eller den du utser, skicka in den färdiga filmen till oss på FIV som två filer istället för DVD:er. Här finns en nerladdningsbar PDF med leveransspecifikationer du kan vidarebefordra till den tekniker som ska skapa filerna. Här laddar du eller din postproducent (eller liknande funktion) upp dina filer."),
                    new SchemaEventActionUpdateStatus(7, 22),
                    new SchemaEventActionDeleteMessage(8, 21, 3)
                ]
            },
            // Event: 23 När PRK eller PR klickar i "PR material mottaget"
            new SchemaEvent(23, 23, 40, 22, labels: ["Sätt PR material till klar", "", "", "", "", "", "", ""], description:"När PRK eller PR klickar i \"PR material mottaget\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        13, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        2, 
                        19, 
                        3, 
                        "Fyll i detaljerad information om mottaget PR material."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "PR material inkommit på projektet."),
                    new SchemaEventActionUpdateStatus(4, 15)
                ]
            },
            // Event: 24 När ansvarig kryssat i "Projekt avklarat" under översikt.
            new SchemaEvent(24, 24, 42, 23, labels: ["Sätt projekt till avklarat", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Projekt avklarat\" under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        3, 
                        "Projektet har nu mottagit rat 3 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Projektet har nu mottagit rat 3 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 3 är nu godkända. Betala ut Rat 3 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(4, 16),
                    new SchemaEventActionDeleteMessage(5, 22, 4)
                ]
            },
            // Event: 25 När ansvarig kryssat i "Projekt avklarat" under översikt utan att "PR material mottaget" har blivit klickat.
            new SchemaEvent(25, 25, 41, 24, isLastInChain: true, labels: ["Projekt avklarat", "", "", "", "", "", "", ""], description:"När ansvarig kryssat i \"Projekt avklarat\" under översikt utan att \"PR material mottaget\" har blivit klickat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        8, 
                        3, 
                        "Projektet har nu mottagit rat 3 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        2, 
                        17, 
                        3, 
                        "Projektet har nu mottagit rat 3 och är avklarat, projektet byter nu status till Avklarat."),
                    new SchemaEventActionMessage(
                        3, 
                        11, 
                        6,
                        "Alla villkor för rat 3 är nu godkända. Betala ut Rat 3 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(4, 17),
                    new SchemaEventActionDeleteMessage(5, 22, 4)
                ]
            }
        };

        return events;
    }
    
    private static List<SchemaEvent> DevelopmentSwedishFeatureFilmSchemaEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event: 1 Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansökan", "", "", "", "", "", "", ""], description:"Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event: 2 PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description:"PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera 360 id", "", "", "", "", "", "", ""], description:"När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet. Kontrollera att alla bilagor är med och inte är tomma. Om projektet behöver kompletteras, använd mail eller chatfunktionen och kommunicera med producenten vilket eller vilka dokument som behövs. När alla bilagor finns på plats, tilldela projektet till en handläggare och skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplett-knappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses med nej.
            new SchemaEvent(6, 6, 9, 5, labels: ["Skriv handläggarens bedömning", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses med nej.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag")
                ]
            },
            // Event 7: När handläggarens bedömning låses med ja.
            new SchemaEvent(7, 7, 10, 5, labels: ["Skriv handläggarens bedömning", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses med ja.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen.")
                ]
            },
            // Event 8: Efter att avslagsbrev skickats till producent
            new SchemaEvent(8, 8, 12, 6, isLastInChain: true, labels: ["Skriv avslagsbrev och skicka till producent", "", "", "", "", "", "", ""], description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 7,1)
                ]
            },
            // Event 9: Om någon klickar på knappen Färdigfinansierat under översikt
            new SchemaEvent(9, 9, 17, 7, labels: ["Sätt ansökan till färdigfinansierad", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 6, 2),
                    new SchemaEventActionMessage(
                        7, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram.")
                ]
            },
            // Event 10: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(10, 10, 20, 9, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        6, 
                        21, 
                        "Sammanställ ekonomisk och konstnärlig redovisning för projektet och skicka till avtalsansvarig här i onlineverktyget genom att klicka på utför.",
                        null,
                        10),
                    new SchemaEventActionMessage(
                        7, 
                        21, 
                        4, 
                        "Sammanställ ekonomisk och konstnärlig redovisning för projektet och skicka till avtalsansvarig här i onlineverktyget genom att klicka på utför.",
                        null, 
                        10),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        9, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        10, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(11, 9)
                ]
            },
            // Event 11: Producenten skickar in ekonomisk och konstnärlig redovisning.
            new SchemaEvent(11, 11, 43, 10, labels: ["Producenten ska skickar in ekonomisk och konstnärlig redovisning", "", "", "", "", "", "", ""], description:"Producenten skickar in ekonomisk och konstnärlig redovisning.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 10, 7)
                ]
            },
            // Event 12: När kryssrutorna "Ekonomisk redovisning klar" och "Konstnärlig redovisning" kryssas i.
            new SchemaEvent(12, 12, 44, 11, isLastInChain: true, labels: ["Sätt ekonomisk och konstnärlig redovisning till klar", "", "", "", "", "", "", ""], description:"När kryssrutorna \"Ekonomisk redovisning klar\" och \"Konstnärlig redovisning\" kryssas i.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionUpdateStatus(6, 18)
                ]
            },
            // Event 13: När datumen för redovisning har passerat.
            new SchemaEvent(13, 13, 45, 0, isStandAlone: true, labels: ["När datum passerat", "", "", "", "", "", "", ""], description:"När datumen för redovisning har passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Datum för redovisning av projektutvecklingen är passerad. Skicka in så snart som möjligt."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Datum för redovisning av projektutvecklingen är passerad. Skicka in så snart som möjligt."),
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Nu har redovisningsdatumet [DATE] passerat för projektet."),
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        3, 
                        "Nu har redovisningsdatumet [DATE] passerat för projektet.")
                ]
            },
        };

        return events;
    }
    
    private static List<SchemaEvent> DevelopmentDramaSeriesSchemaEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event: 1 Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansökan", "", "", "", "", "", "", ""], description:"Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event: 2 PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description:"PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera 360 id", "", "", "", "", "", "", ""], description:"När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet. Kontrollera att alla bilagor är med och inte är tomma. Om projektet behöver kompletteras, använd mail eller chatfunktionen och kommunicera med producenten vilket eller vilka dokument som behövs. När alla bilagor finns på plats, tilldela projektet till en handläggare och skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplettknappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses med nej.
            new SchemaEvent(6, 6, 9, 5, labels: ["Skriv handläggarens bedömning", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses med nej.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag")
                ]
            },
            // Event 7: När handläggarens bedömning låses med ja.
            new SchemaEvent(7, 7, 10, 5, labels: ["Skriv handläggarens bedömning", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses med ja.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen.")
                ]
            },
            // Event 8: Efter att avslagsbrev skickats till producent
            new SchemaEvent(8, 8, 12, 6, isLastInChain: true, labels: ["Skriv avslagsbrev och skicka till producent", "", "", "", "", "", "", ""], description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 7,1)
                ]
            },
            // Event 9: Om någon klickar på knappen Färdigfinansierat under översikt
            new SchemaEvent(9, 9, 17, 7, labels: ["Sätt ansökan till färdigfinansierad", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 6, 2),
                    new SchemaEventActionMessage(
                        7, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram.")
                ]
            },
            // Event 10: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(10, 10, 20, 9, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        6, 
                        21, 
                        "Sammanställ ekonomisk och konstnärlig redovisning för projektet och skicka till avtalsansvarig här i onlineverktyget genom att klicka på utför.",
                        null,
                        10),
                    new SchemaEventActionMessage(
                        7, 
                        21, 
                        4, 
                        "Sammanställ ekonomisk och konstnärlig redovisning för projektet och skicka till avtalsansvarig här i onlineverktyget genom att klicka på utför.",
                        null, 
                        10),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        9, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        10, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(11, 9)
                ]
            },
            // Event 11: Producenten skickar in ekonomisk och konstnärlig redovisning.
            new SchemaEvent(11, 11, 43, 10, labels: ["Producenten ska skickar in ekonomisk och konstnärlig redovisning", "", "", "", "", "", "", ""], description:"Producenten skickar in ekonomisk och konstnärlig redovisning.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 10, 7)
                ]
            },
            // Event 12: När kryssrutorna "Ekonomisk redovisning klar" och "Konstnärlig redovisning" kryssas i.
            new SchemaEvent(12, 12, 44, 11, isLastInChain: true, labels: ["Sätt ekonomisk och konstnärlig redovisning till klar", "", "", "", "", "", "", ""], description:"När kryssrutorna \"Ekonomisk redovisning klar\" och \"Konstnärlig redovisning\" kryssas i.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionUpdateStatus(6, 18)
                ]
            },
            // Event 13: När datumen för redovisning har passerat.
            new SchemaEvent(13, 13, 45, 0, isStandAlone: true, labels: ["När datum passerat", "", "", "", "", "", "", ""], description:"När datumen för redovisning har passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Datum för redovisning av projektutvecklingen är passerad. Skicka in så snart som möjligt."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Datum för redovisning av projektutvecklingen är passerad. Skicka in så snart som möjligt."),
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Nu har redovisningsdatumet [DATE] passerat för projektet."),
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        3, 
                        "Nu har redovisningsdatumet [DATE] passerat för projektet.")
                ]
            },
        };

        return events;
    }
    
    private static List<SchemaEvent> DevelopmentDocumentaryFeatureFilmEvents()
    {
        var events = new List<SchemaEvent>()
        {
            // Event: 1 Ansökan görs av Producent eller någon hos produktionsbolaget
            new SchemaEvent(1, 1, 3, isFirstInChain: true, labels: ["Ansökan", "", "", "", "", "", "", ""], description:"Ansökan görs av Producent eller någon hos produktionsbolaget")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Ansökan för projektet har inkommit från Noisy Cricket"),
                    new SchemaEventActionUpdateStatus(2, 3)
                ]
            },
            // Event: 2 PK klickar på knappen "begär public 360 id"
            new SchemaEvent(2, 2, 4, 1, labels: ["Begär public 360 id", "", "", "", "", "", "", ""], description:"PK klickar på knappen \"begär public 360 id\"")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        12, 
                        2, 
                        "Vi ska nu skapa ett projekt för ansökan och behöver ett nytt public 360 id, fyll i detta och klicka på uppdatera knappen."),
                    new SchemaEventActionDeleteMessage(2, 1, 1)
                ]
            },
            // Event 3: När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar
            new SchemaEvent(3, 3, 5, 2, labels: ["Uppdatera 360 id", "", "", "", "", "", "", ""], description:"När uppdatera knappen klickas (till public 360 id) på obehandlade ansökningar")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        2, 
                        "Du har blivit ombedd av DA att skapa projektet. Kontrollera att alla bilagor är med och inte är tomma. Om projektet behöver kompletteras, använd mail eller chatfunktionen och kommunicera med producenten vilket eller vilka dokument som behövs. När alla bilagor finns på plats, tilldela projektet till en handläggare och skapa projektet."),
                    new SchemaEventActionDeleteMessage(2, 2, 1)
                ]
            },
            // Event 4: PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.
            new SchemaEvent(4, 4, 6, 3, labels: ["Skapa projekt", "", "", "", "", "", "", ""], description:"PK fördelar projektet på SFA/DFA/TVA genom att ange detta under översikt.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Du har blivit tilldelad projektet. Kontrollera att ansökan är komplett. Om projektet behöver kompletteras, använd dokument-översikten och kommunicera med producenten vilket eller vilka uppgifter och eller dokument som behövs till produktionsmötet, klicka sedan på knappen Ansökan komplett under översikt."),
                    new SchemaEventActionDeleteMessage(2, 3, 1),
                    new SchemaEventActionUpdateStatus(3, 4)
                ]
            },
            // Event 5: När komplettknappen på översikt trycks
            new SchemaEvent(5, 5, 7, 4, labels: ["Sätt ansökan till komplett", "", "", "", "", "", "", ""], description:"När komplettknappen på översikt trycks")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        4, 
                        "Projektet är nu komplett. Kontrollera om det är intressant för FiV och skriv handläggarens bedömning."),
                    new SchemaEventActionDeleteMessage(2, 4, 1),
                    new SchemaEventActionUpdateStatus(3, 5)
                ]
            },
            // Event 6: När handläggarens bedömning låses med nej.
            new SchemaEvent(6, 6, 9, 5, labels: ["Skriv handläggarens bedömning", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses med nej.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        5, 
                        "Skriv avslagsbrev och skicka till producent varför att projektet fick avslag")
                ]
            },
            // Event 7: När handläggarens bedömning låses med ja.
            new SchemaEvent(7, 7, 10, 5, labels: ["Skriv handläggarens bedömning", "", "", "", "", "", "", ""], description:"När handläggarens bedömning låses med ja.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        5, 
                        3, 
                        "Produktionsgruppen är tillsatt för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        5, 
                        4, 
                        "Planera handläggning och beslut av ansökan med berörda parter i produktionsgruppen.")
                ]
            },
            // Event 8: Efter att avslagsbrev skickats till producent
            new SchemaEvent(8, 8, 12, 6, isLastInChain: true, labels: ["Skriv avslagsbrev och skicka till producent", "", "", "", "", "", "", ""], description:"Efter att avslagsbrev skickats till producent")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        5, 
                        "Ett avslag har skickats på detta projekt. Diarieför detta och meddela sedan diarieansvarig att hen kan stänga ärendet i Public 360."),
                    new SchemaEventActionUpdateStatus(2, 6),
                    new SchemaEventActionDeleteMessage(3, 7,1)
                ]
            },
            // Event 9: Om någon klickar på knappen Färdigfinansierat under översikt
            new SchemaEvent(9, 9, 17, 7, labels: ["Sätt ansökan till färdigfinansierad", "", "", "", "", "", "", ""], description:"Om någon klickar på knappen Färdigfinansierat  under översikt")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        17, 
                        3, 
                        "Projektet har fått Produktionsbeslut och avtalsprocess påbörjas"),
                    new SchemaEventActionMessage(
                        2, 
                        6, 
                        8, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Projektet har fått produktionsbeslut, det är nu dags att påbörja avtalsprocessen, insamling av additionellt material"),
                    new SchemaEventActionUpdateStatus(5, 8),
                    new SchemaEventActionDeleteMessage(6, 6, 2),
                    new SchemaEventActionMessage(
                        7, 
                        15, 
                        3, 
                        "Är avtalsprocessen för projektet avklarad och signerad? Om inte så kan du radera detta meddelande så blir du påmind på nytt längre fram.")
                ]
            },
            // Event 10: AA klickar på knappen Avtalsprocess klar under översikt
            new SchemaEvent(10, 10, 20, 9, labels: ["Sätt avtalsprocess till klar", "", "", "", "", "", "", ""], description:"AA klickar på knappen Avtalsprocess klar under översikt")
            {
                Actions =
                [
                    new SchemaEventActionEmail(
                        1, 
                        21, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Film i Väst och ALLA andra parter har godkänt och signerat avtalet för projektet."),
                    new SchemaEventActionMessage(
                        3, 
                        15, 
                        3, 
                        "Avtalen för projektet är godkända och ska diarieföras. När detta är gjort, gå till projektöversikten och klicka i 'Avtal diarieförda'."),
                    new SchemaEventActionMessage(
                        4, 
                        6, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionMessage(
                        5, 
                        15, 
                        3, 
                        "Projektet har nu ett signerat avtal därför ber vi er hålla koll på Klipp tillsammans med produktionsansvarig samt uppdatera projektet när detta förändras"),
                    new SchemaEventActionEmail(
                        6, 
                        21, 
                        "Sammanställ ekonomisk och konstnärlig redovisning för projektet och skicka till avtalsansvarig här i onlineverktyget genom att klicka på utför.",
                        null,
                        10),
                    new SchemaEventActionMessage(
                        7, 
                        21, 
                        4, 
                        "Sammanställ ekonomisk och konstnärlig redovisning för projektet och skicka till avtalsansvarig här i onlineverktyget genom att klicka på utför.",
                        null, 
                        10),
                    new SchemaEventActionMessage(
                        8, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        9, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 1. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        10, 
                        11, 
                        6,
                        "Alla villkor för rat 1 är nu godkända. Betala ut Rat 1 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionUpdateStatus(11, 9)
                ]
            },
            // Event 11: Producenten skickar in ekonomisk och konstnärlig redovisning.
            new SchemaEvent(11, 11, 43, 10, labels: ["Producenten ska skickar in ekonomisk och konstnärlig redovisning", "", "", "", "", "", "", ""], description:"Producenten skickar in ekonomisk och konstnärlig redovisning.")
            {
                Actions =
                [
                    new SchemaEventActionDeleteMessage(1, 10, 7)
                ]
            },
            // Event 12: När kryssrutorna "Ekonomisk redovisning klar" och "Konstnärlig redovisning" kryssas i.
            new SchemaEvent(12, 12, 44, 11, isLastInChain: true, labels: ["Sätt ekonomisk och konstnärlig redovisning till klar", "", "", "", "", "", "", ""], description:"När kryssrutorna \"Ekonomisk redovisning klar\" och \"Konstnärlig redovisning\" kryssas i.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        11, 
                        6,
                        "Alla villkor för rat 2 är nu godkända. Betala ut Rat 2 efter att Faktura är FiV tillhanda."),
                    new SchemaEventActionMessage(
                        2, 
                        21, 
                        1, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        3, 
                        21, 
                        "Det är nu dags att skicka en faktura till oss på FiV via post eller mail gällande Rat 2. Utbetalning kommer ske när fakturan från er är hanterad av oss."),
                    new SchemaEventActionMessage(
                        4, 
                        21, 
                        1, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionEmail(
                        5, 
                        21, 
                        "Alla villkor för rat 2 är nu godkända\noch utbetalning kommer ske när\nfakturan från er är hanterad av oss."),
                    new SchemaEventActionUpdateStatus(6, 18)
                ]
            },
            // Event 13: När datumen för redovisning har passerat.
            new SchemaEvent(13, 13, 45, 0, isStandAlone: true, labels: ["När datum passerat", "", "", "", "", "", "", ""], description:"När datumen för redovisning har passerat.")
            {
                Actions =
                [
                    new SchemaEventActionMessage(
                        1, 
                        21, 
                        1, 
                        "Datum för redovisning av projektutvecklingen är passerad. Skicka in så snart som möjligt."),
                    new SchemaEventActionEmail(
                        2, 
                        21, 
                        "Datum för redovisning av projektutvecklingen är passerad. Skicka in så snart som möjligt."),
                    new SchemaEventActionMessage(
                        1, 
                        6, 
                        3, 
                        "Nu har redovisningsdatumet [DATE] passerat för projektet."),
                    new SchemaEventActionMessage(
                        1, 
                        15, 
                        3, 
                        "Nu har redovisningsdatumet [DATE] passerat för projektet.")
                ]
            },
        };

        return events;
    }
    
}