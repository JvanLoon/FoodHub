# FoodCalcHub

Add-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Name 

Remove-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api

Update-Database -Context ApplicationDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api
