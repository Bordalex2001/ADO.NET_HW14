using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesClassLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGamesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CopiesAreSold",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameMode",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Game_GameMode",
                table: "Games",
                sql: "GameMode IN ('Single-player', 'Multiplayer')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Game_GameMode",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CopiesAreSold",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameMode",
                table: "Games");
        }
    }
}
