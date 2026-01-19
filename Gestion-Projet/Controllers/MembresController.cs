using Gestion_Projet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Projet.Controllers;

public class MembresController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public MembresController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Membres
    public async Task<IActionResult> Index(string searchString, bool? actifsOnly)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["ActifsOnly"] = actifsOnly ?? true;
        
        var membres = _context.Membres
            .Include(m => m.ProjetsLed)
            .Include(m => m.Taches)
            .AsQueryable();
        
        // Filtre actifs seulement
        if (actifsOnly ?? true)
        {
            membres = membres.Where(m => m.EstActif);
        }
        
        // Recherche
        if (!string.IsNullOrEmpty(searchString))
        {
            membres = membres.Where(m => 
                m.Nom.Contains(searchString) || 
                m.Prenom.Contains(searchString) ||
                m.Email.Contains(searchString));
        }
        
        return View(await membres.OrderBy(m => m.Nom).ToListAsync());
    }
    
    // GET: Membres/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var membre = await _context.Membres
            .Include(m => m.ProjetsLed)
            .Include(m => m.Taches)
                .ThenInclude(t => t.Projet)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (membre == null)
        {
            return NotFound();
        }
        
        return View(membre);
    }
    
    // GET: Membres/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Membres/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nom,Prenom,Email,Poste,PhotoUrl,DateEmbauche,EstActif")] MembreEquipe membre)
    {
        // Vérifier l'unicité de l'email
        if (await _context.Membres.AnyAsync(m => m.Email == membre.Email))
        {
            ModelState.AddModelError("Email", "Cet email est déjà utilisé");
        }
        
        if (ModelState.IsValid)
        {
            if (membre.DateEmbauche.HasValue)
                membre.DateEmbauche = DateTime.SpecifyKind(membre.DateEmbauche.Value, DateTimeKind.Utc);
            _context.Add(membre);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Membre ajouté avec succès!";
            return RedirectToAction(nameof(Index));
        }
        return View(membre);
    }
    
    // GET: Membres/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var membre = await _context.Membres.FindAsync(id);
        if (membre == null)
        {
            return NotFound();
        }
        return View(membre);
    }
    
    // POST: Membres/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Email,Poste,PhotoUrl,DateEmbauche,EstActif")] MembreEquipe membre)
    {
        if (id != membre.Id)
        {
            return NotFound();
        }
        
        // Vérifier l'unicité de l'email (sauf pour le membre actuel)
        if (await _context.Membres.AnyAsync(m => m.Email == membre.Email && m.Id != membre.Id))
        {
            ModelState.AddModelError("Email", "Cet email est déjà utilisé");
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                if (membre.DateEmbauche.HasValue)
                    membre.DateEmbauche = DateTime.SpecifyKind(membre.DateEmbauche.Value, DateTimeKind.Utc);
                _context.Update(membre);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membre modifié avec succès!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembreExists(membre.Id))
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
        return View(membre);
    }
    
    // GET: Membres/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var membre = await _context.Membres
            .Include(m => m.ProjetsLed)
            .Include(m => m.Taches)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (membre == null)
        {
            return NotFound();
        }
        
        return View(membre);
    }
    
    // POST: Membres/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var membre = await _context.Membres
            .Include(m => m.ProjetsLed)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (membre != null)
        {
            // Vérifier si le membre est chef de projet
            if (membre.ProjetsLed.Any())
            {
                TempData["ErrorMessage"] = "Impossible de supprimer ce membre car il est chef de projet. Veuillez d'abord réassigner ses projets.";
                return RedirectToAction(nameof(Details), new { id });
            }
            
            _context.Membres.Remove(membre);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Membre supprimé avec succès!";
        }
        
        return RedirectToAction(nameof(Index));
    }
    
    // POST: Membres/ToggleActif/5
    [HttpPost]
    public async Task<IActionResult> ToggleActif(int id)
    {
        var membre = await _context.Membres.FindAsync(id);
        if (membre == null)
        {
            return NotFound();
        }
        
        membre.EstActif = !membre.EstActif;
        await _context.SaveChangesAsync();
        
        TempData["SuccessMessage"] = membre.EstActif ? "Membre activé avec succès!" : "Membre désactivé avec succès!";
        return RedirectToAction(nameof(Details), new { id });
    }
    
    private bool MembreExists(int id)
    {
        return _context.Membres.Any(e => e.Id == id);
    }
}
