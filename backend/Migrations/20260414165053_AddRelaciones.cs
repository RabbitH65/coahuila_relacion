using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoahuilaRelacion.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Angulo",
                table: "DeclaracionImagenes");

            migrationBuilder.CreateTable(
                name: "Actores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    NombreNormalizado = table.Column<string>(type: "TEXT", nullable: true),
                    Alias = table.Column<string>(type: "TEXT", nullable: true),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Circunstancias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    NombreNormalizado = table.Column<string>(type: "TEXT", nullable: true),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circunstancias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    NombreNormalizado = table.Column<string>(type: "TEXT", nullable: true),
                    Tipo = table.Column<string>(type: "TEXT", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", nullable: true),
                    Municipio = table.Column<string>(type: "TEXT", nullable: true),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionActores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeclaracionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Origen = table.Column<string>(type: "TEXT", nullable: true),
                    Confianza = table.Column<double>(type: "REAL", nullable: true),
                    Confirmado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionActores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionActores_Actores_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeclaracionActores_Declaraciones_DeclaracionId",
                        column: x => x.DeclaracionId,
                        principalTable: "Declaraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionCircunstancias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeclaracionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CircunstanciaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Origen = table.Column<string>(type: "TEXT", nullable: true),
                    Confianza = table.Column<double>(type: "REAL", nullable: true),
                    Confirmado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionCircunstancias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionCircunstancias_Circunstancias_CircunstanciaId",
                        column: x => x.CircunstanciaId,
                        principalTable: "Circunstancias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeclaracionCircunstancias_Declaraciones_DeclaracionId",
                        column: x => x.DeclaracionId,
                        principalTable: "Declaraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionLugares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeclaracionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LugarId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Origen = table.Column<string>(type: "TEXT", nullable: true),
                    Confianza = table.Column<double>(type: "REAL", nullable: true),
                    Confirmado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionLugares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionLugares_Declaraciones_DeclaracionId",
                        column: x => x.DeclaracionId,
                        principalTable: "Declaraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeclaracionLugares_Lugares_LugarId",
                        column: x => x.LugarId,
                        principalTable: "Lugares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actores_NombreNormalizado",
                table: "Actores",
                column: "NombreNormalizado");

            migrationBuilder.CreateIndex(
                name: "IX_Circunstancias_NombreNormalizado",
                table: "Circunstancias",
                column: "NombreNormalizado");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionActores_ActorId",
                table: "DeclaracionActores",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionActores_DeclaracionId_ActorId",
                table: "DeclaracionActores",
                columns: new[] { "DeclaracionId", "ActorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionCircunstancias_CircunstanciaId",
                table: "DeclaracionCircunstancias",
                column: "CircunstanciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionCircunstancias_DeclaracionId_CircunstanciaId",
                table: "DeclaracionCircunstancias",
                columns: new[] { "DeclaracionId", "CircunstanciaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionLugares_DeclaracionId_LugarId",
                table: "DeclaracionLugares",
                columns: new[] { "DeclaracionId", "LugarId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionLugares_LugarId",
                table: "DeclaracionLugares",
                column: "LugarId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_NombreNormalizado",
                table: "Lugares",
                column: "NombreNormalizado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeclaracionActores");

            migrationBuilder.DropTable(
                name: "DeclaracionCircunstancias");

            migrationBuilder.DropTable(
                name: "DeclaracionLugares");

            migrationBuilder.DropTable(
                name: "Actores");

            migrationBuilder.DropTable(
                name: "Circunstancias");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.AddColumn<string>(
                name: "Angulo",
                table: "DeclaracionImagenes",
                type: "TEXT",
                nullable: true);
        }
    }
}
