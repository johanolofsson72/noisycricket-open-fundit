using System.Net;
using CrystalQuartz.AspNetCore;
using Quartz;
using Server;
using Shared.IpAddressSafeList;
using Shared.Notifications;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Test mode, this is not working for the moment
// Suppress the diagnostic warning for the AddHybridCache method
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

builder.ConfigureHealthChecks()
       .RegisterDatabase()
       .RegisterEndpointsApiExplorer()
       .RegisterEmailInformation()
       .RegisterHttpFactory()
       .RegisterAuthorization()
       .RegisterServices()
       .RegisterHostedBackgroundService()
       .RegisterQuartz()
       .RegisterSwagger(builder.Configuration)
       .RegisterKeyValidator()
       .RegisterCors()
       .RegisterBackgroundQueue()
       .RegisterEasyCache();

var app = builder.Build();

var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();
app.UseCrystalQuartz(() => scheduler);

app.ForceHttps(builder.Configuration)
       .UseCors("CorsPolicy")
       .UseHttpsRedirection()
       .UseStaticFiles();

app.MapHealthCheckAndUi(builder.Configuration)
       .UseSwagger(builder.Configuration)
       .UseApiKeyValidation(builder.Configuration)
       .UseMiddleware<IpAddressSafeListMiddleware>(builder.Configuration["IpAddressSafeList"]);

app.UseRouting();

app.RegisterEndpoints();

app.MapDefaultEndpoints();
app.Run();

public partial class Program { }