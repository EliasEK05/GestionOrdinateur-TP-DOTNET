namespace GestionOrdinateurs.API;

using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

public class OrdinateurService : IOrdinateurService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journal;

    public OrdinateurService(ParcInformatiqueContext context, JournalService journal)
    {
        _context = context;
        _journal = journal;

        // Abonnements
        OrdinateurAjoute += AfficherMessageAjout;
        OrdinateurSupprime += AfficherMessageSuppression;
        OrdinateurMisAJour += AfficherMessageMiseAJour;
    }



    public async Task<List<Ordinateur>> ListerOrdinateursAsync()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    public async Task<Ordinateur?> ObtenirParIdAsync(int id)
    {
        return await _context.Ordinateurs.FindAsync(id);
    }

    public async Task AjouterOrdinateurAsync(Ordinateur ordinateur)
    {
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        _journal.Ecrire("Logs/Ordinateurs/ordinateurs.log", $"{ordinateur.Nom} ajouté");
    }

    public async Task<bool> SupprimerOrdinateurAsync(int id)
    {
        var ordinateur = await ObtenirParIdAsync(id);

        if (ordinateur != null)
        {
            _context.Ordinateurs.Remove(ordinateur);
            await _context.SaveChangesAsync();
            OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));
            _journal.Ecrire("Logs/Ordinateurs/ordinateurs.log", $"{ordinateur.Nom} supprimé");
            return true;
        }

        return false; // Non trouvé
    }

    public async Task<bool> MettreAJourOrdinateurAsync(Ordinateur ordinateur)
    {
        var existant = await ObtenirParIdAsync(ordinateur.Id);
        if (existant == null)
        {
            return false;
        }

        //on recopie les nouvelles valeurs sur l'ordi existant (sauf l'id)
        existant.Nom = ordinateur.Nom;
        existant.Etat = ordinateur.Etat;
        existant.Stock = ordinateur.Stock;
        existant.Prix = ordinateur.Prix;
        existant.Vendu = ordinateur.Vendu;
        existant.Caracteristiques = ordinateur.Caracteristiques;


        await _context.SaveChangesAsync();
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(existant));
        _journal.Ecrire("Logs/Ordinateurs/ordinateurs.log", $"{existant.Nom} modifié");
        return true;
    }

    public async Task<List<Ordinateur>> RechercheParMarqueAsync(string marque)
    {
        //liste ordis trouvés de la marque
        return await _context.Ordinateurs
            .Where(o => o.Caracteristiques.Marque.ToLower() == marque.ToLower())
            .ToListAsync();
    }

    // Définition des événements
    public event EventHandler<OrdinateurAjouteEventArgs> OrdinateurAjoute;
    public event EventHandler<OrdinateurMisAJourEventArgs> OrdinateurMisAJour;
    public event EventHandler<OrdinateurSupprimeEventArgs> OrdinateurSupprime;


    // Méthodes pour afficher les messages dans la console
    private void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine($"Ordinateur ajouté : {e.Ordinateur.Nom}");
    }

    private void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine($"Ordinateur supprimé : {e.Ordinateur.Nom}");
    }

    private void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine($"Ordinateur mis à jour : {e.Ordinateur.Nom}");
    }

    public async Task<List<Ordinateur>> ListerDisponiblesAsync()
    {
        //ordis avec du stock
        return await _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    public async Task<List<Ordinateur>> ListerParPrixDecroissantAsync()
    {
        //du plus cher au moins cher (tri en mémoire : SQLite ne trie pas les decimal)
        var ordinateurs = await _context.Ordinateurs.ToListAsync();
        return ordinateurs.OrderByDescending(o => o.Prix).ToList();

    }

    // Pattern matching : action à faire selon l'état et les caractéristiques
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService }
                => "Remplacement",

            { Etat: EtatOrdinateur.Lent, Caracteristiques: { RamGo: < 8 } }
                => "Ajouter RAM",

            _
                => "Aucune action"
        };
    }

    // Tuple nommé avec les statistiques du parc
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques()
    {
        var ordinateurs = _context.Ordinateurs.ToList();

        return (
            ordinateurs.Count,
            ordinateurs.Count(o => o.Stock > 0),
            ordinateurs.Sum(o => o.Prix * o.Stock), // calcul en mémoire : SQLite ne fait pas de Sum sur les decimal
            _context.Reparations.Count()
        );
    }
}
