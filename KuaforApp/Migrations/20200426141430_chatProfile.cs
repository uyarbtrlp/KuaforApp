using Microsoft.EntityFrameworkCore.Migrations;

namespace KuaforApp.Migrations
{
    public partial class chatProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreProfile",
                table: "Chats",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserProfile",
                table: "Chats",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreProfile",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserProfile",
                table: "Chats");
        }
    }
}
