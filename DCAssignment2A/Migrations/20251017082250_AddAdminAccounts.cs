using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DCAssignment2A.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "UserProfileId", "address", "email", "name", "password", "phoneNumber", "profilePicture" },
                values: new object[,]
                {
                    { 1, "Head Office", "admin1@bankapp.com", "admin1", "adminpass", "0411122233", null },
                    { 2, "Head Office", "admin2@bankapp.com", "admin2", "adminpass", "0411122234", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileId",
                keyValue: 2);
        }
    }
}
