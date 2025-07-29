using AutoMapper;
using FoodHub.DTOs;
using FoodHub.Persistence.Entities;

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
    }
}
