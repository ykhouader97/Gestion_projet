using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_Projet.Models;

/// <summary>
/// Représente un projet
/// </summary>
public class Projet
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Le nom du projet est obligatoire")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    [Display(Name = "Nom du projet")]
    public string Nom { get; set; } = string.Empty;
    
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "La date de début est obligatoire")]
    [DataType(DataType.Date)]
    [Display(Name = "Date de début")]
    public DateTime DateDebut { get; set; }
    
    [DataType(DataType.Date)]
    [Display(Name = "Date de fin prévue")]
    public DateTime? DateFinPrevue { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Le budget doit être positif")]
    [Display(Name = "Budget (DH)")]
    public decimal? Budget { get; set; }
    
    [Required(ErrorMessage = "Le chef de projet est obligatoire")]
    [Display(Name = "Chef de projet")]
    public int ChefProjetId { get; set; }
    
    [Display(Name = "Statut")]
    public StatutProjet Statut { get; set; } = StatutProjet.EnCours;
    
    [Display(Name = "Date de création")]
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [Display(Name = "Chef de projet")]
    public MembreEquipe? ChefProjet { get; set; }
    
    [Display(Name = "Tâches")]
    public ICollection<Tache> Taches { get; set; } = new List<Tache>();
    
    // Computed properties
    [NotMapped]
    [Display(Name = "En retard")]
    public bool EstEnRetard => DateFinPrevue.HasValue && 
                               DateFinPrevue.Value < DateTime.UtcNow.Date && 
                               Statut != StatutProjet.Termine && 
                               Statut != StatutProjet.Annule;
    
    [NotMapped]
    [Display(Name = "Pourcentage d'avancement")]
    public int PourcentageAvancement
    {
        get
        {
            if (Taches == null || !Taches.Any()) return 0;
            var totalTaches = Taches.Count;
            var tachesTerminees = Taches.Count(t => t.EstTerminee);
            return (int)((double)tachesTerminees / totalTaches * 100);
        }
    }
}
