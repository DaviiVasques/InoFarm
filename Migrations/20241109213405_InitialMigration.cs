using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InoFarm.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gerentes",
                columns: table => new
                {
                    GerenteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Sobrenome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    DataAdmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gerentes", x => x.GerenteId);
                });

            migrationBuilder.CreateTable(
                name: "Frutas",
                columns: table => new
                {
                    FrutaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    formaVenda = table.Column<string>(type: "text", nullable: false),
                    GerenteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frutas", x => x.FrutaId);
                    table.ForeignKey(
                        name: "FK_Frutas_Gerentes_GerenteId",
                        column: x => x.GerenteId,
                        principalTable: "Gerentes",
                        principalColumn: "GerenteId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Sobrenome = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    GerenteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Gerentes_GerenteId",
                        column: x => x.GerenteId,
                        principalTable: "Gerentes",
                        principalColumn: "GerenteId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Carrinhos",
                columns: table => new
                {
                    CarrinhoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carrinhos", x => x.CarrinhoId);
                    table.ForeignKey(
                        name: "FK_Carrinhos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarrinhoItens",
                columns: table => new
                {
                    carrinhoItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCarrinho = table.Column<int>(type: "integer", nullable: false),
                    IdFruta = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrinhoItens", x => x.carrinhoItemId);
                    table.ForeignKey(
                        name: "FK_CarrinhoItens_Carrinhos_IdCarrinho",
                        column: x => x.IdCarrinho,
                        principalTable: "Carrinhos",
                        principalColumn: "CarrinhoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarrinhoItens_Frutas_IdFruta",
                        column: x => x.IdFruta,
                        principalTable: "Frutas",
                        principalColumn: "FrutaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItens_IdCarrinho",
                table: "CarrinhoItens",
                column: "IdCarrinho");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItens_IdFruta",
                table: "CarrinhoItens",
                column: "IdFruta");

            migrationBuilder.CreateIndex(
                name: "IX_Carrinhos_IdUsuario",
                table: "Carrinhos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Frutas_GerenteId",
                table: "Frutas",
                column: "GerenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GerenteId",
                table: "Usuarios",
                column: "GerenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrinhoItens");

            migrationBuilder.DropTable(
                name: "Carrinhos");

            migrationBuilder.DropTable(
                name: "Frutas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Gerentes");
        }
    }
}
