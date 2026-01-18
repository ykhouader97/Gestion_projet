using System.ComponentModel.DataAnnotations;

namespace Gestion_Projet.Models;

/// <summary>
/// Représente un membre de l'équipe
/// </summary>
public class MembreEquipe
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
    public string Nom { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
    public string Prenom { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères")]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Le poste ne peut pas dépasser 100 caractères")]
    public string? Poste { get; set; }
    
    [StringLength(200, ErrorMessage = "L'URL de la photo ne peut pas dépasser 200 caractères")]
    [Url(ErrorMessage = "Format d'URL invalide")]
    public string? PhotoUrl { get; set; }
    
    [DataType(DataType.Date)]
    [Display(Name = "Date d'embauche")]
    public DateTime? DateEmbauche { get; set; }
    
    [Display(Name = "Actif")]
    public bool EstActif { get; set; } = true;
    
    // Navigation properties
    [Display(Name = "Projets dirigés")]
    public ICollection<Projet> ProjetsLed { get; set; } = new List<Projet>();
    
    [Display(Name = "Tâches assignées")]
    public ICollection<Tache> Taches { get; set; } = new List<Tache>();
    
    // Computed property for display
    [Display(Name = "Nom complet")]
    public string NomComplet => $"{Prenom} {Nom}";
}
