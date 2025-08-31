using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

using Microsoft.AspNetCore.Identity;

namespace FoodCalc.Features.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Recipe, RecipeDto>();
        CreateMap<Ingredient, IngredientDto>();
        CreateMap<RecipeIngredient, RecipeIngredientDto>();
        CreateMap<IngredientAmountType, IngredientAmountTypeDto>();

        // DTO to Entity mappings
        CreateMap<RecipeDto, Recipe>();
        CreateMap<IngredientDto, Ingredient>();
        CreateMap<RecipeIngredientDto, RecipeIngredient>();
        CreateMap<IngredientAmountTypeDto, IngredientAmountType>();

        // Command DTOs to Entity mappings
        CreateMap<CreateRecipeDto, Recipe>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredient, opt => opt.Ignore());

        CreateMap<UpdateRecipeDto, Recipe>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

        CreateMap<CreateIngredientDto, Ingredient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

        CreateMap<UpdateIngredientDto, Ingredient>()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore());

		CreateMap<UserDto, IdentityUser>()
			.ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
			.ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
			.ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
			.ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
			.ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
			.ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
			.ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
			.ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
			.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
			.ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
			.ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
			.ForMember(dest => dest.UserName, opt => opt.Ignore())
			.ForMember(dest => dest.Email, opt => opt.Ignore())
			.ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());
	}
}
