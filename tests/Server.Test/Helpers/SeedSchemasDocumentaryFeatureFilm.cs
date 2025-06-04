using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.DbContext;
using Shared.Schemas.Entities;

namespace Server.Test.Helpers;

public static class SeedSchemasDocumentaryFeatureFilm
{
    public static List<Schema> Seed(WebApplicationFactory<Program> factory, List<Schema> schemas)
    {

        try
        {
            using var scope = factory.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var controls = context.Controls.ToList();
            var schemaControls = new List<Shared.Schemas.Entities.SchemaControl>();
            const string cssLeft = "application-form-left-container";
            const string cssRight = "application-form-right-container";
            const string cssGreen = " application-form-control-background-light-green";
            const string cssRed = " application-control-color-red";

            // Dokumentär långfilm
            // Page 1
            var controlOrder = 1;
            var formControlOrder = 1;
            schemaControls = new List<Shared.Schemas.Entities.SchemaControl>();

            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Titel",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "00001001"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: cssLeft,
                label: "Tidigare titlar",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000000"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Levererad",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000001"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Skapad",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000002"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Status",
                subLabel: "Ändra status på denna ansökan",
                dataSource: "[\"Ok\", \"Obehandlad\", \"Ej komplett\", \"Inför beslut\", \"Avslag\", \"FIV Produktionsbeslut\", \"Avtalsprocess\", \"Avtalsprocess klar\", \"Rough Cut klar/Godkänd arbetskopia klar\", \"Final cut / DCP klar\", \"Spendredovisning klar\", \"Spendredovisning godkänd\", \"Slutredovisning godkänd\", \"PR material mottaget\", \"Avklarat\", \"Avklarat utan PR material\", \"Avklarad projektutveckling\", \"Deleted\", \"Historia\", \"Avslutad utan samproduktion\", \"Spendredovisning och slutredovisning godkänd\", \"Åtgärdat\", \"Ej läst\"]",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000003"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Handläggare",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000005"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Produktionsansvarig",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000006"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Avtalsansvarig",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000007"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Distributionskonsult",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000008"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Finanschef",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000009"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Manuskonsult",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000010"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: true,
                css: "",
                label: "Återställ",
                applicationFormSectionId: 2,
                applicationSectionId: 20,
                systemKey: "10000004"
            ));
            
            // -- special
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Ansökan komplett",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000011"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Färdigfinansierad",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000012"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 20),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Public 360 möte bokat?",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000013"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Är avtalsprocessen klar?",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000014"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Startdatum för inspelning",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000015"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Slutdatum för inspelning",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000016"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Ekonomisk redovisning som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000017"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Konstnärlig redovisning som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000018"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Rat 1 som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000019"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Rat 2 som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000020"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Rough cut som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000021"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt alla avtal som diarieförda",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000022"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Final cut som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000023"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Final cut / DCP Kopia som klar",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000024"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 20),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Svenskt premiärdatum",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000025"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt spendredovisning som klart",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000026"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt slutredovisning som klart",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000027"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt PR material mottaget och klart",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000028"
            ));
            
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 19),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                visibleOnOverview: false,
                css: "",
                label: "Sätt Projekt som avklarat",
                applicationFormSectionId: 0,
                applicationSectionId: 0,
                systemKey: "10000029"
            ));
            
            
            // gamla



            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Tematik",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Längd/antal episoder",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Produktionsår",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "360-möte datum",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "01001001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Rat 1 klar",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "04000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Rat 2 klar",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "04000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Rough cut klar",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "04000003"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Final cut / DCP Kopia klar",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "04000004"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Final cut / DCP Kopia",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "04000005"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Ekonomisk redovisning klar",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "02000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Ekonomisk redovisning",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "02000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Möte med producent",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Konstnärlig redovisning klar",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "03000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Konstnärlig redovisning",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "03000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "PR material mottagit",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "07000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssRed,
                label: "Projekt avklarat",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "08000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Koldioxidutsläpp",
                subLabel: "CO2 ton",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Samproduktionsavtal diariefört",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "09000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Förtexter godkända av FiV",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Eftertexter godkända av FiV",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Koldioxid kommentar",
                applicationFormSectionId: 1,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Land/region",
                applicationFormSectionId: 2,
                applicationSectionId: 7,
                dataSource: "[\"Select\", \"Sweden\", \"Denmark\", \"Finland\", \"Iceland\", \"Norway\", \"Afghanistan\", \"Albania\", \"Algeria\", \"Andorra\", \"Angola\", \"Antigua and Barbuda\", \"Argentina\", \"Armenia\", \"Australia\", \"Austria\", \"Azerbaijan\", \"Bahamas\", \"Bahrain\", \"Bangladesh\", \"Barbados\", \"Belarus\", \"Belgium\", \"Belize\", \"Benin\", \"Bhutan\", \"Bolivia\", \"Bosnia and Herzegovina\", \"Botswana\", \"Brazil\", \"Brunei\", \"Bulgaria\", \"Burkina Faso\", \"Burundi\", \"Cabo Verde\", \"Cambodia\", \"Cameroon\", \"Canada\", \"Central African Republic (CAR)\", \"Chad\", \"Chile\", \"China\", \"Colombia\", \"Comoros\", \"Democratic Republic of the Congo\", \"Republic of the Congo\", \"Costa Rica\", \"Cote d'Ivoire\", \"Croatia\", \"Cuba\", \"Cyprus\", \"Czech Republic\", \"Djibouti\", \"Dominica\", \"Dominican Republic\", \"Ecuador\", \"Egypt\", \"El Salvador\", \"Equatorial Guinea\", \"Eritrea\", \"Estonia\", \"Ethiopia\", \"Fiji\", \"France\", \"Gabon\", \"Gambia\", \"Georgia\", \"Germany\", \"Ghana\", \"Greece\", \"Grenada\", \"Guatemala\", \"Guinea\", \"Guinea-Bissau\", \"Guyana\", \"Haiti\", \"Honduras\", \"Hungary\", \"India\", \"Indonesia\", \"Iran\", \"Iraq\", \"Ireland\", \"Israel\", \"Italy\", \"Jamaica\", \"Japan\", \"Jordan\", \"Kazakhstan\", \"Kenya\", \"Kiribati\", \"Kosovo\", \"Kuwait\", \"Kyrgyzstan\", \"Laos\", \"Latvia\", \"Lebanon\", \"Lesotho\", \"Liberia\", \"Libya\", \"Liechtenstein\", \"Lithuania\", \"Luxembourg\", \"Macedonia\", \"Madagascar\", \"Malawi\", \"Malaysia\", \"Maldives\", \"Mali\", \"Malta\", \"Marshall Islands\", \"Mauritania\", \"Mauritius\", \"Mexico\", \"Micronesia\", \"Moldova\", \"Monaco\", \"Mongolia\", \"Montenegro\", \"Morocco\", \"Mozambique\", \"Myanmar (Burma)\", \"Namibia\", \"Nauru\", \"Nepal\", \"Netherlands\", \"New Zealand\", \"Nicaragua\", \"Niger\", \"Nigeria\", \"North Korea\", \"Oman\", \"Pakistan\", \"Palau\", \"Palestine\", \"Panama\", \"Papua New Guinea\", \"Paraguay\", \"Peru\", \"Philippines\", \"Poland\", \"Portugal\", \"Qatar\", \"Romania\", \"Russia\", \"Rwanda\", \"Saint Kitts and Nevis\", \"Saint Lucia\", \"Saint Vincent and the Grenadines\", \"Samoa\", \"San Marino\", \"Sao Tome and Principe\", \"Saudi Arabia\", \"Senegal\", \"Serbia\", \"Seychelles\", \"Sierra Leone\", \"Singapore\", \"Slovakia\", \"Slovenia\", \"Solomon Islands\", \"Somalia\", \"South Africa\", \"South Korea\", \"South Sudan\", \"Spain\", \"Sri Lanka\", \"Sudan\", \"Suriname\", \"Swaziland\", \"Switzerland\", \"Syria\", \"Taiwan\", \"Tajikistan\", \"Tanzania\", \"Thailand\", \"Timor-Leste\", \"Togo\", \"Tonga\", \"Trinidad and Tobago\", \"Tunisia\", \"Turkey\", \"Turkmenistan\", \"Tuvalu\", \"Uganda\", \"Ukraine\", \"United Arab Emirates (UAE)\", \"United Kingdom (UK)\", \"United States of America (USA)\", \"Uruguay\", \"Uzbekistan\", \"Vanuatu\", \"Vatican City (Holy See)\", \"Venezuela\", \"Vietnam\", \"Yemen\", \"Zambia\", \"Zimbabwe\"]"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 12),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Producenter",
                applicationFormSectionId: 2,
                applicationSectionId: 7,
                dataSource: "[\"Male\", \"Female\", \"Other\"]"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Produktionsbolag",
                applicationFormSectionId: 2,
                applicationSectionId: 7
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Produktionsbolagets telefon",
                applicationFormSectionId: 2,
                applicationSectionId: 7
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Kommentar och länkar till rörligt material",
                applicationFormSectionId: 2,
                applicationSectionId: 6
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 14),
                applicationFormRequired: false,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Regissörer",
                applicationFormSectionId: 2,
                applicationSectionId: 7,
                dataSource: "[\"Male\", \"Female\", \"Other\"]"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 14),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Manusförfattare",
                applicationFormSectionId: 2,
                applicationSectionId: 7,
                dataSource: "[\"Male\", \"Female\", \"Other\"]"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Distributör",
                applicationFormSectionId: 2,
                applicationSectionId: 7
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Loc utfärdat",
                applicationFormSectionId: 2,
                applicationSectionId: 8
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Loc löper ut",
                applicationFormSectionId: 2,
                applicationSectionId: 8
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Datum för signering av avtal",
                applicationFormSectionId: 2,
                applicationSectionId: 8
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: true,
                css: cssRight + cssGreen,
                label: "Hållbar produktion",
                applicationFormSectionId: 2,
                applicationSectionId: 2,
                subLabel: "I rutan nedan anger ni hur ni arbetar för att minska er miljöpåverkan.\nT.ex. hur ni arbetar kring återvinning, transporter, måltider, pappersförbrukning osv.\nHar ni som företag en miljöpolicy, bifoga den vid steg 3 i denna ansökan."
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 17),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Beräknad längd",
                applicationFormSectionId: 2,
                applicationSectionId: 2
            ));

            // Page 2
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Filmens totala budget",
                applicationFormSectionId: 3,
                applicationSectionId: 9,
                systemKey: "00010001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Lokal valuta",
                applicationFormSectionId: 3,
                applicationSectionId: 9,
                dataSource: "[\"Select\", \"USD\", \"EUR\", \"JPY\", \"GBP\", \"AUD\", \"CAD\", \"CHF\", \"CNY\", \"SEK\", \"NZD\", \"MXN\", \"SGD\", \"HKD\", \"NOK\", \"KRW\", \"TRY\", \"RUB\", \"INR\", \"BRL\", \"ZAR\", \"TWD\", \"DKK\", \"PLN\", \"THB\", \"IDR\", \"HUF\", \"CZK\", \"ILS\", \"CLP\", \"PHP\", \"AED\", \"COP\", \"SAR\", \"MYR\", \"RON\", \"SEK\", \"VND\", \"ARS\", \"NOK\", \"IQD\", \"KWD\", \"QAR\", \"EGP\", \"PKR\", \"DZD\", \"KZT\", \"OMR\", \"CNY\", \"KRW\", \"JPY\", \"INR\", \"RUB\", \"BRL\", \"ZAR\", \"TRY\", \"EUR\", \"USD\", \"GBP\", \"AUD\", \"CAD\", \"CHF\", \"NZD\", \"MXN\", \"SGD\", \"HKD\", \"SEK\", \"NOK\", \"DKK\", \"PLN\", \"CZK\", \"HUF\", \"ILS\", \"THB\", \"AED\", \"CLP\", \"PHP\", \"SAR\", \"MYR\", \"RON\", \"VND\", \"ARS\", \"IQD\", \"KWD\", \"QAR\", \"EGP\", \"PKR\", \"DZD\", \"KZT\", \"OMR\"]",
                systemKey: "00000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Ansökningsbelopp",
                applicationFormSectionId: 3,
                applicationSectionId: 9,
                subLabel: "SEK",
                systemKey: "00000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Film i Väst insats",
                applicationFormSectionId: 1,
                applicationSectionId: 9,
                subLabel: "SEK",
                systemKey: "01000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Spendlöfte",
                applicationFormSectionId: 1,
                applicationSectionId: 9,
                subLabel: "SEK"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Spendkrav",
                applicationFormSectionId: 1,
                applicationSectionId: 9,
                subLabel: "SEK"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Faktisk redovisad spend",
                applicationFormSectionId: 1,
                applicationSectionId: 9,
                subLabel: "SEK"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Godkänd spend-redovisning",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "06000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Godkänd slut-redovisning",
                applicationFormSectionId: 1,
                applicationSectionId: 6,
                systemKey: "06000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Film i Västs insats i relation till projektets budget",
                applicationFormSectionId: 1,
                applicationSectionId: 9,
                subLabel: "%",
                systemKey: "00100001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Spend kommentar",
                applicationFormSectionId: 1,
                applicationSectionId: 9
            ));

            // Page 3
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 13),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: cssLeft,
                label: "Teamlista",
                applicationFormSectionId: 4,
                applicationSectionId: 4
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 13),
                applicationFormRequired: false,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Manus/Treatment",
                applicationFormSectionId: 4,
                applicationSectionId: 4
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 13),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssLeft,
                label: "Finansieringsplan",
                applicationFormSectionId: 4,
                applicationSectionId: 4
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 13),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Projektbeskrivning innehållande regissörens och producentens vision",
                applicationFormSectionId: 4,
                applicationSectionId: 4,
                subLabel: "Max 10 MB."
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 13),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Övriga dokument och bilagor, exempelvis CV regi, manusförfattare, producent…",
                applicationFormSectionId: 4,
                applicationSectionId: 4,
                subLabel: ""
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: true,
                visibleOnApplicationForm: true,
                css: cssRight,
                label: "Kortsynopsis",
                applicationFormSectionId: 4,
                applicationSectionId: 4,
                subLabel: "Max 600 tecken.",
                maxValueLength: 600
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "PoV Ålder",
                applicationFormSectionId: 1,
                applicationSectionId: 10
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Geografi",
                applicationFormSectionId: 1,
                applicationSectionId: 10
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 5),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Kön",
                dataSource: "[\"Male\", \"Female\", \"Both male and female\", \"Other\"]",
                applicationFormSectionId: 1,
                applicationSectionId: 10
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Funktionsvariation",
                applicationFormSectionId: 1,
                applicationSectionId: 10
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Förklaring funktionsvariation",
                applicationFormSectionId: 1,
                applicationSectionId: 10
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "HBTQ",
                applicationFormSectionId: 1,
                applicationSectionId: 10
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 18),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Inspelningsperiod",
                applicationFormSectionId: 1,
                applicationSectionId: 11,
                systemKey: "09000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Kommentar inspelning",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Hållbar produktion",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Inspelningskommun VG",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal insp dgr Trollhättan",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal insp dgr Vänersborg",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal insp dgr Göteborg",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal insp dgr Övriga kommuner VG",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Planerade inspelningsdagar VG",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Faktiska antal inspelningsdagar i VG",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Totalt antal dagar postproduktion VG",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal dagar FiV Studio Fares",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal dagar annan studio i VG",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 2),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Postproduktionsbolag",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Kommentar postproduktion",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 10),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Klippning/conform etc",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 10),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Ljudläggning",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 10),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Ljussättning/grading",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 10),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "VFX",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 10),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Ljudmix",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 10),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Övriga postproduktionsdagar i VG (ex. postproduktionskoordinator)",
                applicationFormSectionId: 1,
                applicationSectionId: 11
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "DVD:er antal mottagna",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Datum för mottagna dvd:er",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Antal mottagna posters",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Datum för mottagna posters",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Stillbilder",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Rörligt material",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Skickat logga för eftertexter",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Posters Arkivnummer",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 6),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "DVD till regionen",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "DVD i FiV Arkiv",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Uppdaterat kortsynopsis",
                applicationFormSectionId: 1,
                applicationSectionId: 12
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Premiärår",
                applicationFormSectionId: 1,
                applicationSectionId: 13
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Svenskt premiärdatum",
                applicationFormSectionId: 1,
                applicationSectionId: 13,
                systemKey: "05000001"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Internationellt premiärdatum",
                applicationFormSectionId: 1,
                applicationSectionId: 13,
                systemKey: "05000002"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 16),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Festival premiärdatum",
                applicationFormSectionId: 1,
                applicationSectionId: 13,
                systemKey: "05000003"
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 3),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Kommentarer premiär",
                applicationFormSectionId: 1,
                applicationSectionId: 13
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Producentens egna estimat",
                applicationFormSectionId: 1,
                applicationSectionId: 18
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Faktisk publik",
                applicationFormSectionId: 1,
                applicationSectionId: 18
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Distributörens estimat",
                applicationFormSectionId: 1,
                applicationSectionId: 18
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Film i Väst estimat",
                applicationFormSectionId: 1,
                applicationSectionId: 18
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Festival potential",
                applicationFormSectionId: 1,
                applicationSectionId: 18
            ));

            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 4),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Publiksegmentets ålder (målgrupp)",
                applicationFormSectionId: 1,
                applicationSectionId: 18
            ));
            
            // Festivaler
            controlOrder = schemaControls.Count + 1;
            formControlOrder = schemaControls.Count + 1;
            schemaControls.Add(SchemaControl.Create(
                controlOrder: controlOrder,
                formControlOrder: formControlOrder,
                control: controls.FirstOrDefault(x => x.ControlTypeId == 11),
                applicationFormRequired: false,
                visibleOnApplicationForm: false,
                css: "",
                label: "Festivaler",
                applicationFormSectionId: 1,
                applicationSectionId: 19
            ));

            schemas.Add(
                new Schema()
                {
                    StatusId = 2,
                    Names = new List<string>
                    { 
                        "Dokumentär långfilm", 
                        "Documentary Feature Film", 
                        "Dokumentar spillefilm", 
                        "Dokumentarfilm", 
                        "Largometraje documental", 
                        "Long métrage documentaire", 
                        "Lungometraggio documentario", 
                        "Dokumentar spillefilm"
                    },
                    ClaimTag = "DFA",
                    UpdatedDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    Controls = schemaControls,
                    Events = SchemaEvents.CreateSchemaEvents("Dokumentär långfilm"),
                    Progress =
                    [
                        new SchemaProgress()
                        {
                            SchemaProgressIdentifier = 1,
                            PercentageOfAmount = 30,
                            MonthToExpire = 1,
                            Requirements =
                            [
                                new SchemaProgressRequirement()
                                {
                                    SchemaProgressRequirementIdentifier = 1,
                                    MilestoneRequirementTypeId = 1,
                                    DocumentDeliveryTypeId = 3
                                },
                                new SchemaProgressRequirement()
                                {
                                    SchemaProgressRequirementIdentifier = 2,
                                    MilestoneRequirementTypeId = 2,
                                    DocumentDeliveryTypeId = 3
                                }
                            ]
                        },
                        new SchemaProgress()
                        {
                            SchemaProgressIdentifier = 2,
                            PercentageOfAmount = 30,
                            MonthToExpire = 2,
                            Requirements =
                            [
                                new SchemaProgressRequirement()
                                {
                                    SchemaProgressRequirementIdentifier = 1,
                                    MilestoneRequirementTypeId = 4,
                                    DocumentDeliveryTypeId = 3
                                }
                            ]
                        },
                        new SchemaProgress()
                        {
                            SchemaProgressIdentifier = 3,
                            PercentageOfAmount = 30,
                            MonthToExpire = 3,
                            Requirements =
                            [
                                new SchemaProgressRequirement()
                                {
                                    SchemaProgressRequirementIdentifier = 1,
                                    MilestoneRequirementTypeId = 5,
                                    DocumentDeliveryTypeId = 3
                                },
                                new SchemaProgressRequirement()
                                {
                                    SchemaProgressRequirementIdentifier = 2,
                                    MilestoneRequirementTypeId = 6,
                                    DocumentDeliveryTypeId = 3
                                }
                            ]
                        },
                        new SchemaProgress()
                        {
                            SchemaProgressIdentifier = 4,
                            PercentageOfAmount = 10,
                            MonthToExpire = 4,
                            Requirements =
                            [
                                new SchemaProgressRequirement()
                                {
                                    SchemaProgressRequirementIdentifier = 1,
                                    MilestoneRequirementTypeId = 7,
                                    DocumentDeliveryTypeId = 3
                                }
                            ]
                        }
                    ],
                    RequiredDocuments =
                    [
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 1, RequiredDocumentId = 10 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 2, RequiredDocumentId = 12 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 3, RequiredDocumentId = 14 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 4, RequiredDocumentId = 21 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 5, RequiredDocumentId = 22 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 6, RequiredDocumentId = 75 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 7, RequiredDocumentId = 78 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 8, RequiredDocumentId = 79 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 9, RequiredDocumentId = 80 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 10, RequiredDocumentId = 81 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 11, RequiredDocumentId = 82 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 12, RequiredDocumentId = 83 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 13, RequiredDocumentId = 84 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 14, RequiredDocumentId = 85 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 15, RequiredDocumentId = 86 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 16, RequiredDocumentId = 87 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 17, RequiredDocumentId = 88 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 18, RequiredDocumentId = 89 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 19, RequiredDocumentId = 90 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 20, RequiredDocumentId = 92 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 21, RequiredDocumentId = 97 },
                        new SchemaRequiredDocument() { RequiredDocumentIdentifier = 22, RequiredDocumentId = 100 }
                    ]
                }
            );

            return schemas;
        }
        catch
        {
            return new List<Schema>();
        }
    }
}