namespace Gestion_Projet.Models;

/// <summary>
/// Statut d'un projet
/// </summary>
public enum StatutProjet
{
    Planification = 0,
    EnCours = 1,
    EnPause = 2,
    Termine = 3,
    Annule = 4
}

/// <summary>
/// Statut d'une tâche
/// </summary>
public enum StatutTache
{
    AFaire = 0,
    EnCours = 1,
    EnRevue = 2,
    Terminee = 3
}

/// <summary>
/// Niveau de priorité d'une tâche
/// </summary>
public enum Priorite
{
    Basse = 1,
    Moyenne = 2,
    Haute = 3,
    Critique = 4
}
