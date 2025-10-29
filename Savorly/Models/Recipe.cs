using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Savorly.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;

        [Required]
        public string ImagePath { get; set; } = string.Empty;

        public int PreparationTime { get; set; }
        public int Servings { get; set; }
        public RecipeType Type { get; set; }
        public bool IsFavorite { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public virtual ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public string TagsDisplay => string.Join(" ", Tags?.Select(t => t.Name) ?? new List<string>());
    }
}