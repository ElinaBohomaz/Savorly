using System.ComponentModel.DataAnnotations;

namespace Savorly.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; } = null!;
    }
}