using System.ComponentModel.DataAnnotations;

namespace Savorly.Models
{
    public class RecipeStep
    {
        [Key]
        public int StepId { get; set; }

        public int StepNumber { get; set; }

        [Required]
        public string Instruction { get; set; } = string.Empty;

        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; } = null!;
    }
}