using RecipeSharingApp.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.Models
{
    public class RecipeCollection : BaseEntity
    {
        public string? UserId {  get; set; }
        public RecipeAppUser? User { get; set; }
        public string? Name { get; set; }
        public ICollection<RecipeInCollection>? Recipes { get; set; }

    }
}
