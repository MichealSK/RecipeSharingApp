using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeSharingApp.Domain.DTOModels;
using RecipeSharingApp.Domain.Identity;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Service.Interface;
using RecipeSharingApp.Web.Controllers;
using System.Security.Claims;
using FluentAssertions;
using Xunit;

namespace Tests.Controller
{
    public class RecipeViewControllerTests
    {
        private readonly Mock<IRecipeService> _mockRecipeService;
        private readonly Mock<IRecipeRatingService> _mockRatingService;
        private readonly Mock<UserManager<RecipeAppUser>> _mockUserManager;
        private readonly RecipeViewController _controller;
        private readonly string _testUserId = "user-abc";

        public RecipeViewControllerTests()
        {
            _mockRecipeService = new Mock<IRecipeService>();
            _mockRatingService = new Mock<IRecipeRatingService>();

            var store = new Mock<IUserStore<RecipeAppUser>>();
            _mockUserManager = new Mock<UserManager<RecipeAppUser>>(store.Object, null, null, null, null, null, null, null, null);

            _controller = new RecipeViewController(
                _mockRecipeService.Object,
                _mockRatingService.Object,
                _mockUserManager.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public void Index_ReturnsViewWithAllRecipes()
        {
            var recipes = new List<Recipe> { new Recipe { Name = "Tacos" } };
            _mockRecipeService.Setup(s => s.GetAll()).Returns(recipes);

            var result = _controller.Index();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(recipes);
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = _controller.Details(null);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Details_ReturnsView_WhenRecipeExists()
        {
            var id = Guid.NewGuid();
            var recipe = new Recipe { Id = id, Name = "Pasta" };
            _mockRecipeService.Setup(s => s.GetById(id)).Returns(recipe);

            var result = _controller.Details(id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(recipe);
        }

        [Fact]
        public void DeleteAll_DeletesEveryRecipe_AndRedirects()
        {
            var recipes = new List<Recipe>
            {
                new Recipe { Id = Guid.NewGuid() },
                new Recipe { Id = Guid.NewGuid() }
            };
            _mockRecipeService.Setup(s => s.GetAll()).Returns(recipes);

            var result = _controller.DeleteAll();

            _mockRecipeService.Verify(s => s.DeleteById(It.IsAny<Guid>()), Times.Exactly(2));
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Index");
        }

        [Fact]
        public void AddReview_Get_RedirectsToLogin_IfUserNull()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            var result = _controller.AddReview(Guid.NewGuid());

            result.Should().BeOfType<RedirectResult>().Which.Url.Should().Contain("Login");
        }

        [Fact]
        public void AddReview_Get_ReturnsViewWithModel()
        {
            var recipeId = Guid.NewGuid();
            var result = _controller.AddReview(recipeId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<ReviewDTO>().Subject;
            model.RecipeId.Should().Be(recipeId);
        }

        [Fact]
        public void AddRating_Post_Success_DeletesOldRatingAndAddsNew()
        {
            // Arrange
            var model = new ReviewDTO { RecipeId = Guid.NewGuid(), Rating = 5, Comment = "Great!" };
            var recipe = new Recipe { Id = model.RecipeId };
            var existingRating = new RecipeRating { Id = Guid.NewGuid() };
            var user = new RecipeAppUser { FirstName = "John", LastName = "Doe" };

            _mockUserManager.Setup(u => u.FindByIdAsync(_testUserId)).ReturnsAsync(user);
            _mockRecipeService.Setup(s => s.GetById(model.RecipeId)).Returns(recipe);
            _mockRatingService.Setup(s => s.GetByRecipe(_testUserId, recipe.Id)).Returns(existingRating);

            // Act
            var result = _controller.AddRating(model);

            // Assert
            _mockRatingService.Verify(s => s.DeleteById(existingRating.Id), Times.Once);
            _mockRecipeService.Verify(s => s.AddRating(recipe, It.Is<RecipeRating>(r => r.Comment == model.Comment)), Times.Once);
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Details");
        }
    }
}