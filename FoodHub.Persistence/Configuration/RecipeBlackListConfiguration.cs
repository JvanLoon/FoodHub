using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodHub.Persistence.Configuration;

public class RecipeBlackListConfiguration : IEntityTypeConfiguration<RecipeBlackList>
{
	public void Configure(EntityTypeBuilder<RecipeBlackList> builder)
	{
		builder.HasKey(x => new { x.UserId, x.RecipeId });

		builder.Property(x => x.UserId)
			.IsRequired();

		builder.Property(x => x.RecipeId)
			.IsRequired();

		// Optionally, configure relationships if you have User or Recipe entities
		//builder.HasOne<AspUsers>().WithMany().HasForeignKey(x => x.UserId);
		//builder.HasOne<Recipe>().WithMany().HasForeignKey(x => x.RecipeId);

		builder.ToTable("RecipeBlackLists");
	}
}
