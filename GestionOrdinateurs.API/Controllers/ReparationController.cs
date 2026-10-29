using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API;

[ApiController]
[Authorize] // token obligatoire pour tous les endpoints
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    private readonly ReparationService _reparService;
    private readonly IOrdinateurService _ordinateurService;
    private readonly StockService _stockService;
    private readonly ILogger<ReparationController> _logger;

    public ReparationController(ReparationService reparService, IOrdinateurService ordinateurService,
        StockService stockService, ILogger<ReparationController> logger)
    {
        _reparService = reparService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
        _logger = logger;
    }

    [HttpPost("simuler")]
    public async Task<IActionResult> SimulerReparation()
    {
        //Récupération de la liste d'ordinateurs.
        var listOrdinateurs = await _ordinateurService.ListerOrdinateursAsync();

        // Les 3 réparations en parallèle
        await Task.WhenAll(
            _reparService.SimulerReparationAsync(listOrdinateurs[0]),
            _reparService.SimulerReparationAsync(listOrdinateurs[1]),
            _reparService.SimulerReparationAsync(listOrdinateurs[2])
        );

        return Ok("Réparations terminées"); // 200 OK
    }

    [HttpGet]
    public async Task<IActionResult> GetReparations()
    {
        return Ok(await _reparService.ListerReparationsAsync()); // 200 OK, avec l'ordinateur de chaque réparation
    }

    [HttpGet("/api/reparations/historique")]
    public async Task<IActionResult> GetHistorique()
    {
        return Ok(await _reparService.ObtenirHistoriqueAsync()); // 200 OK
    }

    [HttpGet("stock-ssd")]
    public IActionResult GetStockSSD()
    {
        // 10 consommations en parallèle
        Parallel.For(0, 10, i => _stockService.ConsommerSSD());

        _logger.LogInformation("Stock SSD : {Stock}", _stockService.StockSSD);
        return Ok(_stockService.StockSSD); // 200 OK
    }
}
