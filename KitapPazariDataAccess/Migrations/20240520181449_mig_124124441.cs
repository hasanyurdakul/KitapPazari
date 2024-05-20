using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitapPazariDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig_124124441 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntendId",
                table: "OrderHeaders",
                newName: "PaymentIntentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "OrderHeaders",
                newName: "PaymentIntendId");
        }
    }
}
