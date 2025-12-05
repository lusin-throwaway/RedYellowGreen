using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedYellowGreen.Api.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CurrentState_StateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState_State = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentStateEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentStateEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentStateEntity_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderEntity_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CurrentState_State",
                table: "Equipment",
                column: "CurrentState_State");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentStateEntity_EquipmentId",
                table: "EquipmentStateEntity",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentStateEntity_State",
                table: "EquipmentStateEntity",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEntity_EquipmentId",
                table: "OrderEntity",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentStateEntity");

            migrationBuilder.DropTable(
                name: "OrderEntity");

            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
