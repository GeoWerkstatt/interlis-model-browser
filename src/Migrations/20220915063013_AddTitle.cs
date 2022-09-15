using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelRepoBrowser.Migrations;

public partial class AddTitle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Title",
            table: "Models",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Title",
            table: "Models");
    }
}
