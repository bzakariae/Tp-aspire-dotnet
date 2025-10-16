using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

    
// Add the database to the application model so that it can be referenced by other resources.


    
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("mydotnetdb");

var api = builder.AddProject<Projects.MyDotNetApp_ApiService>("apiservice")
    .WithReference(postgresdb);


builder.Build().Run();