using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.DTOModels
{
    public class RecipeDTO
    {
        public string? name { get; set; }
        [DataType(DataType.ImageUrl)]
        public string? image { get; set; }
        public ICollection<string>? ingredients { get; set; }
        public ICollection<string>? instructions { get; set; }
        public ICollection<string>? tags { get; set; }
        public int prepTimeMinutes { get; set; }
        public int cookTimeMinutes { get; set; }
        public double rating { get; set; }
    }
}
