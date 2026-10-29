namespace GestionOrdinateurs.API;

using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

public class ReparationService
{
    private readonly IDbContextFactory<ParcInformatiqueContext> _contextFactory;
    private readonly JournalService _journal;

    public ReparationService(IDbContextFactory<ParcInformatiqueContext> contextFactory, JournalService journal)
    {
        _contextFactory = contextFactory;
        _journal = journal;
    }

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        // Un contexte par réparation car elles tournent en parallèle
        await using var context = await _contextFactory.CreateDbContextAsync();

        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Test",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Jean Dupont"
        };
        context.Reparations.Add(reparation);
        await context.SaveChangesAsync(); // création en base

        await Task.Delay(2000);

        reparation.Statut = "Terminée";
        await context.SaveChangesAsync();
        Console.WriteLine($"{ordinateur.Nom} réparé");
        _journal.Ecrire("Logs/Reparations/reparations.log", $"{ordinateur.Nom} réparé");
    }

    public async Task<List<Reparation>> ListerReparationsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Include : charge aussi l'ordinateur de chaque réparation
        return await context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();
    }

    // Ligne d'historique d'une réparation
    public record HistoriqueReparationRecord(
        int OrdinateurId,
        string NomOrdinateur,
        string Technicien,
        string TypeReparation,
        DateTime DateIntervention
    );

    public async Task<List<HistoriqueReparationRecord>> ObtenirHistoriqueAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Transforme chaque réparation en record (avec le nom de l'ordinateur)
        return await context.Reparations
            .Select(r => new HistoriqueReparationRecord(
                r.OrdinateurId,
                r.UnOrdinateur.Nom,
                r.Technicien,
                r.TypeReparation,
                r.DateDebut))
            .ToListAsync();
    }
}
