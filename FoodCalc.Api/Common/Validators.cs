using FastEndpoints;

using FluentValidation;

using FoodCalc.Api.Endpoints.Admin;
using FoodCalc.Api.Endpoints.Authentication;
using FoodCalc.Api.Endpoints.ImportExport;
using FoodCalc.Api.Endpoints.Ingredients;
using FoodCalc.Api.Endpoints.Recipes;

namespace FoodCalc.Api.Common;

// ---------------------------------------------------------------------------
// Recipes
// ---------------------------------------------------------------------------

/// <summary>Paging guard for GET api/recipe/getallrecipes (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class GetRecipesRequestValidator : PagedSearchRequestValidator<GetRecipesRequest>;

/// <summary>
/// Replaces the old inline ModelState check (`if (string.IsNullOrEmpty(recipe.Name))`)
/// in the AddRecipe controller action.
/// </summary>
public class CreateRecipeValidator : Validator<CreateRecipeDto>
{
	public CreateRecipeValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

public class UpdateRecipeValidator : Validator<UpdateRecipeDto>
{
	public UpdateRecipeValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.EntityIdRequired(Entity.Recipe));

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

public class RecipeNameUpdateValidator : Validator<RecipeNameUpdateDto>
{
	public RecipeNameUpdateValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.EntityIdRequired(Entity.Recipe));

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

// ---------------------------------------------------------------------------
// Ingredients
// ---------------------------------------------------------------------------

/// <summary>Paging guard for GET api/ingredient (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class GetIngredientsRequestValidator : PagedSearchRequestValidator<GetIngredientsRequest>;

/// <summary>Replaces the old inline ModelState check in the AddIngredient action.</summary>
public class CreateIngredientValidator : Validator<CreateIngredientDto>
{
	public CreateIngredientValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

public class UpdateIngredientValidator : Validator<UpdateIngredientDto>
{
	public UpdateIngredientValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.EntityIdRequired(Entity.Ingredient));

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage(ValidationMessages.Common.NameRequired);
	}
}

// ---------------------------------------------------------------------------
// Users
// ---------------------------------------------------------------------------

/// <summary>Paging guard for GET api/admin/users (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class AdminGetUsersRequestValidator : PagedSearchRequestValidator<FoodCalc.Api.Endpoints.Admin.GetUsersRequest>;

/// <summary>Paging guard for GET api/user/users (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class UserGetUsersRequestValidator : PagedSearchRequestValidator<FoodCalc.Api.Endpoints.Users.GetUsersRequest>;

public class GetUserRolesRequestValidator : Validator<GetUserRolesRequest>
{
	public GetUserRolesRequestValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty();
	}
}

public class ModifyUserRoleRequestValidator : Validator<ModifyUserRoleRequest>
{
	public ModifyUserRoleRequestValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty();
		RuleFor(x => x.Role)
			.NotEmpty();
	}
}

// ---------------------------------------------------------------------------
// Roles
// ---------------------------------------------------------------------------

/// <summary>Paging guard for the roles endpoints (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class GetRolesRequestValidator : PagedSearchRequestValidator<GetRolesRequest>;

// ---------------------------------------------------------------------------
// Authentication
// ---------------------------------------------------------------------------

/// <summary>
/// Mirrors the DataAnnotations on RegisterDto ([Required][EmailAddress] Email,
/// [Required][StringLength(100, MinimumLength = 6)] Password). The old
/// AuthenticationController was marked [ApiController], so these were enforced
/// automatically via ModelState; FastEndpoints only runs FluentValidation, so
/// the rules are restated here to preserve the 400-on-invalid behavior.
/// </summary>
public class RegisterValidator : Validator<RegisterDto>
{
	public RegisterValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress();
		RuleFor(x => x.Password)
			.NotEmpty()
			.Length(6, 100);
	}
}

/// <summary>Mirrors the DataAnnotations on ResetPasswordDto (see RegisterValidator).</summary>
public class ResetPasswordValidator : Validator<ResetPasswordDto>
{
	public ResetPasswordValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress();
		RuleFor(x => x.Password)
			.NotEmpty()
			.Length(6, 100);
	}
}

/// <summary>Hardening: reject blank credentials before hitting the user store.</summary>
public class LoginValidator : Validator<LoginDto>
{
	public LoginValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty();
		RuleFor(x => x.Password)
			.NotEmpty();
	}
}

/// <summary>Hardening: require an email for the toggle-user query.</summary>
public class ToggleUserRequestValidator : Validator<ToggleUserRequest>
{
	public ToggleUserRequestValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty();
	}
}

// ---------------------------------------------------------------------------
// Import / Export
// ---------------------------------------------------------------------------

/// <summary>Hardening: require an export format.</summary>
public class ExportRequestValidator : Validator<ExportRequest>
{
	public ExportRequestValidator()
	{
		RuleFor(x => x.Format)
			.NotEmpty();
	}
}
