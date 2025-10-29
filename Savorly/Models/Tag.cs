using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Savorly.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}