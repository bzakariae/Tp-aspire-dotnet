var builder = DistributedApplication.CreateBuilder(args);
var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);
    ;
    
var db = sql.AddDatabase("mydotnetapp");


builder.AddProject<Projects.MyDotNetApp_ApiService>("apiservice")
    .WithReference(db)
    .WaitFor(db);
builder.Build().Run();