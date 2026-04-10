using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoahuilaRelacion.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", nullable: true),
                    Ap = table.Column<string>(type: "TEXT", nullable: true),
                    Tomo = table.Column<string>(type: "TEXT", nullable: true),
                    Foja = table.Column<string>(type: "TEXT", nullable: true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Entrevistado = table.Column<string>(type: "TEXT", nullable: true),
                    Declarante = table.Column<string>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Declaraciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegistroId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TextoDeclaracion = table.Column<string>(type: "TEXT", nullable: true),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: true),
                    FechaCaptura = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declaraciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Declaraciones_Registros_RegistroId",
                        column: x => x.RegistroId,
                        principalTable: "Registros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionCoordenadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeclaracionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Latitud = table.Column<double>(type: "REAL", nullable: false),
                    Longitud = table.Column<double>(type: "REAL", nullable: false),
                    PrecisionM = table.Column<double>(type: "REAL", nullable: true),
                    Fuente = table.Column<string>(type: "TEXT", nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionCoordenadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionCoordenadas_Declaraciones_DeclaracionId",
                        column: x => x.DeclaracionId,
                        principalTable: "Declaraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionFechas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeclaracionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoFecha = table.Column<string>(type: "TEXT", nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionFechas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionFechas_Declaraciones_DeclaracionId",
                        column: x => x.DeclaracionId,
                        principalTable: "Declaraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaracionImagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeclaracionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RutaArchivo = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    Angulo = table.Column<string>(type: "TEXT", nullable: true),
                    ElementoRelevante = table.Column<string>(type: "TEXT", nullable: true),
                    TipoImagen = table.Column<string>(type: "TEXT", nullable: true),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCaptura = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaracionImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaracionImagenes_Declaraciones_DeclaracionId",
                        column: x => x.DeclaracionId,
                        principalTable: "Declaraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionCoordenadas_DeclaracionId",
                table: "DeclaracionCoordenadas",
                column: "DeclaracionId");

            migrationBuilder.CreateIndex(
                name: "IX_Declaraciones_Orden",
                table: "Declaraciones",
                column: "Orden");

            migrationBuilder.CreateIndex(
                name: "IX_Declaraciones_RegistroId",
                table: "Declaraciones",
                column: "RegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionFechas_DeclaracionId",
                table: "DeclaracionFechas",
                column: "DeclaracionId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaracionImagenes_DeclaracionId",
                table: "DeclaracionImagenes",
                column: "DeclaracionId");

            migrationBuilder.CreateIndex(
                name: "IX_Registros_Fecha",
                table: "Registros",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Registros_Numero",
                table: "Registros",
                column: "Numero");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeclaracionCoordenadas");

            migrationBuilder.DropTable(
                name: "DeclaracionFechas");

            migrationBuilder.DropTable(
                name: "DeclaracionImagenes");

            migrationBuilder.DropTable(
                name: "Declaraciones");

            migrationBuilder.DropTable(
                name: "Registros");
        }
    }
}
