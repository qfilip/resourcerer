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
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Element_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompositePrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompositeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositePrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompositePrice_Composite_CompositeId",
                        column: x => x.CompositeId,
                        principalTable: "Composite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementPrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementPrice_Element_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementPurchasedEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NumOfUnits = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<double>(type: "REAL", nullable: false),
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
                name: "CompositeSoldEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompositeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PriceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompositeSoldEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompositeSoldEvent_CompositePrice_PriceId",
                        column: x => x.PriceId,
                        principalTable: "CompositePrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompositeSoldEvent_Composite_CompositeId",
                        column: x => x.CompositeId,
                        principalTable: "Composite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementSoldEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ElementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PriceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementSoldEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementSoldEvent_ElementPrice_PriceId",
                        column: x => x.PriceId,
                        principalTable: "ElementPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElementSoldEvent_Element_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Element",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_CompositePrice_CompositeId",
                table: "CompositePrice",
                column: "CompositeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompositeSoldEvent_CompositeId",
                table: "CompositeSoldEvent",
                column: "CompositeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompositeSoldEvent_PriceId",
                table: "CompositeSoldEvent",
                column: "PriceId");

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
                name: "IX_ElementPrice_ElementId",
                table: "ElementPrice",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementPurchasedEvent_ElementId",
                table: "ElementPurchasedEvent",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementSoldEvent_ElementId",
                table: "ElementSoldEvent",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementSoldEvent_PriceId",
                table: "ElementSoldEvent",
                column: "PriceId");

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
                name: "CompositePrice");

            migrationBuilder.DropTable(
                name: "ElementPrice");

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
