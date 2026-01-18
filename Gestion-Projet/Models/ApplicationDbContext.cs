using Microsoft.EntityFrameworkCore;

namespace Gestion_Projet.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Projet> Projets { get; set; }
    public DbSet<MembreEquipe> Membres { get; set; }
    public DbSet<Tache> Taches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuration pour Projet
        modelBuilder.Entity<Projet>(entity =>
        {
            // Relation Projet -> ChefProjet (MembreEquipe)
            entity.HasOne(p => p.ChefProjet)
                .WithMany(m => m.ProjetsLed)
                .HasForeignKey(p => p.ChefProjetId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Check constraint: DateFinPrevue >= DateDebut
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Projet_DateFinPrevue", 
                "\"DateFinPrevue\" IS NULL OR \"DateFinPrevue\" >= \"DateDebut\""));
            
            // Check constraint: Budget >= 0
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Projet_Budget", 
                "\"Budget\" IS NULL OR \"Budget\" >= 0"));
        });
        
        // Configuration pour Tache
        modelBuilder.Entity<Tache>(entity =>
        {
            // Relation Tache -> Projet (cascade delete)
            entity.HasOne(t => t.Projet)
                .WithMany(p => p.Taches)
                .HasForeignKey(t => t.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relation Tache -> Membre (nullable, set null on delete)
            entity.HasOne(t => t.Membre)
                .WithMany(m => m.Taches)
                .HasForeignKey(t => t.MembreId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configuration pour MembreEquipe
        modelBuilder.Entity<MembreEquipe>(entity =>
        {
            // Index unique sur Email
            entity.HasIndex(m => m.Email)
                .IsUnique();
        });
    }
}