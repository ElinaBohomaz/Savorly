
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Savorly.Data;

#nullable disable

namespace Savorly.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.9");

            modelBuilder.Entity("RecipeTag", b =>
                {
                    b.Property<int>("RecipesRecipeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagsTagId")
                        .HasColumnType("INTEGER");

                    b.HasKey("RecipesRecipeId", "TagsTagId");

                    b.HasIndex("TagsTagId");

                    b.ToTable("RecipeTags", (string)null);
                });

            modelBuilder.Entity("Savorly.Models.Ingredient", b =>
                {
                    b.Property<int>("IngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Amount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RecipeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IngredientId");

                    b.HasIndex("RecipeId");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("Savorly.Models.Recipe", b =>
                {
                    b.Property<int>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PreparationTime")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Servings")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("RecipeId");

                    b.HasIndex("UserId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("Savorly.Models.RecipeStep", b =>
                {
                    b.Property<int>("StepId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Instruction")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RecipeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StepNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("StepId");

                    b.HasIndex("RecipeId");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("Savorly.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Savorly.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RecipeTag", b =>
                {
                    b.HasOne("Savorly.Models.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipesRecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Savorly.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Savorly.Models.Ingredient", b =>
                {
                    b.HasOne("Savorly.Models.Recipe", "Recipe")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("Savorly.Models.Recipe", b =>
                {
                    b.HasOne("Savorly.Models.User", null)
                        .WithMany("CreatedRecipes")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Savorly.Models.RecipeStep", b =>
                {
                    b.HasOne("Savorly.Models.Recipe", "Recipe")
                        .WithMany("Steps")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("Savorly.Models.Recipe", b =>
                {
                    b.Navigation("Ingredients");

                    b.Navigation("Steps");
                });

            modelBuilder.Entity("Savorly.Models.User", b =>
                {
                    b.Navigation("CreatedRecipes");
                });
#pragma warning restore 612, 618
        }
    }
}
