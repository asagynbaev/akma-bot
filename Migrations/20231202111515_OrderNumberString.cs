using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindMate.Migrations
{
    /// <inheritdoc />
    public partial class OrderNumberString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Errors",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Dialogs",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Blacklists",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Errors");

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                table: "Dialogs",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "OrderNumber",
                table: "Blacklists",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
