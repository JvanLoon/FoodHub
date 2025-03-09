using Microsoft.EntityFrameworkCore;
using FoodCalc.Features.Recepts.Commands.AddRecept;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Use the custom service registration method
builder.Services.AddCustomServices();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddReceptCommandHandler>());


// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.PropertyNamingPolicy = null; // Ensure property names are not camel-cased
}); ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapDefaultEndpoints();
//app.UseAuthorization();
app.MapControllers();

app.Run();
