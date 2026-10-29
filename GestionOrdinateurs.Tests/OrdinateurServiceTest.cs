namespace GestionOrdinateurs.Tests;

using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

public class OrdinateurServiceTest
{
    [Fact]
    public async Task AjouterOrdinateur()
    {
        // Arrange
        var service = new OrdinateurService(CreerContexte(), new JournalService());
        var ordinateur = new Ordinateur
        {
            Id = 6,
            Nom = "Portable 4",
            Etat = EtatOrdinateur.EnService,
            Caracteristiques = new Caracteristiques(16, TypeOrdinateur.Portable, 512, "Asus", "Intel")
        };

        // Act
        await service.AjouterOrdinateurAsync(ordinateur);

        // Assert
        var result = await service.ObtenirParIdAsync(6);
        result.Should().NotBeNull(); //vérifie si l'ordinateur a été ajouté
        result!.Nom.Should().Be("Portable 4"); //vérifie si le nom OK
    }

    [Fact]
    public async Task SupprimerOrdinateur()
    {
        // Arrange
        var service = new OrdinateurService(CreerContexte(), new JournalService());
        var ordinateur = new Ordinateur
        {
            Id = 7,
            Nom = "Portable 5",
            Etat = EtatOrdinateur.EnService,
            Caracteristiques = new Caracteristiques(8, TypeOrdinateur.Portable, 256, "Dell", "AMD")
        };
        await service.AjouterOrdinateurAsync(ordinateur);

        // Act
        var result = await service.SupprimerOrdinateurAsync(7);

        // Assert
        result.Should().BeTrue(); //vérifie si bien supprimé
        var deletedOrdi = await service.ObtenirParIdAsync(7);
        deletedOrdi.Should().BeNull(); //vérifie si l'ordinateur a été supprimé
    }

    [Fact]
    public async Task SupprimerOrdinateur_Inexistant()
    {
        // Arrange
        var service = new OrdinateurService(CreerContexte(), new JournalService());

        // Act
        var result = await service.SupprimerOrdinateurAsync(999); // ID inexistant

        // Assert
        result.Should().BeFalse(); //vérifie que ca échoue
    }

    [Fact]
    public async Task CreerReparation()
    {
        // Arrange
        var context = CreerContexte();
        var ordinateur = new Ordinateur
        {
            Id = 8,
            Nom = "Fixe 3",
            Etat = EtatOrdinateur.EnPanne,
            Caracteristiques = new Caracteristiques(16, TypeOrdinateur.Fixe, 512, "HP", "Intel")
        };
        context.Ordinateurs.Add(ordinateur);
        var reparation = new Reparation
        {
            OrdinateurId = 8,
            TypeReparation = "Ecran",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Jean Dupont"
        };

        // Act
        context.Reparations.Add(reparation);
        await context.SaveChangesAsync();

        // Assert
        var result = await context.Reparations.FindAsync(reparation.Id);
        result.Should().NotBeNull(); //vérifie si la réparation a été créée
        result!.OrdinateurId.Should().Be(8); //vérifie le lien avec l'ordinateur
    }

    private ParcInformatiqueContext CreerContexte()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

}