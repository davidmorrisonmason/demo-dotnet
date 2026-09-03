using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BasketExpirationTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BasketExpirationTime",
                table: "Baskets",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasketExpirationTime",
                table: "Baskets");
        }
    }
}
