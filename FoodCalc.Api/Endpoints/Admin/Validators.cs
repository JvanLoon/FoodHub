using FastEndpoints;

using FluentValidation;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>Paging guard (PageSize lower-bounded only; see GetRecipesRequestValidator).</summary>
public class GetUsersRequestValidator : Validator<GetUsersRequest>
{
	public GetUsersRequestValidator()
	{
		RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
		RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
	}
}

public class GetUserRolesRequestValidator : Validator<GetUserRolesRequest>
{
	public GetUserRolesRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty();
	}
}

public class ModifyUserRoleRequestValidator : Validator<ModifyUserRoleRequest>
{
	public ModifyUserRoleRequestValidator()
	{
		RuleFor(x => x.Email).NotEmpty();
		RuleFor(x => x.Role).NotEmpty();
	}
}
