using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TestsUI
{
    [TestFixture]
    public class RecipeViewUITests : BaseUiTest
    {
        [Test]
        public async Task User_Can_Navigate_To_Details_And_Add_Review()
        {
            await LoginAsTestUser();

            await Page.GotoAsync($"{BaseUrl}/RecipeView");

            await Page.GetByRole(AriaRole.Link, new() { Name = "Details" }).First.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*/RecipeView/Details/.*"));

            await Page.GetByRole(AriaRole.Link, new() { Name = "Add Rating" }).ClickAsync();

            string testComment = "Delicious! Made this for dinner. " + Guid.NewGuid().ToString()[..4];
            await Page.GetByLabel("Rating (0-5)").FillAsync("5");
            await Page.GetByLabel("Comment").FillAsync(testComment);

            await Page.GetByRole(AriaRole.Button, new() { Name = "Submit Review" }).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(".*/RecipeView/Details/.*"));

            await Expect(Page.GetByText(testComment)).ToBeVisibleAsync();
        }

        [Test]
        public async Task User_Can_Pin_Recipe_From_Details_Page()
        {
            await LoginAsTestUser();

            await Page.GotoAsync($"{BaseUrl}/RecipeView/Details/e2568484-8def-4041-bb42-04737a535b77");

            await Page.GetByRole(AriaRole.Button, new() { Name = "Pin This Recipe" }).ClickAsync();

            var collectionsList = Page.Locator("#collections-list");
            await Expect(collectionsList).ToBeVisibleAsync();

            var collectionBtn = Page.Locator(".collection-select-btn").First;

            if (await collectionBtn.CountAsync() > 0)
            {
                await collectionBtn.ClickAsync();

                await Expect(Page.Locator("#pinToCollectionModal")).Not.ToBeVisibleAsync();
            }
            else
            {
                Assert.Ignore("No collections exist to pin to.");
            }
        }
    }
}