using ErrorOr;

using FoodHub.DTOs;

using MediatR;

namespace FoodCalc.Features.MealPlan.Queries.GetMealPlan;

/// <summary>The calling user's calendar entries in the inclusive date range [From, To].</summary>
public record GetMealPlanQuery(string UserId, DateOnly From, DateOnly To) : IRequest<ErrorOr<List<MealPlanEntryDto>>>;
