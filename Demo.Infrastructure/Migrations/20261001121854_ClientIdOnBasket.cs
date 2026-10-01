using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClientIdOnBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Baskets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_ClientId",
                table: "Baskets",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Clients_ClientId",
                table: "Baskets",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Clients_ClientId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_ClientId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Baskets");
        }
    }
}
