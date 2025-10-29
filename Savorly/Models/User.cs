using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Savorly.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FavoriteRecipes { get; set; } = string.Empty; 
        public string ShoppingList { get; set; } = string.Empty; 
        public string CreatedRecipesIds { get; set; } = string.Empty; 

        public virtual ICollection<Recipe> CreatedRecipes { get; set; } = new List<Recipe>();
    }
}