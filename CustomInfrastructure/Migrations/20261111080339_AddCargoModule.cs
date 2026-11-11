using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCargoModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CargoCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoCompanies_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CargoParcels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StoreOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CargoCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ArrivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeclarationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeclaredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoParcels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoParcels_CargoCompanies_CargoCompanyId",
                        column: x => x.CargoCompanyId,
                        principalTable: "CargoCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoParcels_Declarations_DeclarationId",
                        column: x => x.DeclarationId,
                        principalTable: "Declarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoParcels_StoreOrders_StoreOrderId",
                        column: x => x.StoreOrderId,
                        principalTable: "StoreOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoParcels_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoParcels_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CargoCompanies_Name",
                table: "CargoCompanies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoCompanies_WarehouseId",
                table: "CargoCompanies",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoParcels_CargoCompanyId",
                table: "CargoParcels",
                column: "CargoCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoParcels_DeclarationId",
                table: "CargoParcels",
                column: "DeclarationId",
                unique: true,
                filter: "[DeclarationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CargoParcels_OwnerUserId_Status",
                table: "CargoParcels",
                columns: new[] { "OwnerUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CargoParcels_StoreOrderId",
                table: "CargoParcels",
                column: "StoreOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoParcels_TrackingNumber",
                table: "CargoParcels",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoParcels_WarehouseId",
                table: "CargoParcels",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoParcels");

            migrationBuilder.DropTable(
                name: "CargoCompanies");
        }
    }
}
