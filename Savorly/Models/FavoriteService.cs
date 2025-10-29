using Savorly.Data;
using Savorly.Models;
using Savorly.Services;
using System;
using System.Linq;

namespace Savorly.Services
{
    public static class FavoriteService
    {
        public static void ToggleFavorite(Recipe recipe, AppDbContext context)
        {
            if (!UserService.IsLoggedIn)
            {
                throw new InvalidOperationException("Користувач не авторизований");
            }

            var favoriteIds = UserService.GetFavoriteRecipes();

            if (favoriteIds.Contains(recipe.RecipeId))
            {
                favoriteIds.Remove(recipe.RecipeId);
                recipe.IsFavorite = false;
                System.Diagnostics.Debug.WriteLine($"🗑️ Видалено рецепт {recipe.RecipeId} з улюблених");
            }
            else
            {
                if (!favoriteIds.Contains(recipe.RecipeId))
                {
                    favoriteIds.Add(recipe.RecipeId);
                }
                recipe.IsFavorite = true;
                System.Diagnostics.Debug.WriteLine($"❤️ Додано рецепт {recipe.RecipeId} до улюблених");
            }

            UserService.UpdateFavoriteRecipes(favoriteIds);
            context.SaveChanges();
        }

        public static bool IsFavorite(Recipe recipe)
        {
            if (!UserService.IsLoggedIn) return false;

            var favoriteIds = UserService.GetFavoriteRecipes();
            return favoriteIds.Contains(recipe.RecipeId);
        }

        public static void UpdateRecipesFavoriteStatus(AppDbContext context, System.Collections.Generic.List<Recipe> recipes)
        {
            if (!UserService.IsLoggedIn) return;

            var favoriteIds = UserService.GetFavoriteRecipes();
            foreach (var recipe in recipes)
            {
                recipe.IsFavorite = favoriteIds.Contains(recipe.RecipeId);
            }
        }
    }
}