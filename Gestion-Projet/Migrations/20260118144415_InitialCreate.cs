using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_Projet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Membres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Poste = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PhotoUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DateEmbauche = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFinPrevue = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ChefProjetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projets", x => x.Id);
                    table.CheckConstraint("CK_Projet_Budget", "\"Budget\" IS NULL OR \"Budget\" >= 0");
                    table.CheckConstraint("CK_Projet_DateFinPrevue", "\"DateFinPrevue\" IS NULL OR \"DateFinPrevue\" >= \"DateDebut\"");
                    table.ForeignKey(
                        name: "FK_Projets_Membres_ChefProjetId",
                        column: x => x.ChefProjetId,
                        principalTable: "Membres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Taches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ProjetId = table.Column<int>(type: "INTEGER", nullable: false),
                    MembreId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateEcheance = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Priorite = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    EstTerminee = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taches_Membres_MembreId",
                        column: x => x.MembreId,
                        principalTable: "Membres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Taches_Projets_ProjetId",
                        column: x => x.ProjetId,
                        principalTable: "Projets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Membres_Email",
                table: "Membres",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projets_ChefProjetId",
                table: "Projets",
                column: "ChefProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_MembreId",
                table: "Taches",
                column: "MembreId");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_ProjetId",
                table: "Taches",
                column: "ProjetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Taches");

            migrationBuilder.DropTable(
                name: "Projets");

            migrationBuilder.DropTable(
                name: "Membres");
        }
    }
}
