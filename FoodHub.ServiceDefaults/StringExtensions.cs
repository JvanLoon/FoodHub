namespace FoodHub.ServiceDefaults
{
	public static class StringExtensions
	{
		public static string NormalizeToUpper(this string value)
		{
			if (string.IsNullOrEmpty(value) || value == null)
			{
				return string.Empty;
			}

			return value.Trim().ToUpperInvariant();
		}
	}
}
