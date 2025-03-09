var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FoodCalc_ApiService>("apiservice");

builder.AddProject<Projects.FoodCalc_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
