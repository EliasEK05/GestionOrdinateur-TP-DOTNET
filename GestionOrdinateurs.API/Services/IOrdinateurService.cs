namespace GestionOrdinateurs.API;

using GestionOrdinateurs.Shared;


public interface IOrdinateurService
{
    Task<List<Ordinateur>> ListerOrdinateursAsync();
    Task<Ordinateur?> ObtenirParIdAsync(int id);
    Task AjouterOrdinateurAsync(Ordinateur ordinateur);
    Task<bool> SupprimerOrdinateurAsync(int id);
    Task<bool> MettreAJourOrdinateurAsync(Ordinateur ordinateur);
    Task<List<Ordinateur>> RechercheParMarqueAsync(string marque);
    Task<List<Ordinateur>> ListerDisponiblesAsync();
    Task<List<Ordinateur>> ListerParPrixDecroissantAsync();
    string Diagnostiquer(Ordinateur ordinateur);
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
}

