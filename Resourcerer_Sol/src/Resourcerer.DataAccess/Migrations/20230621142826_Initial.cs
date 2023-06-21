using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resourcerer.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Claims = table.Column<string>(type: "TEXT", nullable: true),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Composite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Composite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Composite_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Element",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UnitOfMeasureId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Element", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Element_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Element_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompositeSoldEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitsSold = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceByUnit = table.Column<double>(type: "REAL", nullable: false),
                    CompositeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositeSoldEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompositeSoldEvent_Composite_CompositeId",
                        column: x => x.CompositeId,
                        principalTable: "Composite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementPurchasedEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitsBought = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceByUnit = table.Column<double>(type: "REAL", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementPurchasedEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementPurchasedEvent_Element_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElementPurchasedEvent_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ElementSoldEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitsSold = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceByUnit = table.Column<double>(type: "REAL", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementSoldEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementSoldEvent_Element_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElementSoldEvent_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Excerpt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompositeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<double>(type: "REAL", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excerpt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Excerpt_Composite_CompositeId",
                        column: x => x.CompositeId,
                        principalTable: "Composite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Excerpt_Element_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Excerpt_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CompositeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EntityStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Price_Composite_CompositeId",
                        column: x => x.CompositeId,
                        principalTable: "Composite",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Price_Element_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Element",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Name",
                table: "AppUser",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Composite_CategoryId",
                table: "Composite",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Composite_Name",
                table: "Composite",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompositeSoldEvent_CompositeId",
                table: "CompositeSoldEvent",
                column: "CompositeId");

            migrationBuilder.CreateIndex(
                name: "IX_Element_CategoryId",
                table: "Element",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Element_Name",
                table: "Element",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Element_UnitOfMeasureId",
                table: "Element",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementPurchasedEvent_ElementId",
                table: "ElementPurchasedEvent",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementPurchasedEvent_UnitOfMeasureId",
                table: "ElementPurchasedEvent",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementSoldEvent_ElementId",
                table: "ElementSoldEvent",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementSoldEvent_UnitOfMeasureId",
                table: "ElementSoldEvent",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Excerpt_CompositeId",
                table: "Excerpt",
                column: "CompositeId");

            migrationBuilder.CreateIndex(
                name: "IX_Excerpt_ElementId",
                table: "Excerpt",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Excerpt_UnitOfMeasureId",
                table: "Excerpt",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_CompositeId",
                table: "Price",
                column: "CompositeId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_ElementId",
                table: "Price",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasure_Name",
                table: "UnitOfMeasure",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "CompositeSoldEvent");

            migrationBuilder.DropTable(
                name: "ElementPurchasedEvent");

            migrationBuilder.DropTable(
                name: "ElementSoldEvent");

            migrationBuilder.DropTable(
                name: "Excerpt");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "Composite");

            migrationBuilder.DropTable(
                name: "Element");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "UnitOfMeasure");
        }
    }
}
