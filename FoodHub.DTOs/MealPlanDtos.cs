namespace FoodHub.DTOs;

/// <summary>A recipe planned for a given calendar day (belongs to the calling user).</summary>
public class MealPlanEntryDto
{
	public Guid Id { get; set; }
	public DateOnly Date { get; set; }
	public Guid RecipeId { get; set; }
	public string RecipeName { get; set; } = string.Empty;
}

/// <summary>Add one recipe to one day on the calling user's calendar.</summary>
public class AddMealPlanEntryDto
{
	public DateOnly Date { get; set; }
	public Guid RecipeId { get; set; }
}

/// <summary>
/// Fill the given days with random recipes drawn from the existing library.
/// <see cref="Ingredients"/> (optional) biases the pick toward recipes that use them.
/// <see cref="RecipesPerDay"/> is honored by the logic but the UI keeps it at 1 for now.
/// <see cref="Overwrite"/> = clear each selected day first; otherwise entries are appended.
/// </summary>
public class RandomizeMealPlanDto
{
	public List<DateOnly> Dates { get; set; } = [];
	public List<string> Ingredients { get; set; } = [];
	public int RecipesPerDay { get; set; } = 1;
	public bool Overwrite { get; set; }
}
