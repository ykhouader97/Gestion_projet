using Gestion_Projet.Models;

namespace Gestion_Projet.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Vérifier si la base de données contient déjà des données
        if (context.Membres.Any())
        {
            return; // La base de données a déjà été initialisée
        }

        // Créer des membres
        var membres = new[]
        {
            new MembreEquipe
            {
                Nom = "Alami",
                Prenom = "Hassan",
                Email = "hassan.alami@example.com",
                Poste = "Chef de Projet Senior",
                DateEmbauche = new DateTime(2020, 1, 15),
                EstActif = true
            },
            new MembreEquipe
            {
                Nom = "Benali",
                Prenom = "Fatima",
                Email = "fatima.benali@example.com",
                Poste = "Développeuse Full Stack",
                DateEmbauche = new DateTime(2021, 3, 10),
                EstActif = true
            },
            new MembreEquipe
            {
                Nom = "Chakir",
                Prenom = "Mohammed",
                Email = "mohammed.chakir@example.com",
                Poste = "Designer UX/UI",
                DateEmbauche = new DateTime(2021, 6, 1),
                EstActif = true
            },
            new MembreEquipe
            {
                Nom = "Douiri",
                Prenom = "Amina",
                Email = "amina.douiri@example.com",
                Poste = "Développeuse Backend",
                DateEmbauche = new DateTime(2022, 2, 20),
                EstActif = true
            },
            new MembreEquipe
            {
                Nom = "El Fassi",
                Prenom = "Youssef",
                Email = "youssef.elfassi@example.com",
                Poste = "Testeur QA",
                DateEmbauche = new DateTime(2022, 9, 5),
                EstActif = true
            }
        };

        context.Membres.AddRange(membres);
        context.SaveChanges();

        // Créer des projets
        var projets = new[]
        {
            new Projet
            {
                Nom = "Plateforme E-Commerce",
                Description = "Développement d'une plateforme de vente en ligne moderne avec gestion des stocks et paiements sécurisés",
                DateDebut = new DateTime(2024, 1, 10),
                DateFinPrevue = new DateTime(2024, 6, 30),
                Budget = 150000,
                ChefProjetId = membres[0].Id,
                Statut = StatutProjet.EnCours,
                DateCreation = DateTime.Now.AddMonths(-3)
            },
            new Projet
            {
                Nom = "Application Mobile Banking",
                Description = "Application mobile pour la gestion bancaire avec authentification biométrique",
                DateDebut = new DateTime(2024, 2, 1),
                DateFinPrevue = new DateTime(2024, 8, 31),
                Budget = 200000,
                ChefProjetId = membres[0].Id,
                Statut = StatutProjet.EnCours,
                DateCreation = DateTime.Now.AddMonths(-2)
            },
            new Projet
            {
                Nom = "Système de Gestion RH",
                Description = "Système complet de gestion des ressources humaines avec suivi des congés et évaluations",
                DateDebut = new DateTime(2023, 10, 1),
                DateFinPrevue = new DateTime(2024, 3, 31),
                Budget = 80000,
                ChefProjetId = membres[0].Id,
                Statut = StatutProjet.Termine,
                DateCreation = DateTime.Now.AddMonths(-6)
            }
        };

        context.Projets.AddRange(projets);
        context.SaveChanges();

        // Créer des tâches
        var taches = new[]
        {
            // Tâches pour E-Commerce
            new Tache
            {
                Titre = "Conception de la base de données",
                Description = "Modéliser et créer le schéma de la base de données",
                ProjetId = projets[0].Id,
                MembreId = membres[3].Id,
                DateEcheance = new DateTime(2024, 2, 15),
                Priorite = Priorite.Haute,
                Statut = StatutTache.Terminee,
                EstTerminee = true,
                DateCreation = DateTime.Now.AddMonths(-3)
            },
            new Tache
            {
                Titre = "Développement API produits",
                Description = "Créer les endpoints REST pour la gestion des produits",
                ProjetId = projets[0].Id,
                MembreId = membres[1].Id,
                DateEcheance = new DateTime(2024, 3, 20),
                Priorite = Priorite.Haute,
                Statut = StatutTache.EnCours,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddMonths(-2)
            },
            new Tache
            {
                Titre = "Design interface utilisateur",
                Description = "Créer les maquettes et prototypes de l'interface",
                ProjetId = projets[0].Id,
                MembreId = membres[2].Id,
                DateEcheance = new DateTime(2024, 3, 10),
                Priorite = Priorite.Moyenne,
                Statut = StatutTache.EnRevue,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddMonths(-2)
            },
            new Tache
            {
                Titre = "Intégration passerelle paiement",
                Description = "Intégrer Stripe pour les paiements en ligne",
                ProjetId = projets[0].Id,
                MembreId = membres[1].Id,
                DateEcheance = new DateTime(2024, 4, 30),
                Priorite = Priorite.Critique,
                Statut = StatutTache.AFaire,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddMonths(-1)
            },
            new Tache
            {
                Titre = "Tests de sécurité",
                Description = "Effectuer des tests de pénétration et d'audit de sécurité",
                ProjetId = projets[0].Id,
                MembreId = membres[4].Id,
                DateEcheance = new DateTime(2024, 5, 15),
                Priorite = Priorite.Haute,
                Statut = StatutTache.AFaire,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddMonths(-1)
            },
            
            // Tâches pour Mobile Banking
            new Tache
            {
                Titre = "Authentification biométrique",
                Description = "Implémenter Touch ID et Face ID",
                ProjetId = projets[1].Id,
                MembreId = membres[1].Id,
                DateEcheance = new DateTime(2024, 4, 15),
                Priorite = Priorite.Critique,
                Statut = StatutTache.EnCours,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddMonths(-1)
            },
            new Tache
            {
                Titre = "Module de virements",
                Description = "Développer la fonctionnalité de virements bancaires",
                ProjetId = projets[1].Id,
                MembreId = membres[3].Id,
                DateEcheance = new DateTime(2024, 5, 1),
                Priorite = Priorite.Haute,
                Statut = StatutTache.AFaire,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddDays(-14)
            },
            new Tache
            {
                Titre = "Design responsive",
                Description = "Adapter l'interface pour différentes tailles d'écran",
                ProjetId = projets[1].Id,
                MembreId = membres[2].Id,
                DateEcheance = DateTime.Today.AddDays(2), // Tâche urgente
                Priorite = Priorite.Haute,
                Statut = StatutTache.EnCours,
                EstTerminee = false,
                DateCreation = DateTime.Now.AddDays(-7)
            },
            
            // Tâches pour Système RH (projet terminé)
            new Tache
            {
                Titre = "Module de gestion des congés",
                Description = "Système de demande et validation des congés",
                ProjetId = projets[2].Id,
                MembreId = membres[1].Id,
                DateEcheance = new DateTime(2024, 1, 31),
                Priorite = Priorite.Haute,
                Statut = StatutTache.Terminee,
                EstTerminee = true,
                DateCreation = DateTime.Now.AddMonths(-5)
            },
            new Tache
            {
                Titre = "Rapports et statistiques",
                Description = "Génération de rapports RH et tableaux de bord",
                ProjetId = projets[2].Id,
                MembreId = membres[3].Id,
                DateEcheance = new DateTime(2024, 3, 15),
                Priorite = Priorite.Moyenne,
                Statut = StatutTache.Terminee,
                EstTerminee = true,
                DateCreation = DateTime.Now.AddMonths(-4)
            }
        };

        context.Taches.AddRange(taches);
        context.SaveChanges();
    }
}
