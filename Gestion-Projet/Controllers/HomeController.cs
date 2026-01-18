using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gestion_Projet.Models;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Projet.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        
        // Statistiques des projets
        var totalProjets = await _context.Projets.CountAsync();
        var projetsActifs = await _context.Projets.CountAsync(p => p.Statut == StatutProjet.EnCours);
        var projetsTermines = await _context.Projets.CountAsync(p => p.Statut == StatutProjet.Termine);
        var projetsEnRetard = await _context.Projets
            .CountAsync(p => p.DateFinPrevue.HasValue && 
                           p.DateFinPrevue.Value < today && 
                           p.Statut != StatutProjet.Termine && 
                           p.Statut != StatutProjet.Annule);
        
        // Statistiques des tâches
        var totalTaches = await _context.Taches.CountAsync();
        var tachesTerminees = await _context.Taches.CountAsync(t => t.EstTerminee);
        var tachesEnCours = await _context.Taches.CountAsync(t => t.Statut == StatutTache.EnCours);
        
        // 5 derniers projets créés
        var derniersProjets = await _context.Projets
            .Include(p => p.ChefProjet)
            .Include(p => p.Taches)
            .OrderByDescending(p => p.DateCreation)
            .Take(5)
            .ToListAsync();
        
        // Tâches urgentes (échéance < 3 jours)
        var tachesUrgentes = await _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.Membre)
            .Where(t => t.DateEcheance.HasValue && 
                       t.DateEcheance.Value <= today.AddDays(3) && 
                       t.DateEcheance.Value >= today &&
                       !t.EstTerminee)
            .OrderBy(t => t.DateEcheance)
            .ToListAsync();
        
        // Membres actifs
        var membresActifs = await _context.Membres
            .Where(m => m.EstActif)
            .Include(m => m.Taches)
            .OrderBy(m => m.Nom)
            .ToListAsync();
        
        // Passer les données à la vue
        ViewBag.TotalProjets = totalProjets;
        ViewBag.ProjetsActifs = projetsActifs;
        ViewBag.ProjetsTermines = projetsTermines;
        ViewBag.ProjetsEnRetard = projetsEnRetard;
        ViewBag.TotalTaches = totalTaches;
        ViewBag.TachesTerminees = tachesTerminees;
        ViewBag.TachesEnCours = tachesEnCours;
        ViewBag.DerniersProjets = derniersProjets;
        ViewBag.TachesUrgentes = tachesUrgentes;
        ViewBag.MembresActifs = membresActifs;
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}