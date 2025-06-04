using Google.Protobuf.Reflection;

var builder = DistributedApplication.CreateBuilder(args);

var openAiGateway = builder.AddProject<Projects.OpenAiGateway>("openai-gateway");

var server = builder.AddProject<Projects.Server>("server")
    .WithReference(openAiGateway)
    .WaitFor(openAiGateway);

var admin = builder.AddProject<Projects.AppAdmin>("admin")
    .WithReference(openAiGateway)
    .WaitFor(openAiGateway);

var client = builder.AddProject<Projects.AppClient>("client")
    .WithReference(openAiGateway)
    .WaitFor(openAiGateway);

builder.Build().Run();