using Microsoft.AspNetCore.Identity;

namespace RecipeSharingApp.Domain.Identity
{
    public class RecipeAppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}