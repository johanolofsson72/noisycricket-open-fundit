
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using AppAdmin.Components.Shared;
using AppAdmin.Resources;
using AppAdmin.State;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.Applications.DTOs;
using Shared.Applications.Services;
using Shared.Documents.DTOs;
using Shared.Documents.Services;
using Shared.Events.Services;
using Shared.Messages.Services;
using Shared.Milestones.Services;
using Shared.Notifications;
using Shared.Organizations.DTOs;
using Shared.Organizations.Services;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Rtf;
using Telerik.Windows.Documents.Flow.FormatProviders.Txt;
using Telerik.Windows.Documents.Flow.Model;
using pdfProviderNamespace = Telerik.Windows.Documents.Flow.FormatProviders.Pdf;

namespace AppAdmin.Components.Projects;

public class DocumentBase : ComponentBase
{
    [Parameter] public int ApplicationId { get; set; } = new();
    [Parameter] public int DocumentId { get; set; }
    [Parameter] public int AssessmentId { get; set; }
    [Parameter] public int DocumentTypeId { get; set; }
    [Parameter] public EventCallback<int> OnUpdated { get; set; }
    [Parameter] public string TemplateName { get; set; } = string.Empty;
    [Parameter] public string Ingress { get; set; } = string.Empty;
    [Parameter] public string FileName { get; set; } = string.Empty;
    [Parameter] public List<string> Templates { get; set; } = [];
    [Parameter] public bool ShowSending { get; set; } = false;

    [CascadingParameter] public required Action<bool> ChangeLoaderVisibilityAction { get; set; }
    [CascadingParameter] public required AppState AppState { get; set; }
    [Inject] ApplicationService ApplicationService { get; set; } = default!;
    [Inject] IConfiguration Configuration { get; set; } = default!;
    [Inject] MilestoneService MilestoneService { get; set; } = default!;
    [Inject] NotificationService NotificationService { get; set; } = default!;
    [Inject] DocumentService DocumentService { get; set; } = default!;
    [Inject] MessageService MessageService { get; set; } = default!;
    [Inject] OrganizationService OrganizationService { get; set; } = default!;
    [Inject] IWebHostEnvironment WebHostEnvironment { get; set; } = default!;
    [Inject] DocumentService DocumentsService { get; set; } = default!;
    [Inject] IJSRuntime _js { get; set; } = default!;
    [Inject] IWebHostEnvironment Environment { get; set; } = default!;
    [Inject] IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] EventService EventService { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    protected string SelectedTemplate { get; set; } = string.Empty;
    protected DocumentApplicationDto SelectedApplication { get; set; } = new ();
    protected TelerikListView<DocumentDto> DocumentsListViewRef { get; set; } = new();
    protected List<DocumentDto> DocumentListViewValue { get; set; } = new();
    protected bool DocumentLocked { get; set; }
    protected bool DocumentLoaded { get; set; }
    protected bool Proceed { get; set; }
    protected string TempPath { get; set; } = string.Empty;
    protected string ChosenFileName { get; set; } = @LocalizationService.StringFromResource("Documents-38");
    protected List<IEditorTool> Tools { get; set; } =
    [
        new EditorButtonGroup(new Telerik.Blazor.Components.Editor.Bold(), 
            new Telerik.Blazor.Components.Editor.Italic(), new Telerik.Blazor.Components.Editor.Underline()),
        new EditorButtonGroup(new Telerik.Blazor.Components.Editor.AlignLeft(),
            new Telerik.Blazor.Components.Editor.AlignCenter(), new Telerik.Blazor.Components.Editor.AlignRight()),
        new UnorderedList(),
        new EditorButtonGroup(new CreateLink(), new Telerik.Blazor.Components.Editor.Unlink(), new InsertImage()),
        new InsertTable(),
        new EditorButtonGroup(new AddRowBefore(), new AddRowAfter(), new MergeCells(), new SplitCell()),
        new Telerik.Blazor.Components.Editor.Format(),
        new Telerik.Blazor.Components.Editor.FontSize(),
        new Telerik.Blazor.Components.Editor.FontFamily()
    ];
    protected string DocumentContent { get; set; } = @LocalizationService.StringFromResource("Documents-1");
    protected string DocumentPath { get; set; } = string.Empty;
    protected TelerikPdfViewer PdfViewerRef { get; set; } = default!;
    protected byte[] PdfFileData { get; set; } = default!;
    protected string DocumentAreaDisplay { get; set; } = "hidden";
    protected string TemplatePath { get; set; } = string.Empty;
    protected int DocumentEditId { get; set; } = 0;
    protected bool Enabled { get; set; } = true;

    protected override async Task OnParametersSetAsync()
    { 
        Tools = new List<IEditorTool>(EditorToolSets.All);
        Tools.Insert(0, new CustomTool("ExportToPdfTool"));
        
        TemplatePath = Path.Combine(WebHostEnvironment.ContentRootPath, 
            Path.Combine(WebHostEnvironment.ContentRootPath, 
                "Templates"), Templates.Count > 0 
            ? Templates.First() 
            : TemplateName);
        
        SelectedTemplate = Templates.Count > 0 ? Templates.First() : string.Empty;
        
        await Load();
    }

    private async Task Load()
    {
        var applicationResult = await ApplicationService.DocumentApplicationByIdAsync(ApplicationId, new CancellationToken());
        if (applicationResult.IsOk) SelectedApplication = applicationResult.Value;

        var documentsResult = await DocumentsService.DocumentsByApplicationIdAndDocumentTypeAsync(SelectedApplication.Id, DocumentTypeId, new CancellationToken());
        if (documentsResult.IsOk)
        {
            DocumentListViewValue.Clear();
            foreach (var requirement in documentsResult.Value)
            {
                if (AssessmentId > 0)
                {
                    if (requirement.Metadata.Any(x => x.Key == "Text2" && x.Value == AssessmentId.ToString()))
                    {
                        DocumentListViewValue.Add(requirement);
                    }
                }
                else
                {
                    DocumentListViewValue.Add(requirement);
                }
            }
        }

        var pop = DocumentListViewValue.Count;
        DocumentsListViewRef.Rebind();
    }

    protected async Task OnListAdd()
    {
        Enabled = false;
        ChangeLoaderVisibilityAction(true);
        await InvokeAsync(StateHasChanged);
        
        var fileName = "";
        var newName = "";
        if (FileName == "CHOOSE")
        {
            fileName = ChosenFileName;
            newName = fileName + ".pdf";
        }
        else
        {
            fileName = FileName;
            newName = (DocumentListViewValue.Count + 1).ToOrdinalWords(CultureInfo.CurrentCulture) + " " + fileName + ".pdf";
        }
        
        // Save the document in the database
        if (DocumentEditId > 0)
        {
            var document = DocumentListViewValue.First(x => x.Id == DocumentEditId);
            
            var lockDocumentResult = await DocumentsService.UpdateDocumentAsync(document.Id, 
                new UpdateDocumentDto()
                {
                    StatusId = document.StatusId,
                    RequirementTypeId = document.RequirementTypeId,
                    DeliveryTypeId = document.DeliveryTypeId,
                    FileName = document.FileName,
                    MimeType = document.MimeType,
                    Extension = document.Extension,
                    Path = document.Path,
                    Phrases = document.Phrases,
                    Summarize = document.Summarize,
                    Binary = Encoding.ASCII.GetBytes(DocumentContent),
                    Metadata = document.Metadata.Select(x => new DocumentMetaDataDto{ DocumentMetaDataIdentifier = x.DocumentMetaDataIdentifier, Key = x.Key, Value = x.Value }).ToList(),
                    IsDelivered = document.IsDelivered,
                    IsSigned = document.IsSigned,
                    IsCertified = document.IsCertified,
                    IsLocked = document.IsLocked,
                    DeliverDate = document.DeliverDate,
                    SignedDate = document.SignedDate,
                    CertifiedDate = document.CertifiedDate,
                    LockedDate = document.LockedDate
                }, new CancellationToken());
            
            if (!lockDocumentResult.IsOk)
            {
                NotificationService.Error(lockDocumentResult.Error.ToString());
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-2"));

            DocumentEditId = 0;
        }
        else
        {
            var bookmark = AssessmentId == 3 || Proceed;
            var items = new List<DocumentMetaDataDto>();
            if (DocumentTypeId == 70)
            {
                // läs av bookmark APPROVED i DocumentContent
                // bookmark = DocumentContent.Contains("APPROVED");
                items.Add(new DocumentMetaDataDto { DocumentMetaDataIdentifier = 1, Key = "ASSESSMENT_APPROVED", Value = bookmark.ToString() });

                if (AssessmentId > 0)
                {
                    items.Add(new DocumentMetaDataDto { DocumentMetaDataIdentifier = 2, Key = "Text2", Value = AssessmentId.ToString() });
                    items.Add(new DocumentMetaDataDto { DocumentMetaDataIdentifier = 3, Key = "Text3", Value = "true" });
                }
            }
            
            var createDocumentResult = await DocumentsService.CreateDocumentAsync(
                new CreateDocumentDto()
                {
                    ApplicationId = ApplicationId,
                    StatusId = 2,
                    RequirementTypeId = DocumentTypeId,
                    DeliveryTypeId = 1,
                    FileName = newName,
                    MimeType = "application/pdf",
                    Extension = ".pdf",
                    Path = string.Empty,
                    Phrases = string.Empty,
                    Summarize = string.Empty,
                    Binary = Encoding.UTF8.GetBytes(DocumentContent),
                    Metadata = items,
                    IsDelivered = false,
                    IsSigned = false,
                    IsCertified = false,
                    IsLocked = false
                }, new CancellationToken());
            
            if (!createDocumentResult.IsOk)
            {
                NotificationService.Error(createDocumentResult.Error.ToString());
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-3"));
        }

        DocumentAreaDisplay = "hidden";

        await Load();
        
        Enabled = true;
        ChangeLoaderVisibilityAction(false);
        await InvokeAsync(StateHasChanged);
    }
    
    protected async Task OnEdit(int documentId)
    {
        if (documentId > 0)
        {
            Enabled = false;
            //ChangeLoaderVisibilityAction(true);
            await InvokeAsync(StateHasChanged);
            
            var document = DocumentListViewValue.First(x => x.Id == documentId);

            DocumentContent = Encoding.UTF8.GetString(document.Binary, 0, document.Binary.Length);
            DocumentAreaDisplay = "visible";
            DocumentLoaded = true;
            DocumentEditId = documentId;

            await DocumentContentChangedHandler(DocumentContent);
            
            Enabled = true;
            //ChangeLoaderVisibilityAction(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task OnDelete(ListViewCommandEventArgs args)
    {
        if (args.Item is not null)
        {
            Enabled = false;
            //ChangeLoaderVisibilityAction(true);
            await InvokeAsync(StateHasChanged);
            
            var document = (DocumentDto)args.Item;
            var deleteDocumentResult = await DocumentsService.DeleteDocumentAsync(document.Id, new CancellationToken());
            if (!deleteDocumentResult.IsOk)
            {
                NotificationService.Error(deleteDocumentResult.Error.ToString());
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-4"));

            await Load();
            
            Enabled = true;
            //ChangeLoaderVisibilityAction(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task OnLoadTemplate()
    {
        DocumentContent = await InitTemplate(OpenTemplate());
        DocumentAreaDisplay = "visible";
        DocumentLoaded = true;
    }

    private async Task<string> InitTemplate(string templateText)
    {
        try
        {
            // Get all data needed for the template
            var ctrls = SelectedApplication.Controls;
            var title = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00001001"))?.Value ?? "";
            var budgetString = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00010001"))?.Value ?? "";
            decimal.TryParse(budgetString, out var budget);
            var appliedString = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("00000002"))?.Value ?? "";
            decimal.TryParse(appliedString, out var applied);
            var ourContributionString = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("01000001"))?.Value ?? "";
            decimal.TryParse(ourContributionString, out var ourContribution);
            var company = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("77C2DFBD"))?.Value ?? "";
            var producer = SelectedApplication.Producer.Name;
            var director = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("4D5B21F8"))?.Value ?? "";
            var projectManager = SelectedApplication.ProjectManager.Name;
            var createdBy = AppState.User.FullName;
            var companyAddress1 = "";
            var companyAddress2 = "";
            var companyPhone = "";
            var companyMobile = "";
            var organizationResult = await OrganizationService.OrganizationByIdAsync(SelectedApplication.Organization.Id);
            if (organizationResult.IsOk)
            {
                company = organizationResult.Value.Name;
                if (organizationResult.Value.Addresses.Any())
                {
                    var address = organizationResult.Value.Addresses.FirstOrDefault();
                    companyAddress1 = address?.Line1;
                    companyAddress2 = address?.PostalCode + " " + address?.City;
                }
                if (organizationResult.Value.PhoneNumbers.Count > 0)
                {
                    var phone = organizationResult.Value.PhoneNumbers.FirstOrDefault();
                    companyPhone = phone?.Number;
                }
                if (organizationResult.Value.PhoneNumbers.Count > 1)
                {
                    var phone = organizationResult.Value.PhoneNumbers.Skip(1).FirstOrDefault();
                    companyMobile = phone?.Number;
                }
            }
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var writer = SelectedApplication.ScriptManager.Name;
            var projectNumber = SelectedApplication.ProjectNumber;
            var projectType = SelectedApplication.SchemaNames[SelectedApplication.SchemaId];
            var budgetYear = ctrls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("978AC998"))?.Value ?? "";
            var decisionDate = SelectedApplication.DecisionDate.ToString("yyyy-MM-dd");

            // Get type of template
            var template = TemplateName;
            if (template == "fundit-pm-assessment.docx")
            {
                // Translations
                templateText = templateText.Replace("#ProjectManagerAssessment#", @LocalizationService.StringFromResource("Word-1"));
                templateText = templateText.Replace("#Project#", @LocalizationService.StringFromResource("Word-2"));
                templateText = templateText.Replace("#ProductionCompany#", @LocalizationService.StringFromResource("Word-3"));
                templateText = templateText.Replace("#Producer#", @LocalizationService.StringFromResource("Word-4"));
                templateText = templateText.Replace("#Director#", @LocalizationService.StringFromResource("Word-5"));
                templateText = templateText.Replace("#ApplicationSum#", @LocalizationService.StringFromResource("Word-6"));
                templateText = templateText.Replace("#TotalBudget#", @LocalizationService.StringFromResource("Word-7"));
                templateText = templateText.Replace("#ProjectManager#", @LocalizationService.StringFromResource("Word-8"));
                templateText = templateText.Replace("#CreatedBy#", @LocalizationService.StringFromResource("Word-9"));
                templateText = templateText.Replace("#Assessment#", @LocalizationService.StringFromResource("Word-10"));
                templateText = templateText.Replace("#Recommendation#", @LocalizationService.StringFromResource("Word-11"));

                // Replace placeholders
                templateText = templateText.Replace("000001", title);
                templateText = templateText.Replace("000002", company);
                templateText = templateText.Replace("000003", producer);
                templateText = templateText.Replace("000004", director);
                templateText = templateText.Replace("000005", applied.ToString("C"));
                templateText = templateText.Replace("000006", budget.ToString("C"));
                templateText = templateText.Replace("000007", projectManager);
                templateText = templateText.Replace("000008", createdBy);
            }
            else if (template == "fundit-pg-assessment.docx")
            {
                // Translations
                templateText = templateText.Replace("#ProductionGroupAssessment#", @LocalizationService.StringFromResource("Word-12"));
                templateText = templateText.Replace("#Project#", @LocalizationService.StringFromResource("Word-2"));
                templateText = templateText.Replace("#ProductionCompany#", @LocalizationService.StringFromResource("Word-3"));
                templateText = templateText.Replace("#Producer#", @LocalizationService.StringFromResource("Word-4"));
                templateText = templateText.Replace("#Director#", @LocalizationService.StringFromResource("Word-5"));
                templateText = templateText.Replace("#ApplicationSum#", @LocalizationService.StringFromResource("Word-6"));
                templateText = templateText.Replace("#TotalBudget#", @LocalizationService.StringFromResource("Word-7"));
                templateText = templateText.Replace("#ProjectManager#", @LocalizationService.StringFromResource("Word-8"));
                templateText = templateText.Replace("#CreatedBy#", @LocalizationService.StringFromResource("Word-9"));
                templateText = templateText.Replace("#Assessment#", @LocalizationService.StringFromResource("Word-10"));
                templateText = templateText.Replace("#Recommendation#", @LocalizationService.StringFromResource("Word-11"));

                // Replace placeholders
                templateText = templateText.Replace("000001", title);
                templateText = templateText.Replace("000002", company);
                templateText = templateText.Replace("000003", producer);
                templateText = templateText.Replace("000004", director);
                templateText = templateText.Replace("000005", applied.ToString("C"));
                templateText = templateText.Replace("000006", budget.ToString("C"));
                templateText = templateText.Replace("000007", projectManager);
                templateText = templateText.Replace("000008", createdBy);
            }
            else if (template == "fundit-beslut.docx")
            {
                // Translations
                templateText = templateText.Replace("#DesicionBasis#", @LocalizationService.StringFromResource("Word-26"));
                templateText = templateText.Replace("#Invest#", @LocalizationService.StringFromResource("Word-19"));
                templateText = templateText.Replace("#Sek#", @LocalizationService.StringFromResource("Word-20"));
                templateText = templateText.Replace("#Produced#", @LocalizationService.StringFromResource("Word-21"));
                templateText = templateText.Replace("#Writer#", @LocalizationService.StringFromResource("Word-22"));
                templateText = templateText.Replace("#Directed#", @LocalizationService.StringFromResource("Word-23"));
                templateText = templateText.Replace("#Description#", @LocalizationService.StringFromResource("Word-24"));
                templateText = templateText.Replace("#ToDate#", @LocalizationService.StringFromResource("Word-25"));
                templateText = templateText.Replace("#CreatedBy#", @LocalizationService.StringFromResource("Word-9"));
                templateText = templateText.Replace("#Assessment#", @LocalizationService.StringFromResource("Word-10"));
                templateText = templateText.Replace("#Recommendation#", @LocalizationService.StringFromResource("Word-11"));

                // Replace placeholders
                templateText = templateText.Replace("000001", company);
                templateText = templateText.Replace("000002", producer);
                templateText = templateText.Replace("000003", companyAddress1);
                templateText = templateText.Replace("000004", companyAddress2);
                templateText = templateText.Replace("000005", date);
                templateText = templateText.Replace("000006", title);
                templateText = templateText.Replace("000007", ourContribution.ToString("C"));
                templateText = templateText.Replace("000008", title);
                templateText = templateText.Replace("000009", company);
                templateText = templateText.Replace("000010", writer);
                templateText = templateText.Replace("000011", director);
            }
            else if (template == "fundit-loc.docx")
            {
                // Translations
                templateText = templateText.Replace("#Invest#", @LocalizationService.StringFromResource("Word-19"));
                templateText = templateText.Replace("#Sek#", @LocalizationService.StringFromResource("Word-20"));
                templateText = templateText.Replace("#Produced#", @LocalizationService.StringFromResource("Word-21"));
                templateText = templateText.Replace("#Writer#", @LocalizationService.StringFromResource("Word-22"));
                templateText = templateText.Replace("#Directed#", @LocalizationService.StringFromResource("Word-23"));
                templateText = templateText.Replace("#Description#", @LocalizationService.StringFromResource("Word-24"));
                templateText = templateText.Replace("#ToDate#", @LocalizationService.StringFromResource("Word-25"));
                templateText = templateText.Replace("#CreatedBy#", @LocalizationService.StringFromResource("Word-9"));
                templateText = templateText.Replace("#Assessment#", @LocalizationService.StringFromResource("Word-10"));
                templateText = templateText.Replace("#Recommendation#", @LocalizationService.StringFromResource("Word-11"));

                // Replace placeholders
                templateText = templateText.Replace("000001", company);
                templateText = templateText.Replace("000002", producer);
                templateText = templateText.Replace("000003", companyAddress1);
                templateText = templateText.Replace("000004", companyAddress2);
                templateText = templateText.Replace("000005", date);
                templateText = templateText.Replace("000006", title);
                templateText = templateText.Replace("000007", ourContribution.ToString("C"));
                templateText = templateText.Replace("000008", title);
                templateText = templateText.Replace("000009", company);
                templateText = templateText.Replace("000010", writer);
                templateText = templateText.Replace("000011", director);
            }
            else if (template == "fundit-avslag.docx")
            {
                // Translations
                templateText = templateText.Replace("#Hello#", @LocalizationService.StringFromResource("Word-13"));
                templateText = templateText.Replace("#Thanks#", @LocalizationService.StringFromResource("Word-14"));
                templateText = templateText.Replace("#Decision#", @LocalizationService.StringFromResource("Word-15"));
                templateText = templateText.Replace("#Wish#", @LocalizationService.StringFromResource("Word-16"));
                templateText = templateText.Replace("#Regards#", @LocalizationService.StringFromResource("Word-17"));
                templateText = templateText.Replace("#Produktionsgruppen#", @LocalizationService.StringFromResource("Word-18"));

                // Replace placeholders
                templateText = templateText.Replace("000001", company);
                templateText = templateText.Replace("000002", producer);
                templateText = templateText.Replace("000003", companyAddress1);
                templateText = templateText.Replace("000004", companyAddress2);
                templateText = templateText.Replace("000005", date);
                templateText = templateText.Replace("000006", producer);
                templateText = templateText.Replace("000007", title);
            }
            else if (template == "fundit-economyoverview.docx")
            {
                // Translations
                templateText = templateText.Replace("#EconomyOverview#", @LocalizationService.StringFromResource("Word-27"));
                templateText = templateText.Replace("#ProjectInformation#", @LocalizationService.StringFromResource("Word-28"));
                templateText = templateText.Replace("#ProjectNumber#", @LocalizationService.StringFromResource("Word-29"));
                templateText = templateText.Replace("#ProjectTitle#", @LocalizationService.StringFromResource("Word-30"));
                templateText = templateText.Replace("#ProductionCompany#", @LocalizationService.StringFromResource("Word-3"));
                templateText = templateText.Replace("#ProductionCompanyPhone#", @LocalizationService.StringFromResource("Word-32"));
                templateText = templateText.Replace("#ProductionCompanyMobile#", @LocalizationService.StringFromResource("Word-33"));
                templateText = templateText.Replace("#Producer#", @LocalizationService.StringFromResource("Word-4"));
                templateText = templateText.Replace("#Director#", @LocalizationService.StringFromResource("Word-5"));
                templateText = templateText.Replace("#ProjectType#", @LocalizationService.StringFromResource("Word-36"));
                templateText = templateText.Replace("#BudgetYear#", @LocalizationService.StringFromResource("Word-37"));
                templateText = templateText.Replace("#DecisionDate#", @LocalizationService.StringFromResource("Word-38"));
                templateText = templateText.Replace("#AllocatedAmount#", @LocalizationService.StringFromResource("Word-39"));
                templateText = templateText.Replace("#SwedishFeatureFilm#", @LocalizationService.StringFromResource("Word-40"));
                templateText = templateText.Replace("#InternationalFeatureFilm#", @LocalizationService.StringFromResource("Word-41"));
                templateText = templateText.Replace("#ShortFilm#", @LocalizationService.StringFromResource("Word-42"));
                templateText = templateText.Replace("#Tv#", @LocalizationService.StringFromResource("Word-43"));
                templateText = templateText.Replace("#Documentary#", @LocalizationService.StringFromResource("Word-44"));
                templateText = templateText.Replace("#AccountingAllocation#", @LocalizationService.StringFromResource("Word-45"));
                templateText = templateText.Replace("#PaymentComments#", @LocalizationService.StringFromResource("Word-46"));
                templateText = templateText.Replace("#Date#", @LocalizationService.StringFromResource("Word-47"));

                // Replace placeholders with data
                templateText = templateText.Replace("000001", projectNumber);
                templateText = templateText.Replace("000002", title);
                templateText = templateText.Replace("000003", company);
                templateText = templateText.Replace("000004", companyPhone);
                templateText = templateText.Replace("000005", companyMobile);
                templateText = templateText.Replace("000006", producer);
                templateText = templateText.Replace("000007", director);
                templateText = templateText.Replace("000008", projectType);
                templateText = templateText.Replace("000009", budgetYear);
                templateText = templateText.Replace("000010", decisionDate);
                templateText = templateText.Replace("000011", date);
            }
            else if (template == "fundit-blank.docx")
            {
                
            }
            
            return templateText;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return templateText;
        }
    }

    protected async Task OnSign(int documentId)
    {
        if (documentId > 0)
        {
            Enabled = false;
            ChangeLoaderVisibilityAction(true);
            await InvokeAsync(StateHasChanged);
            
            var document = DocumentListViewValue.First(x => x.Id == documentId);
            var lockDocumentResult = await DocumentsService.UpdateDocumentAsync(documentId, 
                new UpdateDocumentDto()
                {
                    StatusId = document.StatusId,
                    RequirementTypeId = document.RequirementTypeId,
                    DeliveryTypeId = document.DeliveryTypeId,
                    FileName = document.FileName,
                    MimeType = document.MimeType,
                    Extension = document.Extension,
                    Path = document.Path,
                    Phrases = document.Phrases,
                    Summarize = document.Summarize,
                    Binary = Encoding.ASCII.GetBytes(DocumentContent),
                    Metadata = document.Metadata.Select(x => new DocumentMetaDataDto{ Key = x.Key, Value = x.Value }).ToList(),
                    IsDelivered = document.IsDelivered,
                    IsSigned = true,
                    IsCertified = document.IsCertified,
                    IsLocked = document.IsLocked,
                    DeliverDate = document.DeliverDate,
                    SignedDate = DateTime.Now,
                    CertifiedDate = document.CertifiedDate,
                    LockedDate = document.LockedDate
                }, new CancellationToken());
            
            if (!lockDocumentResult.IsOk)
            {
                NotificationService.Error(lockDocumentResult.Error.ToString());
                ChangeLoaderVisibilityAction(false);
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-5"));

            await Load();
            
            Enabled = true;
            ChangeLoaderVisibilityAction(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task OnSend(int documentId)
    {
        if (documentId > 0)
        {
            Enabled = false;
            ChangeLoaderVisibilityAction(true);
            await InvokeAsync(StateHasChanged);
            
            var document = DocumentListViewValue.First(x => x.Id == documentId);

            if (!await PerformSendEventCheck(document))
            {
                Enabled = true;
                ChangeLoaderVisibilityAction(false);
                await InvokeAsync(StateHasChanged);
                return;
            }
            
            var sendDocumentResult = await DocumentsService.UpdateDocumentAsync(documentId, 
                new UpdateDocumentDto()
                {
                    StatusId = document.StatusId,
                    RequirementTypeId = document.RequirementTypeId,
                    DeliveryTypeId = document.DeliveryTypeId,
                    FileName = document.FileName,
                    MimeType = document.MimeType,
                    Extension = document.Extension,
                    Path = document.Path,
                    Phrases = document.Phrases,
                    Summarize = document.Summarize,
                    Binary = Encoding.ASCII.GetBytes(DocumentContent),
                    Metadata = document.Metadata.Select(x => new DocumentMetaDataDto{ Key = x.Key, Value = x.Value }).ToList(),
                    IsDelivered = true,
                    IsSigned = document.IsSigned,
                    IsCertified = document.IsCertified,
                    IsLocked = document.IsLocked,
                    DeliverDate = DateTime.Now,
                    SignedDate = document.SignedDate,
                    CertifiedDate = document.CertifiedDate,
                    LockedDate = document.LockedDate
                }, new CancellationToken());
            
            if (!sendDocumentResult.IsOk)
            {
                NotificationService.Error(sendDocumentResult.Error.ToString());
                ChangeLoaderVisibilityAction(false);
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-6"));

            // ToDo: send with email to someone

            await Load();
            
            Enabled = true;
            ChangeLoaderVisibilityAction(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task OnLock(int documentId)
    {
        if (documentId > 0)
        {
            Enabled = false;
            ChangeLoaderVisibilityAction(true);
            await InvokeAsync(StateHasChanged);
            
            var document = DocumentListViewValue.First(x => x.Id == documentId);

            if (!await PerformLockEventCheck(document))
            {
                Enabled = true;
                ChangeLoaderVisibilityAction(false);
                await InvokeAsync(StateHasChanged);
                return;
            }
            
            var lockDocumentResult = await DocumentsService.UpdateDocumentAsync(documentId, 
                new UpdateDocumentDto()
                {
                    StatusId = document.StatusId,
                    RequirementTypeId = document.RequirementTypeId,
                    DeliveryTypeId = document.DeliveryTypeId,
                    FileName = document.FileName,
                    MimeType = document.MimeType,
                    Extension = document.Extension,
                    Path = document.Path,
                    Phrases = document.Phrases,
                    Summarize = document.Summarize,
                    Binary = Encoding.ASCII.GetBytes(DocumentContent),
                    Metadata = document.Metadata.Select(x => new DocumentMetaDataDto{ Key = x.Key, Value = x.Value }).ToList(),
                    IsDelivered = document.IsDelivered,
                    IsSigned = document.IsSigned,
                    IsCertified = document.IsCertified,
                    IsLocked = true,
                    DeliverDate = document.DeliverDate,
                    SignedDate = document.SignedDate,
                    CertifiedDate = document.CertifiedDate,
                    LockedDate = DateTime.Now
                }, new CancellationToken());
            
            if (!lockDocumentResult.IsOk)
            {
                NotificationService.Error(lockDocumentResult.Error.ToString());
                ChangeLoaderVisibilityAction(false);
                Enabled = true;
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-7"));
            
            await Load();
            
            Enabled = true;
            ChangeLoaderVisibilityAction(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task<bool> PerformSendEventCheck(DocumentDto document)
    {
        try
        {
            switch (DocumentTypeId)
            {
                // Schema 1, 2, 3, 4, 5
                // Event 10: Efter att avslagsbrev skickats till producent
                // Schema 6, 7, 8
                // Event 8: Efter att avslagsbrev skickats till producent
                case 60 when SelectedApplication.SchemaId is 1 or 2 or 3 or 4 or 5:
                {
                    var ev10 = await EventService.TriggerNextEventById(SelectedApplication.Id, 10, new CancellationToken());
                    if (!ev10.IsOk)
                    {
                        NotificationService.Error(ev10.Error.Message, 30000);
                        Console.WriteLine(ev10.Error.Message);
                    }
                    break;
                }
                case 60:
                {
                    var ev8 = await EventService.TriggerNextEventById(SelectedApplication.Id, 8, new CancellationToken());
                    if (!ev8.IsOk)
                    {
                        NotificationService.Error(ev8.Error.Message, 30000);
                        Console.WriteLine(ev8.Error.Message);
                    }
                    break;
                }
                case 59 when SelectedApplication.SchemaId is 1 or 3 or 4 or 5:
                {
                    // Schema 1, 3, 4, 5
                    // Event 13: När loc/loi skickas
                    var ev13 = await EventService.TriggerNextEventById(SelectedApplication.Id, 13, new CancellationToken());
                    if (!ev13.IsOk)
                    {
                        NotificationService.Error(ev13.Error.Message, 30000);
                        Console.WriteLine(ev13.Error.Message);
                    }
                
                    var ctl1 = SelectedApplication.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("f96cb26f"));
                    if (ctl1 is not null)
                    {
                        ctl1.Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
                        var updateApplicationResult = await ApplicationService.UpdateApplicationControlAsync(SelectedApplication.Id, ctl1.Id, ctl1.Value, false, new CancellationToken());
                        if (!updateApplicationResult.IsOk) NotificationService.Error(updateApplicationResult.Error.Message);
                    }
                
                    var ctl2 = SelectedApplication.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("9a666fc1"));
                    if (ctl2 is not null)
                    {
                        ctl2.Value = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-dd");
                        var updateApplicationResult = await ApplicationService.UpdateApplicationControlAsync(SelectedApplication.Id, ctl2.Id, ctl2.Value, false, new CancellationToken());
                    }

                    break;
                }
                case 59 when SelectedApplication.SchemaId is 2:
                {
                    // Schema 2
                    // Event 12: När loc/loi skickas
                    var ev12 = await EventService.TriggerNextEventById(SelectedApplication.Id, 12, new CancellationToken());
                    if (!ev12.IsOk)
                    {
                        NotificationService.Error(ev12.Error.Message, 30000);
                        Console.WriteLine(ev12.Error.Message);
                    }
                
                    var ctl1 = SelectedApplication.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("F96CB26F"));
                    if (ctl1 is not null)
                    {
                        ctl1.Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
                        var updateApplicationResult = await ApplicationService.UpdateApplicationControlAsync(SelectedApplication.Id, ctl1.Id, ctl1.Value, false, new CancellationToken());
                        if (!updateApplicationResult.IsOk) NotificationService.Error(updateApplicationResult.Error.Message);
                    }
                
                    var ctl2 = SelectedApplication.Controls.FirstOrDefault(x => x.UniqueId.ToString().StartsWith("9A666FC1"));
                    if (ctl2 is not null)
                    {
                        ctl2.Value = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-dd");
                        var updateApplicationResult = await ApplicationService.UpdateApplicationControlAsync(SelectedApplication.Id, ctl2.Id, ctl2.Value, false, new CancellationToken());
                    }

                    break;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.ToString());
            return false;
        }
    }

    private async Task<bool> PerformLockEventCheck(DocumentDto document)
    {
        /*1, 2, 3, 4, 5
        Event 5: När komplett-knappen på översikt trycks
        Event 6: När handläggarens bedömning låses
        Event 7: Nej på handläggarens bedömning
        Event 8: Ja på handläggarens bedömning
        Event 9: Nej på produktionsgruppens bedömning
        Event 10: Efter att avslagsbrev skickats till producent
        Event 11: Ja på produktionsgruppens bedömning
        Event 12: När 360-möte är satt som bokat på översikten
        Event 13: När loc/loi skickas
        Event 14: När LOC/LOI har skickats ligger projektet i statusen FIV Produktionsbeslut tills någon trycker på Färdigfinansierat
        Event 15: Om någon klickar på knappen Färdigfinansierat under översikt
        Event 16: När handläggaren låser beslutsunderlaget.
        Event 17: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail oc
        Event 18: AA klickar på knappen Avtalsprocess klar under översikt
        Event 19: När VD:n tar bort meddelandet om beslutsunderlag.
                        
        6, 7, 8
        Event 5: När komplett-knappen på översikt trycks
        Event 6: När handläggarens bedömning låses med nej.
        Event 7: När handläggarens bedömning låses med ja.
        Event 8: Efter att avslagsbrev skickats till producent
        Event 9: Om någon klickar på knappen Färdigfinansierat under översikt
        Event 10: AA klickar på knappen Avtalsprocess klar under översikt
        Event 11: Producenten skickar in ekonomisk och konstnärlig redovisning.
        Event 12: När kryssrutorna "Ekonomisk redovisning klar" och "Konstnärlig redovisning" kryssas i.
        */
            
        try
        {
            if (DocumentTypeId != 70) return true;
            
            // läs av bookmark APPROVED i MetaData
            var item = document.Metadata.FirstOrDefault(x => x.Key == "ASSESSMENT_APPROVED");

            if (item is null) return false;
                
            _ = bool.TryParse(item.Value, out var approved);
                        
            switch (AssessmentId)
            {   
                case 1:
                    // handläggarens bedömning
                    //
                    if (SelectedApplication.SchemaId is 1 or 2 or 3 or 4 or 5)
                    {
                            
                        // Event 6: När handläggarens bedömning låses
                        var ev6 = await EventService
                            .TriggerNextEventById(SelectedApplication.Id, 6, new CancellationToken());

                        if (approved)
                        {
                            var ev8 = await EventService.TriggerNextEventById(SelectedApplication.Id, 8, new CancellationToken(), 2);
                            if (!ev8.IsOk)
                            {
                                NotificationService.Error(ev8.Error.Message, 30000);
                                Console.WriteLine(ev8.Error.Message);
                            }
                        }
                        else
                        {
                            var ev7 = await EventService.TriggerNextEventById(SelectedApplication.Id, 7, new CancellationToken(), 1);
                            if (!ev7.IsOk)
                            {
                                NotificationService.Error(ev7.Error.Message, 30000);
                                Console.WriteLine(ev7.Error.Message);
                            }
                        }
                    }
                    else
                    {
                        if (approved)
                        {
                            var ev7 = await EventService.TriggerNextEventById(SelectedApplication.Id, 7, new CancellationToken(), 2);
                            if (!ev7.IsOk)
                            {
                                NotificationService.Error(ev7.Error.Message, 30000);
                                Console.WriteLine(ev7.Error.Message);
                            }
                        }
                        else
                        {
                            var ev6 = await EventService.TriggerNextEventById(SelectedApplication.Id, 6, new CancellationToken(), 2);
                            if (!ev6.IsOk)
                            {
                                NotificationService.Error(ev6.Error.Message, 30000);
                                Console.WriteLine(ev6.Error.Message);
                            }
                        }
                    }
                    break;
                
                case 2:
                    // produktionsgruppens bedömning
                    //
                    if (approved)
                    {
                        // Event 11: Ja på produktionsgruppens bedömning
                        /*var ev11 = await EventService
                                .TriggerNextEvent(SelectedApplication.Id, new CancellationToken(), 2);*/
                        var ev11 = await EventService.TriggerNextEventById(SelectedApplication.Id, 11, new CancellationToken(), 2);
                        if (!ev11.IsOk)
                        {
                            NotificationService.Error(ev11.Error.Message, 30000);
                            Console.WriteLine(ev11.Error.Message);
                        }
                    }
                    else
                    {
                        // Event 9: Nej på produktionsgruppens bedömning
                        /*var ev9 = await EventService
                                .TriggerNextEvent(SelectedApplication.Id, new CancellationToken(), 1);*/
                        var ev9 = await EventService.TriggerNextEventById(SelectedApplication.Id, 9, new CancellationToken(), 1);
                        if (!ev9.IsOk)
                        {
                            NotificationService.Error(ev9.Error.Message, 30000);
                            Console.WriteLine(ev9.Error.Message);
                        }
                    }
                    break;
                case 3:
                    // beslutsunderlag
                    //
                    // Event 16: När handläggaren låser beslutsunderlaget.
                    /*var ev16 = await EventService
                            .TriggerNextEvent(SelectedApplication.Id, new CancellationToken());*/
                    var ev15 = await EventService.TriggerNextEventById(SelectedApplication.Id, 15, new CancellationToken());
                    if (!ev15.IsOk)
                    {
                        NotificationService.Error(ev15.Error.Message, 30000);
                        Console.WriteLine(ev15.Error.Message);
                    }
                    _ = await ApplicationService.SetApplicationDecisionDateAsync(SelectedApplication.Id, CancellationToken.None);
                        
                    if (SelectedApplication.SchemaId is 1 or 3 or 4 or 5)
                    {
                        // Event 17: Avtalsprocess mellan FiV och Producent som sker utanför systemet med hjälp av mail och telefon samt ett dynamiskt framtaget avtalsunderlag bearbetas i en förhandlingsprocess som sträcker sej mellan 2 veckor till drygt 6 månader.
                        /*var ev17 = await EventService
                                .TriggerNextEvent(SelectedApplication.Id, new CancellationToken());*/
                        var ev16 = await EventService.TriggerNextEventById(SelectedApplication.Id, 16, new CancellationToken());
                        if (!ev16.IsOk)
                        {
                            NotificationService.Error(ev16.Error.Message, 30000);
                            Console.WriteLine(ev16.Error.Message);
                        }
                    }
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.ToString());
            return false;
        }
    }

    protected async Task OnUnlock(int documentId)
    {
        if (documentId > 0)
        {
            Enabled = false;
            ChangeLoaderVisibilityAction(true);
            await InvokeAsync(StateHasChanged);
            
            var document = DocumentListViewValue.First(x => x.Id == documentId);
            var lockDocumentResult = await DocumentsService.UpdateDocumentAsync(documentId, 
                new UpdateDocumentDto()
                {
                    StatusId = document.StatusId,
                    RequirementTypeId = document.RequirementTypeId,
                    DeliveryTypeId = document.DeliveryTypeId,
                    FileName = document.FileName,
                    MimeType = document.MimeType,
                    Extension = document.Extension,
                    Path = document.Path,
                    Phrases = document.Phrases,
                    Summarize = document.Summarize,
                    Binary = Encoding.ASCII.GetBytes(DocumentContent),
                    Metadata = document.Metadata.Select(x => new DocumentMetaDataDto{ Key = x.Key, Value = x.Value }).ToList(),
                    IsDelivered = document.IsDelivered,
                    IsSigned = document.IsSigned,
                    IsCertified = document.IsCertified,
                    IsLocked = false,
                    DeliverDate = document.DeliverDate,
                    SignedDate = document.SignedDate,
                    CertifiedDate = document.CertifiedDate,
                    LockedDate = DateTime.Now
                }, new CancellationToken());
            
            if (!lockDocumentResult.IsOk)
            {
                NotificationService.Error(lockDocumentResult.Error.ToString());
                Enabled = true;
                ChangeLoaderVisibilityAction(false);
                return;
            }
            NotificationService.Success(@LocalizationService.StringFromResource("Documents-8"));

            await Load();
            
            Enabled = true;
            ChangeLoaderVisibilityAction(false);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected async Task DocumentContentChangedHandler(string value)
    {
        try
        {
            await Task.Delay(0);
            
            var provider = new HtmlFormatProvider();
            if (provider != null)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                var document = provider.Import(value);
#pragma warning restore CS0618 // Type or member is obsolete

                var exportProvider = new pdfProviderNamespace.PdfFormatProvider();
                byte[] exportFileBytes = null!;
                using (MemoryStream ms = new MemoryStream())
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    exportProvider.Export(document, ms);
#pragma warning restore CS0618 // Type or member is obsolete
                    exportFileBytes = ms.ToArray();
                }

                DocumentContent = value;

                PdfFileData = exportFileBytes;
            }

            PdfViewerRef.Rebind();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    protected string OpenTemplate()
    {
        if (SelectedTemplate != string.Empty)
        {
            TemplatePath = Path.Combine(WebHostEnvironment.ContentRootPath, 
                Path.Combine(WebHostEnvironment.ContentRootPath, 
                    "Templates"), SelectedTemplate);
        }
        var document = ReadFile(TemplatePath);

        // convert the file to HTML
        var provider = new HtmlFormatProvider();
        provider.ExportSettings.StylesExportMode = StylesExportMode.Inline;
        provider.ExportSettings.DocumentExportLevel = DocumentExportLevel.Document;
#pragma warning disable CS0618 // Type or member is obsolete
        var html = provider.Export(document);
#pragma warning restore CS0618 // Type or member is obsolete

        // get only the <body> contents
        var bodyStart = html.IndexOf("<body>", StringComparison.Ordinal) + "<body>".Length;
        var bodyEnd = html.IndexOf("</body>", StringComparison.Ordinal);
        var body = html.Substring(bodyStart, bodyEnd - bodyStart);

        return body;
    }

    protected RadFlowDocument ReadFile(string path)
    {
        try
        {
            var fileFormatProvider = new DocxFormatProvider();

            using var input = new FileStream(path, FileMode.Open, FileAccess.Read);
#pragma warning disable CS0618 // Type or member is obsolete
            var document = fileFormatProvider.Import(input);
#pragma warning restore CS0618 // Type or member is obsolete

            return document;
        }
        catch
        {
            throw;
        }
    }
    
    protected async Task ExportToPdf()
    {
        // ensure that table cells have inline border styles and are visible in the exported document
        DocumentContent = DocumentContent.Replace("<td>", "<td style=\"border:1px solid #ccc;\">");
        // call the export service, it will discern the details based on the extension of the exported file we want
        bool isSuccess = await ExportAndDownloadHtmlContent(DocumentContent, $"EditorContent.pdf");
        // in case here was an issue
        if (isSuccess == false)
        {
            NotificationService.Error("error converting to pdf");
        }
    }

    protected async Task<bool> ExportAndDownloadHtmlContent(string htmlContent, string fileName)
    {
        try
        {
            // prepare a document with the HTML content that we can use for conversion
            var provider = new HtmlFormatProvider();
#pragma warning disable CS0618 // Type or member is obsolete
            var document = provider.Import(htmlContent);
#pragma warning restore CS0618 // Type or member is obsolete

            // get the provider to export and then download the file
            var exportProvider = GetExportFormatProvider(fileName, out var mimeType);
            byte[] exportFileBytes = null!;
            using (var ms = new MemoryStream())
            {
#pragma warning disable CS0618 // Type or member is obsolete
                exportProvider.Export(document, ms);
#pragma warning restore CS0618 // Type or member is obsolete
                exportFileBytes = ms.ToArray();
            }

            // download the file in the browser
            await FileDownloader.Save(_js, exportFileBytes, mimeType, fileName);
        }
        catch
        {
            return false;
        }
        return true;
    }

    private IFormatProvider<RadFlowDocument> GetExportFormatProvider(string fileName, out string mimeType)
    {
        // we get both the provider and the MIME type to use only one swtich-case
        IFormatProvider<RadFlowDocument> fileFormatProvider;
        string extension = Path.GetExtension(fileName);
        switch (extension)
        {
            case ".docx":
                fileFormatProvider = new DocxFormatProvider();
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                break;
            case ".rtf":
                fileFormatProvider = new RtfFormatProvider();
                mimeType = "application/rtf";
                break;
            case ".html":
                fileFormatProvider = new HtmlFormatProvider();
                mimeType = "text/html";
                break;
            case ".txt":
                fileFormatProvider = new TxtFormatProvider();
                mimeType = "text/plain";
                break;
            case ".pdf":
                fileFormatProvider = new pdfProviderNamespace.PdfFormatProvider();
                mimeType = "application/pdf";
                break;
            default:
                fileFormatProvider = null!;
                mimeType = string.Empty;
                break;
        }

        if (fileFormatProvider == null)
        {
            throw new NotSupportedException("The chosen format cannot be exported with the supported providers.");
        }

        return fileFormatProvider;
    }

    protected static string Compress(string uncompressedString)
    {
        byte[] compressedBytes;

        using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
        {
            using (var compressedStream = new MemoryStream())
            { 
                // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, I don't want to rely on that very odd behavior should it ever change
                using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                {
                    uncompressedStream.CopyTo(compressorStream);
                }

                // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                compressedBytes = compressedStream.ToArray();
            }
        }

        return Convert.ToBase64String(compressedBytes);
    }

    protected static string Decompress(string compressedString)
    {
        byte[] decompressedBytes;

        var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

        using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
        {
            using (var decompressedStream = new MemoryStream())
            {
                decompressorStream.CopyTo(decompressedStream);

                decompressedBytes = decompressedStream.ToArray();
            }
        }

        return Encoding.UTF8.GetString(decompressedBytes);
    }

}

public static class FileDownloader
{
    public static async Task Save(IJSRuntime jsRuntime, byte[] byteData, string mimeType, string fileName)
    {
        if (byteData == null)
        {
            await jsRuntime.InvokeVoidAsync("alert", "The byte array provided for Exporting was Null.");
        }
        else
        {
            await jsRuntime.InvokeVoidAsync("saveFile", Convert.ToBase64String(byteData), mimeType, fileName);
        }
    }
}