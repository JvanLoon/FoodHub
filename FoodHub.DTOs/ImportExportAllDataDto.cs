using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.DTOs;

public class ImportExportAllDataDto
{
	public List<RecipeDto> Recipes { get; set; } = [];
	public List<IngredientDto> Ingredients { get; set; } = [];
	public List<RecipeIngredientDto> RecipeIngredients { get; set; } = [];
	public List<UserWithRolesDto>? Users { get; set; }
}
