namespace GestionOrdinateurs.Shared;

// Collection spécialisée d'ordinateurs avec indexeurs
public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    // Recherche par id : inventaire[ordinateurId]
    public Ordinateur? this[int index]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Id == index); }
    }

    // Recherche par nom : inventaire[ordinateurNom]
    public Ordinateur? this[string reference]
    {
        get { return Ordinateurs.FirstOrDefault(o => o.Nom == reference); }
    }
}
