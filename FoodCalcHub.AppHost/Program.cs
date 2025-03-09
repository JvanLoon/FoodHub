var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FoodCalc_ApiService>("apiservice");

builder.AddProject<Projects.FoodCalc_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

//builder.AddBlazorWebApp("blazor", "FoodCalc.Web");

//builder.AddProject<Projects.FoodCalc_Feature>("features")
//	.WithReference(apiService);

builder.Build().Run();
