using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_Projet.Models;

/// <summary>
/// Représente une tâche dans un projet
/// </summary>
public class Tache
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Le titre est obligatoire")]
    [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
    [Display(Name = "Titre")]
    public string Titre { get; set; } = string.Empty;
    
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Le projet est obligatoire")]
    [Display(Name = "Projet")]
    public int ProjetId { get; set; }
    
    [Display(Name = "Membre assigné")]
    public int? MembreId { get; set; }
    
    [DataType(DataType.Date)]
    [Display(Name = "Date d'échéance")]
    public DateTime? DateEcheance { get; set; }
    
    [Display(Name = "Priorité")]
    public Priorite Priorite { get; set; } = Priorite.Moyenne;
    
    [Display(Name = "Statut")]
    public StatutTache Statut { get; set; } = StatutTache.AFaire;
    
    [Display(Name = "Terminée")]
    public bool EstTerminee { get; set; } = false;
    
    [Display(Name = "Date de création")]
    public DateTime DateCreation { get; set; } = DateTime.Now;
    
    // Navigation properties
    [Display(Name = "Projet")]
    public Projet? Projet { get; set; }
    
    [Display(Name = "Membre assigné")]
    public MembreEquipe? Membre { get; set; }
    
    // Computed properties
    [NotMapped]
    [Display(Name = "En retard")]
    public bool EstEnRetard => DateEcheance.HasValue && 
                               DateEcheance.Value < DateTime.Today && 
                               !EstTerminee;
    
    [NotMapped]
    [Display(Name = "Urgente")]
    public bool EstUrgente => DateEcheance.HasValue && 
                              DateEcheance.Value <= DateTime.Today.AddDays(3) && 
                              DateEcheance.Value >= DateTime.Today &&
                              !EstTerminee;
}
