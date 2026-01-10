using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditOrderandOrderDetailsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_OrderDate",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TotalOrderPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalOrderPrice",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UserAddress",
                table: "Orders",
                newName: "DeliveryAddress");

            migrationBuilder.RenameColumn(
                name: "TotalTaxPrice",
                table: "Orders",
                newName: "ServiceFeePrice");

            migrationBuilder.RenameColumn(
                name: "TotalMealsPrice",
                table: "Orders",
                newName: "SubPrice");

            migrationBuilder.RenameColumn(
                name: "MealDeteils",
                table: "CartDetails",
                newName: "MealDetails");

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "DeliveryAddressLocation",
                table: "Orders",
                type: "geography",
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Orders",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AddOnsPrice",
                table: "OrderDetails",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsPrice",
                table: "OrderDetails",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MealDetails",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[SubPrice] + [ServiceFeePrice] + [DeliveryFee]",
                stored: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "OrderDetails",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[MealQuantity] * ([MealPrice] + [ItemsPrice] + [AddOnsPrice])",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldComputedColumnSql: "[MealPrice] * [MealQuantity]",
                oldStored: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerPhoneNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressLocation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AddOnsPrice",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ItemsPrice",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MealDetails",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "SubPrice",
                table: "Orders",
                newName: "TotalMealsPrice");

            migrationBuilder.RenameColumn(
                name: "ServiceFeePrice",
                table: "Orders",
                newName: "TotalTaxPrice");

            migrationBuilder.RenameColumn(
                name: "DeliveryAddress",
                table: "Orders",
                newName: "UserAddress");

            migrationBuilder.RenameColumn(
                name: "MealDetails",
                table: "CartDetails",
                newName: "MealDeteils");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOrderPrice",
                table: "Orders",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[TotalMealsPrice] + [TotalTaxPrice]");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "OrderDetails",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[MealPrice] * [MealQuantity]",
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldComputedColumnSql: "[MealQuantity] * ([MealPrice] + [ItemsPrice] + [AddOnsPrice])");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_OrderDate",
                table: "Orders",
                columns: new[] { "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TotalOrderPrice",
                table: "Orders",
                column: "TotalOrderPrice");
        }
    }
}
