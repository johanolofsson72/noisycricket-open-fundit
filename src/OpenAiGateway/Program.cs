
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using Microsoft.SemanticKernel.Connectors.OpenAI;



var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: builder.Configuration.GetValue<string>("OpenAi:DeploymentName")!,
    endpoint: builder.Configuration.GetValue<string>("OpenAi:Endpoint")!,
    apiKey: builder.Configuration.GetValue<string>("OpenAi:Key")!,
    modelId: builder.Configuration.GetValue<string>("OpenAi:DeploymentName")!);
builder.Services.AddSingleton<OpenAiService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/v1/chatprivate", ([FromServices] OpenAiService service, ChatPrivate chatPrivate, IConfiguration configuration) => service.ChatPrivate(chatPrivate, configuration))
    .WithName("ChatPrivate")
    .WithOpenApi();

app.MapPost("/api/v1/chatpublic", ([FromServices] OpenAiService service, ChatPublic chatPublic, IConfiguration configuration) => service.ChatPublic(chatPublic, configuration))
    .WithName("ChatPublic")
    .WithOpenApi();

app.MapPost("/api/v1/translateSql", ([FromServices] OpenAiService service, TranslateSql translateSql, IConfiguration configuration) => service.TranslateSql(translateSql, configuration))
    .WithName("TranslateSql")
    .WithOpenApi();

app.MapPost("/api/v1/translateWords", ([FromServices] OpenAiService service, TranslateWords textToTranslate) => service.TranslateWords(textToTranslate))
    .WithName("TranslateWords")
    .WithOpenApi();

app.MapGet("/", () => Results.Ok("OpenAI Gateway!!!"))
    .WithName("Hello")
    .WithOpenApi();

app.MapDefaultEndpoints();
app.Run();

internal class OpenAiService
{
    private readonly IChatCompletionService _sqlPublicCompletionService;
    private readonly ChatHistory _sqlHistory;
    private readonly IChatCompletionService _chatPublicCompletionService;
    private readonly ChatHistory _chatPublicHistory;
    private readonly IChatCompletionService _chatPrivateCompletionService;
    private readonly ChatHistory _chatPrivateHistory;
    private readonly string _key;
    private readonly string _endpoint;
    private readonly string _location;

    public OpenAiService(IChatCompletionService completionService, IConfiguration configuration)
    {
        _sqlPublicCompletionService = completionService;
        _chatPublicCompletionService = completionService;
        _chatPrivateCompletionService = completionService;
        _sqlHistory = new ChatHistory();
        _chatPublicHistory = new ChatHistory();
        _chatPrivateHistory = new ChatHistory();
        var projectData = File.ReadAllText(configuration.GetValue<string>("OpenAi:GeneratedKeysPath")!);
        var prompt = configuration.GetValue<string>("OpenAi:Prompt")!.Replace("#projectdata#", projectData);
        var chatPublic = configuration.GetValue<string>("OpenAi:ChatPublic")!;
        var chatPrivate = configuration.GetValue<string>("OpenAi:ChatPrivate")!;
        _sqlHistory.AddSystemMessage(prompt);
        _chatPublicHistory.AddSystemMessage(chatPublic);
        _chatPrivateHistory.AddSystemMessage(chatPrivate);
        _key = configuration.GetValue<string>("AiTranslator:Key")!;
        _endpoint = configuration.GetValue<string>("AiTranslator:Endpoint")!;
        _location = configuration.GetValue<string>("AiTranslator:Region")!;
    }

    public async Task<string> TranslateSql(TranslateSql translateSql, IConfiguration configuration)
    {
        try
        {
            if (!configuration.GetValue<bool>("Activated")) return "This feature is not activated";

            _sqlHistory.AddUserMessage(translateSql.Question);

            var settings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 300 // Begränsar svaret till 100 tokens
            };

            var response = await _sqlPublicCompletionService.GetChatMessageContentsAsync(_sqlHistory, settings);
            var lastMessage = CleanResult(response[^1].ToString());

            return lastMessage;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ex.ToString();
        }
    }
    
    public async Task<string> ChatPublic(ChatPublic chatPublic, IConfiguration configuration)
    {
        try
        {
            if (!configuration.GetValue<bool>("Activated")) return "This feature is not activated";

            _chatPublicHistory.AddUserMessage(chatPublic.Question);

            var settings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 300
            };

            var response = await _chatPublicCompletionService.GetChatMessageContentsAsync(_chatPublicHistory, settings);
            var lastMessage = CleanResult(response[^1].ToString());

            return lastMessage;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<string> ChatPrivate(ChatPrivate chatPrivate, IConfiguration configuration)
    {
        try
        {
            if (!configuration.GetValue<bool>("Activated")) return "This feature is not activated";

            _chatPrivateHistory.AddUserMessage(chatPrivate.PreInfo);
            _chatPrivateHistory.AddUserMessage(chatPrivate.Question);

            var settings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 300
            };

            var response = await _chatPrivateCompletionService.GetChatMessageContentsAsync(_chatPrivateHistory, settings);
            var lastMessage = CleanResult(response[^1].ToString());

            return lastMessage;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<string> TranslateWords(TranslateWords textToTranslate)
    {
        try
        {
            var route = "/translate?api-version=3.0&from=" + textToTranslate.From + "&to=" + string.Join("&to=", textToTranslate.To);
            var body = new object[] { new { Text = textToTranslate.TextToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(_endpoint + route);
            Console.WriteLine($@"Request URI: {request.RequestUri}");
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _location);

            var response = await client.SendAsync(request).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync();
            
            return result;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    
    private static string CleanResult(string result)
    {
        result = result.Replace("sql", "").Replace("```", "").Replace("my", "");
        return result.Trim();
    }
}

public class ChatPrivate
{
    public required string PreInfo { get; set; }
    public required string Question { get; set; }
}

public class ChatPublic
{
    public required string Question { get; set; }
}

public class TranslateSql
{
    public required string Question { get; set; }
}

public class TranslateWords
{
    public required string TextToTranslate { get; set; }
    public required string From { get; set; }
    public required string[] To { get; set; }
}


