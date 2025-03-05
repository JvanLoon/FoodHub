var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FoodCalcHub_ApiService>("apiservice");

builder.AddProject<Projects.FoodCalcHub_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
