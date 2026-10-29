using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;

public enum EtatOrdinateur
{
    EnService,
    EnPanne,
    EnMaintenance,
    HorsService,
    Lent,
    Reparable
}

[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public EtatOrdinateur Etat { get; set; }

    [DefaultValue(0)]
    public int Stock { get; set; }

    [DefaultValue(0)]
    public decimal Prix { get; set; }

    // Ordinateur vendu ou non (false par défaut)
    [DefaultValue(false)]
    public bool Vendu { get; set; } = false;

    // Ram, type, stockage, marque, processeur
    public Caracteristiques Caracteristiques { get; set; }

    // Réparations de cet ordinateur
    public List<Reparation> Reparations { get; set; } = new();
}