# 🍽️ FoodHub

**A comprehensive recipe and ingredient management system built with .NET 8 and Blazor Server**

FoodHub is a modern web application that helps you manage recipes, track ingredients, and generate shopping lists. Whether you're a home cook or managing a kitchen, FoodHub makes meal planning and ingredient management effortless.

## ✨ Features

- 📝 **Recipe Management**: Create, edit, and organize your favorite recipes
- 🥘 **Ingredient Management**: Manage ingredients with quantities and measurement units
- 🛒 **Shopping List Generation**: Automatically generate shopping lists from selected recipes
- 📊 **Ingredient Aggregation**: Combine ingredients across multiple recipes to avoid duplicates
- 🌐 **Modern Web Interface**: Responsive Blazor Server UI with real-time updates
- 🔄 **API**: Clean API architecture for integration possibilities

## 🛠️ Technology Stack

- **Backend**: .NET 8, ASP.NET Core Web API
- **Frontend**: Blazor Server, Bootstrap 5
- **Database**: SQL Server with Entity Framework Core
- **Architecture**: Clean Architecture with CQRS pattern using MediatR
- **ORM**: Entity Framework Core with Code-First migrations
- **Dependency Injection**: Built-in .NET DI container
- **Mapping**: AutoMapper for object-to-object mapping

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)

### Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/JvanLoon/FoodHub.git
   cd FoodHub
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update connection string**
   Update the connection string in `FoodCalc.Api/appsettings.json` to point to your SQL Server instance.

4. **Run database migrations**
   ```bash
   dotnet ef database update --project FoodHub.Persistence --startup-project FoodCalc.Api
   ```

5. **Start the application**
   ```bash
   dotnet run --project FoodHub.AppHost
   ```

6. **Access the application**
   - Web Application: The URL will be displayed in the console (typically https://localhost:7xxx)
   - API Documentation: Available at the API's swagger endpoint (typically https://localhost:7426/swagger)

## 📖 Usage Guide


### Generating Shopping Lists

1. **Select Recipes**
   - On the home page, check the boxes for recipes you want to cook
   - Click "Get Ingredients from Selected Recipes"

2. **Review Aggregated Ingredients**
   - The system automatically combines similar ingredients
   - Review quantities and adjust as needed
   - Use the generated list for shopping

## 🔧 Development

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project FoodHub.Persistence --startup-project FoodCalc.Api

# Apply migrations
dotnet ef database update --project FoodHub.Persistence --startup-project FoodCalc.Api

# Remove last migration
dotnet ef migrations remove --project FoodHub.Persistence --startup-project FoodCalc.Api

# Revert to specific migration
dotnet ef database update --project FoodHub.Persistence --startup-project FoodCalc.Api --migration <MigrationName>
```

OR

```bash
# Add a new migration
Add-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Name <MigrationName>

# Apply migrations
Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api

# Remove last migration
Remove-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api

# Revert to specific migration
Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Migration <MigrationName>
```

### API Documentation

When running the application, Swagger documentation is available at:
- Local development: `https://localhost:7426/swagger` (or the port shown in your console)

## 🤝 Contributing

2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request


## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/JvanLoon/FoodHub/issues) page
2. Create a new issue if your problem isn't already reported
3. Provide detailed information about your environment and the issue

## 🔮 Roadmap

- [ ] User authentication and authorization
- [ ] Recipe categories and tags
- [ ] Nutritional information tracking
- [ ] Recipe sharing and community features
- [ ] Mobile app (Xamarin/MAUI)
- [ ] Recipe import from popular cooking websites
- [ ] Meal planning calendar
- [ ] Inventory management

---

**Made with ❤️ by the FoodHub "team"**
