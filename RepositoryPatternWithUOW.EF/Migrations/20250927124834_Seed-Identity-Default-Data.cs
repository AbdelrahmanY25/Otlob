using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "757e63f2-b6d2-44b8-92bf-bbd8e0b18faa", "38922526-75d6-46a4-beb3-aa23e540f8ef", false, "RestaurantAdmin", "RESTAURANTADMIN" },
                    { "8f727b3f-5028-4804-a9ed-f746ed83ab7f", "534d6a39-d52f-4b2d-864b-efeafd39df1e", false, "SuperAdmin", "SUPERADMIN" },
                    { "ccb604b3-3d5f-43d9-acb2-45deba2252be", "03cfba10-053e-4975-b27e-6f8fa82e22c3", true, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "BirthDate", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "Image", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "83524a49-2ad7-4800-ab6b-4d7912c4833b", 0, null, "5b81e95c-681e-44c9-9b6d-26e65c62a2fc", "justotlob@gmail.com", true, "Otlob", null, null, "Admin", false, null, "JUSTOTLOB@GMAIL.COM", "OTLOB", "AQAAAAIAAYagAAAAEMH3iMEhrE3oE9fNO45Vw6VSV0qK25Pdbxjml3y9RB0opyIRfrNJijp6e0VZ3ZxfWg==", null, false, "1db72ac7954d419ca8d8e5e64427e951", false, "Otlob" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8f727b3f-5028-4804-a9ed-f746ed83ab7f", "83524a49-2ad7-4800-ab6b-4d7912c4833b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "757e63f2-b6d2-44b8-92bf-bbd8e0b18faa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ccb604b3-3d5f-43d9-acb2-45deba2252be");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8f727b3f-5028-4804-a9ed-f746ed83ab7f", "83524a49-2ad7-4800-ab6b-4d7912c4833b" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f727b3f-5028-4804-a9ed-f746ed83ab7f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "83524a49-2ad7-4800-ab6b-4d7912c4833b");
        }
    }
}
