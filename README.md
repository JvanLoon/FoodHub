# FoodCalcHub

## Overview

FoodCalcHub is a web application designed to help users manage recipes and calculate ingredient quantities efficiently. The app allows users to create, edit, and aggregate recipes, making it easier to plan meals and manage shopping lists.

## Technologies Used

- **.NET 8**
- **Blazor** (WebAssembly)
- **Entity Framework Core**
- **ASP.NET Core Web API**

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Building and Running the Application

1. Clone the repository:
git clone <repository-url> cd <repository-folder>

2. Restore dependencies:
dotnet restore
   
3. Build the solution:
dotnet build

4. Run the API and Blazor projects:
dotnet run --project FoodCalc.Api dotnet run --project FoodCalc.Web

Alternatively, you can use Visual Studio to start both projects.

## Database Migrations

Entity Framework Core is used for database migrations. Use the following commands in the Package Manager Console or terminal:
```
Add-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Name <MigrationName>

Remove-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api

Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api

To apply a specific migration, use the following command:
Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Migration <MigrationName>
```


## Purpose and Features

FoodCalcHub is built to streamline recipe management and ingredient calculation. Key features include:

- Creating and editing recipes
- Managing ingredients
- Aggregating ingredients across multiple recipes
- Generating shopping lists based on selected recipes

This application is ideal for home cooks, meal planners, and anyone looking to organize their cooking and shopping efficiently.
