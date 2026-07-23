using FastEndpoints;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FoodCalc.Api.Endpoints.Authentication;

public class CheckJwtTokenRequest
{
	[BindFrom("token")]
	public string? Token { get; set; }
}

/// <summary>POST api/authentication/checkjwttoken?token= — anonymous. Returns a JSON bool.</summary>
public class CheckJwtTokenEndpoint(IConfiguration configuration) : Endpoint<CheckJwtTokenRequest, bool>
{
	public override void Configure()
	{
		Post(ApiRoutes.Authentication.CheckJwtToken);
		AllowAnonymous();
	}

	public override async Task HandleAsync(CheckJwtTokenRequest req, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(req.Token))
		{
			await Send.OkAsync(false, ct);
			return;
		}

		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
		try
		{
			tokenHandler.ValidateToken(
				req.Token,
				new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = configuration["Jwt:Issuer"],
					ValidateAudience = true,
					ValidAudience = configuration["Jwt:Audience"],
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.Zero
				}, out _);

			await Send.OkAsync(true, ct);
		}
		catch { await Send.OkAsync(false, ct); }
	}
}