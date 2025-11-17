namespace FoodHub.ServiceDefaults;

public static class DecimalExtensions
{
	public static string ToFormattedString(this decimal value)
	{
		return value % 1 == 0 ? value.ToString("0") : value.ToString("0.##");
	}
}
