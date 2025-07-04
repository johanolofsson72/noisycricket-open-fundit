using Shared.Controls.Entities;

namespace Server.Test.Helpers;

public static class SchemaControl
{
    public static Shared.Schemas.Entities.SchemaControl Create(int controlOrder, int formControlOrder, Control? control, bool applicationFormRequired, bool visibleOnApplicationForm, string css, string label, int applicationSectionId, int applicationFormSectionId, string dataSource = "", string subLabel = "", int maxValueLength = 0, string systemKey = "", bool visibleOnOverview = true)
    {
        control ??= new Control();
        
        return new Shared.Schemas.Entities.SchemaControl()
        {
            Id = controlOrder,
            SchemaControlIdentifier = controlOrder,
            ControlId = control.Id,
            ControlTypeId = control.ControlTypeId,
            ControlTypeName = control.ControlTypeName,
            ControlValueType = control.ValueType,
            BaseStructure = control.BaseStructure,
            Order = controlOrder,
            VisibleOnApplicationForm = visibleOnApplicationForm,
            VisibleOnOverview = visibleOnOverview,
            ApplicationFormPage = applicationFormSectionId switch {
                2 => 1,
                3 => 2,
                4 => 3,
                _ => 10
            },
            ApplicationFormOrder = formControlOrder,
            ApplicationFormRequired = applicationFormRequired,
            Css = css,
            Labels = [label, "", "", "", "", "", "", ""],
            ApplicationSectionId = applicationSectionId,
            ApplicationFormSectionId = applicationFormSectionId,
            Placeholders = ["", "", "", "", "", "", "", ""],
            DataSource = dataSource,
            SubLabels = [subLabel, "", "", "", "", "", "", ""],
            MaxValueLength = maxValueLength,
            UniqueId = GenerateGuid(label)
        };
    }
    
    private static Guid GenerateGuid(string label)
    {
        var guid = Guid.NewGuid();
        if (label.Length <= 0) return guid;
        var gs = guid.ToString().Substring(0, guid.ToString().Length - 3);
            
        guid = label switch
        {
            "Titel" => new Guid($"00001001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                       
            "Tematik" => new Guid($"DC772931-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                     
            "Längd/antal episoder" => new Guid($"7461A0F4-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "Produktionsår" => new Guid($"978AC998-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                               
            "360-möte datum" => new Guid($"01001001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                              
            "Rat 1 klar" => new Guid($"04000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                  
            "Rat 2 klar" => new Guid($"04000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                  
            "Rough cut klar" => new Guid($"04000003-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                              
            "Final cut / DCP Kopia klar" => new Guid($"04000004-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                  
            "Final cut / DCP Kopia" => new Guid($"04000005-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                       
            "Ekonomisk redovisning klar" => new Guid($"02000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                  
            "Ekonomisk redovisning" => new Guid($"02000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                       
            "Möte med producent" => new Guid($"9FE8F2F9-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                          
            "Konstnärlig redovisning klar" => new Guid($"03000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                
            "Konstnärlig redovisning" => new Guid($"03000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                     
            "PR material mottagit" => new Guid($"07000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "Projekt avklarat" => new Guid($"08000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                            
            "Koldioxidutsläpp" => new Guid($"C16749B7-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                            
            "Samproduktionsavtal diariefört" => new Guid($"09000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                              
            "Förtexter godkända av FiV" => new Guid($"41E9C6F3-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                   
            "Eftertexter godkända av FiV" => new Guid($"8BCE9E39-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                 
            "Koldioxid kommentar" => new Guid($"61A9C7D8-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                         
            "Land/region" => new Guid($"EA09893C-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                 
            "Producenter" => new Guid($"EBA1414F-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                 
            "Produktionsbolag" => new Guid($"77C2DFBD-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                             
            "Produktionsbolagets telefon" => new Guid($"B41F016E-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                  
            "Kommentar och länkar till rörligt material" => new Guid($"B784472E-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                  
            "Regissörer" => new Guid($"4D5B21F8-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                  
            "Manusförfattare" => new Guid($"BF962372-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                             
            "Distributör" => new Guid($"F88289B5-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                 
            "Loc utfärdat" => new Guid($"F96CB26F-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                
            "Loc löper ut" => new Guid($"9A666FC1-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                
            "Datum för signering av avtal" => new Guid($"839E5FEB-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                               
            "Filmens totala budget" => new Guid($"00010001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                       
            "Lokal valuta" => new Guid($"00000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                
            "Ansökningsbelopp" => new Guid($"00000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                            
            "Film i Väst insats" => new Guid($"01000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                          
            "Spendlöfte" => new Guid($"FA929FA7-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                  
            "Spendkrav" => new Guid($"025127D7-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                   
            "Faktisk redovisad spend" => new Guid($"70C68F4A-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                     
            "Godkänd spend-redovisning" => new Guid($"06000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                   
            "Godkänd slut-redovisning" => new Guid($"06000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                    
            "Film i Västs insats i relation till projektets budget" => new Guid($"00100001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                       
            "Spend kommentar" => new Guid($"258B43F9-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                             
            "Teamlista" => new Guid($"2694FE36-{gs[(gs.IndexOf('-') + 1)..]}" + "075"),                                                                   
            "Manus/Treatment" => new Guid($"067C33C6-{gs[(gs.IndexOf('-') + 1)..]}" + "014"),                                                             
            "Finansieringsplan" => new Guid($"277EEF1C-{gs[(gs.IndexOf('-') + 1)..]}" + "022"),                                                           
            "Projektbeskrivning innehållande regissörens och producentens vision" => new Guid($"33EDD2B7-{gs[(gs.IndexOf('-') + 1)..]}" + "012"),         
            "Övriga dokument och bilagor, exempelvis CV regi, manusförfattare, producent…" => new Guid($"1528348A-{gs[(gs.IndexOf('-') + 1)..]}" + "010"),
            "Kortsynopsis" => new Guid($"F52326F1-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                
            "PoV Ålder" => new Guid($"C5AD15DB-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                   
            "Geografi" => new Guid($"BB08C05A-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                    
            "Kön" => new Guid($"E9DCD705-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                         
            "Funktionsvariation" => new Guid($"02D26695-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                          
            "Förklaring funktionsvariation" => new Guid($"E3AD4C97-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                               
            "HBTQ" => new Guid($"96831AA4-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                        
            "Inspelningsperiod" => new Guid($"09000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                           
            "Kommentar inspelning" => new Guid($"5D5F639E-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "Hållbar produktion" => new Guid($"AA2B6D99-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                          
            "Inspelningskommun VG" => new Guid($"F22E8978-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "Antal insp dgr Trollhättan" => new Guid($"D66D4806-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                  
            "Antal insp dgr Vänersborg" => new Guid($"48B2DFFF-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                   
            "Antal insp dgr Göteborg" => new Guid($"5081B3DA-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                     
            "Antal insp dgr Övriga kommuner VG" => new Guid($"09C2A3C7-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Planerade inspelningsdagar VG" => new Guid($"8FC992F5-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                               
            "Faktiska antal inspelningsdagar i VG" => new Guid($"0DFBAE46-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                        
            "Totalt antal dagar postproduktion VG" => new Guid($"CCB8B97B-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                        
            "Antal dagar FiV Studio Fares" => new Guid($"ADB43FA7-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                
            "Antal dagar annan studio i VG" => new Guid($"9D0B514E-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                               
            "Postproduktionsbolag" => new Guid($"0EC6E54A-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "Kommentar postproduktion" => new Guid($"DBBEC49C-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                    
            "Klippning/conform etc" => new Guid($"24A96FDA-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                       
            "Ljudläggning" => new Guid($"1BD34194-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                
            "Ljussättning/grading" => new Guid($"694EC5D5-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "VFX" => new Guid($"D077CA91-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                         
            "Ljudmix" => new Guid($"C02CAE68-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                     
            "Övriga postproduktionsdagar i VG (ex. postproduktionskoordinator)" => new Guid($"9394C5D3-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),           
            "DVD:er antal mottagna" => new Guid($"371EB8F9-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                       
            "Datum för mottagna dvd:er" => new Guid($"F50C32DA-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                   
            "Antal mottagna posters" => new Guid($"1E0749AB-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                      
            "Datum för mottagna posters" => new Guid($"86BF0491-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                  
            "Stillbilder" => new Guid($"65A15322-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                 
            "Rörligt material" => new Guid($"78EA2E40-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                            
            "Skickat logga för eftertexter" => new Guid($"53FD1167-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                               
            "Posters Arkivnummer" => new Guid($"1034C54F-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                         
            "DVD till regionen" => new Guid($"D14BC6BD-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                           
            "DVD i FiV Arkiv" => new Guid($"88AACEE7-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                             
            "Uppdaterat kortsynopsis" => new Guid($"114807EA-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                     
            "Premiärår" => new Guid($"D75EC5DE-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                                   
            "Svenskt premiärdatum" => new Guid($"05000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                        
            "Internationellt premiärdatum" => new Guid($"05000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                
            "Festival premiärdatum" => new Guid($"05000003-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                       
            "Kommentarer premiär" => new Guid($"09163A00-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                         
            "Producentens egna estimat" => new Guid($"1DA8F872-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                   
            "Faktisk publik" => new Guid($"59549044-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                              
            "Distributörens estimat" => new Guid($"ED2A6425-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                      
            "Film i Väst estimat" => new Guid($"963625DB-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                         
            "Festival potential" => new Guid($"B60E2380-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                                          
            "Publiksegmentets ålder (målgrupp)" => new Guid($"1F3A0375-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Festivaler" => new Guid($"1C7109D5-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),   
            "Tidigare titlar" => new Guid($"10000000-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),  
            "Ansökan komplett" => new Guid($"10000011-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Färdigfinansierad" => new Guid($"10000012-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Public 360 möte bokat?" => new Guid($"10000013-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Är avtalsprocessen klar?" => new Guid($"10000014-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Återställ" => new Guid($"10000004-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Handläggare" => new Guid($"10000005-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Produktionsansvarig" => new Guid($"10000006-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Avtalsansvarig" => new Guid($"10000007-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Distributionskonsult" => new Guid($"10000008-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Finanschef" => new Guid($"10000009-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Manuskonsult" => new Guid($"10000010-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Startdatum för inspelning" => new Guid($"10000015-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Slutdatum för inspelning" => new Guid($"10000016-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                        
            "Levererad" => new Guid($"10000001-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                         
            "Skapad" => new Guid($"10000002-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                         
            "Status" => new Guid($"10000003-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                        
            "Sätt Ekonomisk redovisning som klar" => new Guid($"10000017-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Konstnärlig redovisning som klar" => new Guid($"10000018-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Rat 1 som klar" => new Guid($"10000019-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Rat 2 som klar" => new Guid($"10000020-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Rough cut som klar" => new Guid($"10000021-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt alla avtal som diarieförda" => new Guid($"10000022-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Final cut som klar" => new Guid($"10000023-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Final cut / DCP Kopia som klar" => new Guid($"10000024-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Svenskt premiärdatum" => new Guid($"10000025-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt spendredovisning som klart" => new Guid($"10000026-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt slutredovisning som klart" => new Guid($"10000027-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt PR material mottaget och klart" => new Guid($"10000028-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                           
            "Sätt Projekt som avklarat" => new Guid($"10000029-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),                                        
            "Budget inkl. spendbudget" => new Guid($"11100001-{gs[(gs.IndexOf('-') + 1)..]}" + "021"),                                             
            "Distributionsplan" => new Guid($"11100002-{gs[(gs.IndexOf('-') + 1)..]}" + "111"),                                             
            "Tidsplan" => new Guid($"11100003-{gs[(gs.IndexOf('-') + 1)..]}" + "017"),                                             
            "Synopsis/Treatment (max tio sidor)" => new Guid($"11100004-{gs[(gs.IndexOf('-') + 1)..]}" + "016"),                   
            "Beskrivning av vad utvecklingen avser" => new Guid($"11100005-{gs[(gs.IndexOf('-') + 1)..]}" + "003"),                 
            "Utvecklingsbudget" => new Guid($"11100006-{gs[(gs.IndexOf('-') + 1)..]}" + "021"),                 
            "Spendbudget" => new Guid($"11100007-{gs[(gs.IndexOf('-') + 1)..]}" + "078"),               
            "CV på nyckelfunktioner" => new Guid($"11100008-{gs[(gs.IndexOf('-') + 1)..]}" + "112"),   
            "Projektbeskrivning" => new Guid($"11100009-{gs[(gs.IndexOf('-') + 1)..]}" + "012"),   
            "Beräknad längd" => new Guid($"11100010-{gs[(gs.IndexOf('-') + 1)..]}" + "000"),  
            _ => guid
        };
        
        return guid;
    }
}
