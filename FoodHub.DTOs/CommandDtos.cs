namespace FoodHub.DTOs;

public class CreateRecipeDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateRecipeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RecipeIngredientDto> RecipeIngredient { get; set; } = new();
}

public class CreateIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}

public class UpdateIngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool ShouldBeAddedToShoppingCart { get; set; } = true;
}
