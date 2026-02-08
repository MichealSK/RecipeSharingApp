using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.DTOModels
{
    public class RecipeResponseDTO
    {
        public List<RecipeDTO> recipes { get; set; }
        public int total { get; set; }
        public int skip { get; set; }
        public int limit { get; set; }
    }
}
