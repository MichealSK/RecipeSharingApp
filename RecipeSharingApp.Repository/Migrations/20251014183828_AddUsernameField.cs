using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeSharingApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "RecipeRatings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "RecipeRatings");
        }
    }
}
