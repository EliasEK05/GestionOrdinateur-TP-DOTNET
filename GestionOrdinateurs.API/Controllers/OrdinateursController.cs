using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API;

[ApiController]
[Authorize] // token obligatoire pour tous les endpoints
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly ILogger<OrdinateursController> _logger;
    private readonly IConfiguration _configuration;

    public OrdinateursController(IOrdinateurService ordinateurService, ILogger<OrdinateursController> logger, IConfiguration configuration)
    {
        _ordinateurService = ordinateurService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        _logger.LogInformation("info appli");

        var info = new
        {
            Nom = _configuration["Application:Name"],
            Version = _configuration["Application:Version"]
        };

        return Ok(info); // 200 OK
    }

    [HttpGet]
    public async Task<IActionResult> GetListeOrdinateurs()
    {
        _logger.LogInformation("Récupération liste des ordi");
        var ordinateurs = await _ordinateurService.ListerOrdinateursAsync();
        return Ok(ordinateurs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrdinateurParID(int id)
    {
        _logger.LogInformation("Chercher ordinateur");
        var ordinateur = await _ordinateurService.ObtenirParIdAsync(id);

        if (ordinateur == null)
        {
            return NotFound(); // Retourne code 404 si pas trouvé
        }

        return Ok(ordinateur); // Retourne l'ordinateur avec  code 200
    }

    [HttpPost]
    public async Task<IActionResult> AjouterOrdinateur(Ordinateur ordinateur)
    {
        _logger.LogInformation("Ajout ordinateur");
        await _ordinateurService.AjouterOrdinateurAsync(ordinateur);
        return CreatedAtAction(nameof(GetOrdinateurParID), new { id = ordinateur.Id }, ordinateur); // Retourne code 201 Created
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrdinateur(Ordinateur ordinateur)
    {
        _logger.LogInformation("Mise a jour ordinateur");
        bool misAJour = await _ordinateurService.MettreAJourOrdinateurAsync(ordinateur);

        if (!misAJour)
        {
            return NotFound("Impossible de mettre à jour "); // 404 Not Found
        }

        return Ok(ordinateur); // 200 OK

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // suppression réservée aux administrateurs
    public async Task<IActionResult> Supprimer(int id)
    {
        _logger.LogInformation("Suppression ordinateur");
        bool supprime = await _ordinateurService.SupprimerOrdinateurAsync(id);

        if (!supprime)
        {
            return NotFound("Impossible de supprimer"); // 404 Not Found
        }

        return NoContent(); // 204 No Content
    }



    [HttpGet("marque/{marque}")]
    public async Task<IActionResult> GetOrdinateurParMarque(string marque)
    {
        _logger.LogInformation("Recherche par marque");
        var result = await _ordinateurService.RechercheParMarqueAsync(marque);

        if (result.Count == 0)
        {
            return NotFound("Aucun ordinateur trouvé pour cette marque"); // 404 Not Found
        }
        return Ok(result); // 200 OK
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> GetDisponibles()
    {
        _logger.LogInformation("Ordinateurs disponibles");
        return Ok(await _ordinateurService.ListerDisponiblesAsync()); // 200 OK
    }

    [HttpGet("par-prix")]
    public async Task<IActionResult> GetParPrixDecroissant()
    {
        _logger.LogInformation("Ordinateurs du plus cher au moins cher");
        return Ok(await _ordinateurService.ListerParPrixDecroissantAsync()); // 200 OK
    }

    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> GetDiagnostic(int id)
    {
        _logger.LogInformation("Diagnostic ordinateur");
        var ordinateur = await _ordinateurService.ObtenirParIdAsync(id);

        if (ordinateur == null)
        {
            return NotFound(); // 404 si pas trouvé
        }

        return Ok(_ordinateurService.Diagnostiquer(ordinateur)); // 200 OK
    }

    [HttpGet("statistiques")]
    public IActionResult GetStatistiques()
    {
        _logger.LogInformation("Statistiques du parc");

        // Déconstruction du tuple nommé
        var (total, enStock, valeurStock, nbReparations)
            = _ordinateurService.CalculerStatistiques();

        return Ok(new { total, enStock, valeurStock, nbReparations }); // 200 OK
    }

    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> Chercher(string nom)
    {
        _logger.LogInformation("Recherche dans l'inventaire");
        var inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = await _ordinateurService.ListerOrdinateursAsync();

        var ordinateur = inventaire[nom]; // recherche par nom avec l'indexeur

        return Ok(ordinateur);
    }


}