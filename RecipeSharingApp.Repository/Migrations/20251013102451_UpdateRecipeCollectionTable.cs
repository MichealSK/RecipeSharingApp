using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeSharingApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecipeCollectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeCollections_RecipeCollectionId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_RecipeCollectionId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "RecipeCollectionId",
                table: "Recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RecipeCollectionId",
                table: "Recipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_RecipeCollectionId",
                table: "Recipes",
                column: "RecipeCollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeCollections_RecipeCollectionId",
                table: "Recipes",
                column: "RecipeCollectionId",
                principalTable: "RecipeCollections",
                principalColumn: "Id");
        }
    }
}
