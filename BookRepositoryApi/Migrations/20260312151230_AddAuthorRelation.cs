using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookRepositoryApi.Migrations
{
public partial class AddAuthorRelation : Migration
    {
protected override void Up(MigrationBuilder migrationBuilder)
        {
            // add the column nullable so existing rows don't violate the new constraint
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "integer",
                nullable: true);

            // if any books already exist, assign them to the first user (typically the seeded admin)
            migrationBuilder.Sql(
                "UPDATE \"Books\" SET \"AuthorId\" = (SELECT \"Id\" FROM \"Users\" ORDER BY \"Id\" LIMIT 1) WHERE \"AuthorId\" IS NULL;");

            // make the column required now that all rows have a value
            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Books");
        }
    }
}

