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
- [EF Core CLI tools](https://learn.microsoft.com/ef/core/cli/dotnet)  
  Install with: ```dotnet tool install --global dotnet-ef```

### Building and Running the Application

use Visual Studio to start both projects

Alternatively, you can:

1. Restore dependencies:
```dotnet restore```
   
2. Build the solution:
```dotnet build```

3. Run the application using VS

## Database Migrations

Entity Framework Core is used for database migrations. 

**Before running the application for the first time, you must apply the migrations to create and update the database schema.**

To do this, open the Package Manager Console and run:


- **Add a new migration:**

```Add-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Name <MigrationName>```

- **Remove the latest migration:**

```Remove-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api```

- **Update the database to a specific migration:**

```Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api```

To revert a migration, use the following command:

```Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Migration <MigrationName>```

> **Note:** Always ensure your database is up to date by running the update command above before starting the application.




## Purpose and Features

FoodCalcHub is built to streamline recipe management and ingredient calculation. Key features include:

- Creating and editing recipes
- Managing ingredients
- Generating shopping lists based on selected recipes
- More to come!
