using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.ServiceDefaults;

public static class DecimalExtensions
{
	public static string ToFormattedString(this decimal value)
	{
		return value % 1 == 0 ? value.ToString("0") : value.ToString("0.##");
	}
}
