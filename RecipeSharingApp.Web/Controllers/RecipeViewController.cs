using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeSharingApp.Domain.DTOModels;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Service.Implementation;
using RecipeSharingApp.Service.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RecipeSharingApp.Domain.Identity;

namespace RecipeSharingApp.Web.Controllers
{
    public class RecipeViewController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IRecipeRatingService _recipeRatingService;
        private readonly UserManager<RecipeAppUser> _userManager;

        public RecipeViewController(IRecipeService recipeService, IRecipeRatingService recipeRatingService, UserManager<RecipeAppUser> userManager)
        {
            _recipeService = recipeService;
            _recipeRatingService = recipeRatingService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_recipeService.GetAll());
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _recipeService.GetById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> AdminView()
        {
            HttpClient client = new HttpClient();
            string URL = "https://dummyjson.com/recipes";
            HttpResponseMessage message = await client.GetAsync(URL);

            if (!message.IsSuccessStatusCode)
            {
                return View(new List<RecipeDTO>());
            }

            var responseDTO = await message.Content.ReadFromJsonAsync<RecipeResponseDTO>();
            
            if (responseDTO == null)
            {
                return View(new List<RecipeDTO>());
            }

            List<RecipeDTO> data = responseDTO.recipes;
            return View(data);
        }

        public IActionResult DeleteAll()
        {
            foreach(Recipe r in _recipeService.GetAll())
            {
                _recipeService.DeleteById(r.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FetchRecipes()
        {
            HttpClient client = new HttpClient();

            string URL = "https://dummyjson.com/recipes";
            HttpResponseMessage message = await client.GetAsync(URL); 
            if (!message.IsSuccessStatusCode)
            {
                return View(new List<RecipeDTO>());
            }

            var responseDTO = await message.Content.ReadFromJsonAsync<RecipeResponseDTO>();

            if (responseDTO == null)
            {
                return View(new List<RecipeDTO>());
            }

            List<RecipeDTO> data = responseDTO.recipes;
            List<Recipe> recipesToAdd = new List<Recipe>();
            List<RecipeRating> ratingsToAdd = new List<RecipeRating>();

            foreach (RecipeDTO recipe in data)
            {
                if ((_recipeService.GetByName(recipe.name) != null)) {
                    continue;
                }

                Recipe b = new Recipe
                {
                    Id = new Guid(),
                    Name = recipe.name,
                    ImageUrl = recipe.image,
                    Instructions = recipe.instructions,
                    Ingredients = recipe.ingredients,
                    Tags = recipe.tags,
                    PrepTime = recipe.prepTimeMinutes + recipe.cookTimeMinutes,
                    Rating = recipe.rating,
                    Pins = 0
                };
                recipesToAdd.Add(b);
                RecipeRating r = new RecipeRating
                {
                    Id = new Guid(),
                    UserId = null,
                    Recipe = b,
                    RecipeId = b.Id,
                    Rating = recipe.rating,
                };
                ratingsToAdd.Add(r);
            }

            foreach (Recipe r in recipesToAdd)
            {
                _recipeService.Add(r);
            }

            foreach(RecipeRating r in ratingsToAdd)
            {
                _recipeRatingService.Add(r);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddReview(Guid recipeId)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Redirect("~/Identity/Account/Login");
            }

            var model = new ReviewDTO
            {
                RecipeId = recipeId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRating(ReviewDTO model)
        {
            if (ModelState.IsValid)
            {
                string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userManager.FindByIdAsync(currentUserId).GetAwaiter().GetResult();

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "User is not logged in or ID could not be retrieved.");
                    return View(model);
                }

                Recipe recipe = _recipeService.GetById(model.RecipeId);

                RecipeRating newRating = new RecipeRating
                {
                    Id = Guid.NewGuid(),
                    UserId = currentUserId,
                    RecipeId = model.RecipeId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    UserName = user.FirstName + " " + user.LastName,
                };

                RecipeRating r = _recipeRatingService.GetByRecipe(currentUserId, recipe.Id);
                if (r != null) {
                    _recipeRatingService.DeleteById(r.Id);
                    
                }
                
                _recipeService.AddRating(recipe, newRating);
                return RedirectToAction("Details", "RecipeView", new { id = model.RecipeId });
            }

            return View(model);
        }
    }

}
