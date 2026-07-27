using FoodHub.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// ---- Infrastructure: PostgreSQL + pgAdmin (containers Aspire spins up) ----
// Recreated on each run, so nothing is installed on the machine. The named
// volume keeps recipes/users across runs; reset the data from pgAdmin's
// import/restore feature. pgAdmin is published as its own container, wired to
// this server, and shows up nested under it in the dashboard.
var postgres = builder.AddPostgres(AspireConstants.PostgresDatabase)
					  .WithDataVolume(AspireConstants.PostgresVolume)
					  .WithPgAdmin(c => { c.WithLifetime(ContainerLifetime.Persistent); })
					  .WithContainerName(AspireConstants.PostgresContainer)
					  .WithLifetime(ContainerLifetime.Persistent);

var foodcalcDb = postgres.AddDatabase(AspireConstants.Database);

// ---- Application: API + Web run as PROJECTS, not containers, so they stay ----
// ---- fully debuggable (breakpoints, hot reload). Aspire still orchestrates ----
// ---- them and injects the Postgres connection string.                      ----
var apiService = builder.AddProject<Projects.FoodCalc_Api>(AspireConstants.ApiService)
						.WithHttpHealthCheck("/health")
						.WithReference(foodcalcDb)
						.WaitFor(foodcalcDb);

var web = builder.AddProject<Projects.FoodCalc_Web>(AspireConstants.WebService)
				 .WithExternalHttpEndpoints()
				 .WithReference(apiService)
				 .WaitFor(apiService);

await builder.Build()
			 .RunAsync();