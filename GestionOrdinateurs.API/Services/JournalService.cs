namespace GestionOrdinateurs.API;

// Écrit les messages dans des fichiers de log
public class JournalService
{
    // Verrou : plusieurs réparations peuvent écrire en même temps
    private readonly object _verrou = new object();

    public void Ecrire(string fichier, string message)
    {
        lock (_verrou)
        {
            // Crée le dossier s'il n'existe pas (ex : lors des tests unitaires)
            Directory.CreateDirectory(Path.GetDirectoryName(fichier)!);

            // using : le fichier est fermé automatiquement
            using var writer =
                new StreamWriter(
                    fichier,
                    append: true);

            writer.WriteLine($"{DateTime.Now:dd/MM/yyyy} - {message}");
        }
    }
}
