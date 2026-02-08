using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.DTOModels
{
    public class ReviewDTO
    {
        public Guid RecipeId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
