using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservedBy",
                table: "Reservations",
                newName: "CustomerEmail");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Email);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerEmail",
                table: "Reservations",
                column: "CustomerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_CustomerEmail",
                table: "Reservations",
                column: "CustomerEmail",
                principalTable: "Customers",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_CustomerEmail",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_CustomerEmail",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                table: "Reservations",
                newName: "ReservedBy");
        }
    }
}
