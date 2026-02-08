using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.Models
{
    public class RecipeInCollection : BaseEntity
    {
        public RecipeInCollection() { }
        public RecipeInCollection(Guid collectionId, RecipeCollection collection, Guid recipeId, Recipe recipe)
        {
            Id = Guid.NewGuid();
            CollectionId = collectionId;
            Collection = collection;
            RecipeId = recipeId;
            Recipe = recipe;
        }

        public Guid CollectionId { get; set; }
        public RecipeCollection Collection { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
