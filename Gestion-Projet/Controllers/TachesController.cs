using Gestion_Projet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Projet.Controllers;

public class TachesController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public TachesController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Taches
    public async Task<IActionResult> Index(string searchString, int? projetFilter, int? membreFilter, 
        StatutTache? statutFilter, Priorite? prioriteFilter, bool? enRetardOnly)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["ProjetFilter"] = projetFilter;
        ViewData["MembreFilter"] = membreFilter;
        ViewData["StatutFilter"] = statutFilter;
        ViewData["PrioriteFilter"] = prioriteFilter;
        ViewData["EnRetardOnly"] = enRetardOnly ?? false;
        
        ViewBag.Projets = new SelectList(_context.Projets, "Id", "Nom");
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet");
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutTache)).Cast<StatutTache>());
        ViewBag.Priorites = new SelectList(Enum.GetValues(typeof(Priorite)).Cast<Priorite>());
        
        var taches = _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.Membre)
            .AsQueryable();
        
        // Recherche
        if (!string.IsNullOrEmpty(searchString))
        {
            taches = taches.Where(t => t.Titre.Contains(searchString));
        }
        
        // Filtres
        if (projetFilter.HasValue)
        {
            taches = taches.Where(t => t.ProjetId == projetFilter);
        }
        
        if (membreFilter.HasValue)
        {
            taches = taches.Where(t => t.MembreId == membreFilter);
        }
        
        if (statutFilter.HasValue)
        {
            taches = taches.Where(t => t.Statut == statutFilter);
        }
        
        if (prioriteFilter.HasValue)
        {
            taches = taches.Where(t => t.Priorite == prioriteFilter);
        }
        
        if (enRetardOnly ?? false)
        {
            var today = DateTime.UtcNow.Date;
            taches = taches.Where(t => t.DateEcheance.HasValue && 
                                      t.DateEcheance.Value < today && 
                                      !t.EstTerminee);
        }
        
        return View(await taches.OrderBy(t => t.DateEcheance).ToListAsync());
    }
    
    // GET: Taches/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var tache = await _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.Membre)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (tache == null)
        {
            return NotFound();
        }
        
        return View(tache);
    }
    
    // GET: Taches/Create
    public IActionResult Create(int? projetId)
    {
        ViewBag.Projets = new SelectList(_context.Projets, "Id", "Nom", projetId);
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet");
        ViewBag.Priorites = new SelectList(Enum.GetValues(typeof(Priorite)).Cast<Priorite>(), Priorite.Moyenne);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutTache)).Cast<StatutTache>(), StatutTache.AFaire);
        
        var tache = new Tache();
        if (projetId.HasValue)
        {
            tache.ProjetId = projetId.Value;
        }
        
        return View(tache);
    }
    
    // POST: Taches/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titre,Description,ProjetId,MembreId,DateEcheance,Priorite,Statut")] Tache tache)
    {
        if (ModelState.IsValid)
        {
            if (tache.DateEcheance.HasValue)
                tache.DateEcheance = DateTime.SpecifyKind(tache.DateEcheance.Value, DateTimeKind.Utc);

            tache.DateCreation = DateTime.UtcNow;
            tache.EstTerminee = tache.Statut == StatutTache.Terminee;
            _context.Add(tache);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tâche créée avec succès!";
            
            // Rediriger vers les détails du projet si on vient de là
            if (Request.Headers["Referer"].ToString().Contains("/Projets/Details/"))
            {
                return RedirectToAction("Details", "Projets", new { id = tache.ProjetId });
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Projets = new SelectList(_context.Projets, "Id", "Nom", tache.ProjetId);
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet", tache.MembreId);
        ViewBag.Priorites = new SelectList(Enum.GetValues(typeof(Priorite)).Cast<Priorite>(), tache.Priorite);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutTache)).Cast<StatutTache>(), tache.Statut);
        return View(tache);
    }
    
    // GET: Taches/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var tache = await _context.Taches.FindAsync(id);
        if (tache == null)
        {
            return NotFound();
        }
        
        ViewBag.Projets = new SelectList(_context.Projets, "Id", "Nom", tache.ProjetId);
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet", tache.MembreId);
        ViewBag.Priorites = new SelectList(Enum.GetValues(typeof(Priorite)).Cast<Priorite>(), tache.Priorite);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutTache)).Cast<StatutTache>(), tache.Statut);
        return View(tache);
    }
    
    // POST: Taches/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,ProjetId,MembreId,DateEcheance,Priorite,Statut,DateCreation")] Tache tache)
    {
        if (id != tache.Id)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                if (tache.DateEcheance.HasValue)
                    tache.DateEcheance = DateTime.SpecifyKind(tache.DateEcheance.Value, DateTimeKind.Utc);
                tache.DateCreation = DateTime.SpecifyKind(tache.DateCreation, DateTimeKind.Utc);

                tache.EstTerminee = tache.Statut == StatutTache.Terminee;
                _context.Update(tache);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tâche modifiée avec succès!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TacheExists(tache.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Projets = new SelectList(_context.Projets, "Id", "Nom", tache.ProjetId);
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet", tache.MembreId);
        ViewBag.Priorites = new SelectList(Enum.GetValues(typeof(Priorite)).Cast<Priorite>(), tache.Priorite);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutTache)).Cast<StatutTache>(), tache.Statut);
        return View(tache);
    }
    
    // GET: Taches/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var tache = await _context.Taches
            .Include(t => t.Projet)
            .Include(t => t.Membre)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (tache == null)
        {
            return NotFound();
        }
        
        return View(tache);
    }
    
    // POST: Taches/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tache = await _context.Taches.FindAsync(id);
        if (tache != null)
        {
            _context.Taches.Remove(tache);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tâche supprimée avec succès!";
        }
        
        return RedirectToAction(nameof(Index));
    }
    
    // POST: Taches/ToggleComplete/5
    [HttpPost]
    public async Task<IActionResult> ToggleComplete(int id)
    {
        var tache = await _context.Taches.FindAsync(id);
        if (tache == null)
        {
            return NotFound();
        }
        
        tache.EstTerminee = !tache.EstTerminee;
        tache.Statut = tache.EstTerminee ? StatutTache.Terminee : StatutTache.EnCours;
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
    
    private bool TacheExists(int id)
    {
        return _context.Taches.Any(e => e.Id == id);
    }
}
