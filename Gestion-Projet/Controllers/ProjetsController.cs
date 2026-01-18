using Gestion_Projet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gestion_Projet.Controllers;

public class ProjetsController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public ProjetsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET: Projets
    public async Task<IActionResult> Index(string searchString, string statutFilter, string sortOrder)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["StatutFilter"] = statutFilter;
        ViewData["NomSortParm"] = string.IsNullOrEmpty(sortOrder) ? "nom_desc" : "";
        ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
        
        var projets = _context.Projets
            .Include(p => p.ChefProjet)
            .Include(p => p.Taches)
            .AsQueryable();
        
        // Recherche
        if (!string.IsNullOrEmpty(searchString))
        {
            projets = projets.Where(p => p.Nom.Contains(searchString));
        }
        
        // Filtre par statut
        if (!string.IsNullOrEmpty(statutFilter) && Enum.TryParse<StatutProjet>(statutFilter, out var statut))
        {
            projets = projets.Where(p => p.Statut == statut);
        }
        
        // Tri
        projets = sortOrder switch
        {
            "nom_desc" => projets.OrderByDescending(p => p.Nom),
            "Date" => projets.OrderBy(p => p.DateDebut),
            "date_desc" => projets.OrderByDescending(p => p.DateDebut),
            _ => projets.OrderBy(p => p.Nom)
        };
        
        return View(await projets.ToListAsync());
    }
    
    // GET: Projets/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var projet = await _context.Projets
            .Include(p => p.ChefProjet)
            .Include(p => p.Taches)
                .ThenInclude(t => t.Membre)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (projet == null)
        {
            return NotFound();
        }
        
        return View(projet);
    }
    
    // GET: Projets/Create
    public IActionResult Create()
    {
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet");
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutProjet)).Cast<StatutProjet>());
        return View();
    }
    
    // POST: Projets/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nom,Description,DateDebut,DateFinPrevue,Budget,ChefProjetId,Statut")] Projet projet)
    {
        // Validation personnalisée
        if (projet.DateFinPrevue.HasValue && projet.DateFinPrevue < projet.DateDebut)
        {
            ModelState.AddModelError("DateFinPrevue", "La date de fin prévue doit être postérieure à la date de début");
        }
        
        if (ModelState.IsValid)
        {
            projet.DateCreation = DateTime.Now;
            _context.Add(projet);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Projet créé avec succès!";
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet", projet.ChefProjetId);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutProjet)).Cast<StatutProjet>(), projet.Statut);
        return View(projet);
    }
    
    // GET: Projets/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var projet = await _context.Projets.FindAsync(id);
        if (projet == null)
        {
            return NotFound();
        }
        
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet", projet.ChefProjetId);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutProjet)).Cast<StatutProjet>(), projet.Statut);
        return View(projet);
    }
    
    // POST: Projets/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Description,DateDebut,DateFinPrevue,Budget,ChefProjetId,Statut,DateCreation")] Projet projet)
    {
        if (id != projet.Id)
        {
            return NotFound();
        }
        
        // Validation personnalisée
        if (projet.DateFinPrevue.HasValue && projet.DateFinPrevue < projet.DateDebut)
        {
            ModelState.AddModelError("DateFinPrevue", "La date de fin prévue doit être postérieure à la date de début");
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(projet);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Projet modifié avec succès!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjetExists(projet.Id))
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
        
        ViewBag.Membres = new SelectList(_context.Membres.Where(m => m.EstActif), "Id", "NomComplet", projet.ChefProjetId);
        ViewBag.Statuts = new SelectList(Enum.GetValues(typeof(StatutProjet)).Cast<StatutProjet>(), projet.Statut);
        return View(projet);
    }
    
    // GET: Projets/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var projet = await _context.Projets
            .Include(p => p.ChefProjet)
            .Include(p => p.Taches)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (projet == null)
        {
            return NotFound();
        }
        
        return View(projet);
    }
    
    // POST: Projets/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var projet = await _context.Projets.FindAsync(id);
        if (projet != null)
        {
            _context.Projets.Remove(projet);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Projet supprimé avec succès!";
        }
        
        return RedirectToAction(nameof(Index));
    }
    
    private bool ProjetExists(int id)
    {
        return _context.Projets.Any(e => e.Id == id);
    }
}
