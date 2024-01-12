using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrnetgroup.Wms.EntityframeworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsToWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Workers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Workers");
        }
    }
}
