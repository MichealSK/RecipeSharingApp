using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Required for GetUserId() extension method
using Microsoft.AspNetCore.Mvc;
using RecipeSharingApp.Domain.DTOModels;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Service.Interface;
using System.Security.Claims; // Usually needed for access to ClaimsPrincipal

// Assuming you have these interfaces defined and injected
// private readonly IRecipeCollectionService _recipeCollectionService; 

[Authorize] // Ensures only logged-in users can access this controller
public class CollectionController : Controller
{
    private readonly IRecipeService _recipeService;
    private readonly IRecipeCollectionService _recipeCollectionService;
    public CollectionController(IRecipeService recipeService, IRecipeCollectionService recipeCollectionService)
    {
        _recipeService = recipeService;
        _recipeCollectionService = recipeCollectionService;
    }

    public IActionResult Index()
    {
        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Redirect("~/Identity/Account/Login");
        }

        if (string.IsNullOrEmpty(currentUserId))
        {
            return View(new List<RecipeCollection>());
        }

        List<RecipeCollection> userCollections = _recipeCollectionService.GetUserCollectionsSync(currentUserId);

        return View(userCollections);
    }

    public IActionResult Details(Guid? id)
    {
        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Redirect("~/Identity/Account/Login");
        }
        if (id == null)
        {
            return NotFound();
        }

        var product = _recipeCollectionService.GetById(id.Value);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // [HttpGet] /Collection/Create
    public IActionResult Create()
    {
        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Redirect("~/Identity/Account/Login");
        }
        return View(new RecipeCollectionDTO());
    }

    // [HttpPost] /Collection/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RecipeCollectionDTO model)
    {

        if (ModelState.IsValid)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Redirect("~/Identity/Account/Login");
            }

            if (currentUserId == null)
            {
                ModelState.AddModelError("", "User is not logged in or ID could not be retrieved.");
                return View(model);
            }

            RecipeCollection newCollection = new RecipeCollection
            {
                Name = model.Name,
                UserId = currentUserId,
                Recipes = new List<RecipeInCollection>()
            };

            _recipeCollectionService.Add(newCollection);

            return RedirectToAction("Index", "Collection");
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> GetUserCollectionsJson()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Redirect("~/Identity/Account/Login");
        }

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var collections = await _recipeCollectionService.GetUserCollections(userId);

        var result = collections.Select(c => new
        {
            id = c.Id,
            name = c.Name,
            recipes = c.Recipes.Count
        }).ToList();

        return Json(result);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PinRecipe(Guid recipeId, Guid collectionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var recipe = _recipeService.GetById(recipeId);
        var collection = _recipeCollectionService.GetById(collectionId);

        if (recipe == null || collection == null || collection.UserId != userId)
        {
            return Json(new { success = false, message = "Invalid recipe or collection." });
        }

        try
        {
            await _recipeCollectionService.AddRecipeToCollection(collectionId, recipe);
            return Json(new { success = true, message = "Recipe pinned successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Pinning failed due to an internal error: {ex.Message}" });
        }
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
        _recipeCollectionService.DeleteById(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Unpin")]
    [ValidateAntiForgeryToken]
    public IActionResult UnpinConfirmed(Guid id)
    {
        _recipeCollectionService.RemoveRecipeFromCollection(id);
        return RedirectToAction(nameof(Index));
    }
}
