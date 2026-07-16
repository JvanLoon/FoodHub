using FastEndpoints;

using FluentValidation;

using FoodCalc.Api.Endpoints.Common;

namespace FoodCalc.Api.Endpoints.Admin;

/// <summary>Paging guard for GET api/admin/users (see <see cref="PagedSearchRequestValidator{T}"/>).</summary>
public class GetUsersRequestValidator : PagedSearchRequestValidator<GetUsersRequest>;

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
