using Aspire.Hosting;
using Aspire.Hosting.Keycloak;
var builder = DistributedApplication.CreateBuilder(args);



var keycloak = builder
    .AddKeycloak("keycloak", 8090)   // <-- nouveau port host
    .WithDataVolume();


var postgres = builder.AddPostgres("postgres").WithDataVolume();
var postgresdb = postgres.AddDatabase("mydotnetdb");

var api = builder.AddProject<Projects.MyDotNetApp_ApiService>("apiservice")
    .WithReference(postgresdb)
    .WithReference(keycloak)
    .WaitFor(postgres)
    .WithExternalHttpEndpoints();

var blazor = builder.AddProject<Projects.MyDotNetApp_BlazorClient>("blazorclient")
    .WithExternalHttpEndpoints()
    .WithEnvironment("ApiBaseUrl", api.GetEndpoint("http"))
    .WithReference(keycloak);

builder.Build().Run();