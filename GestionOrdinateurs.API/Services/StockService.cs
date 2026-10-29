namespace GestionOrdinateurs.API;

public class StockService
{
    public int StockSSD { get; set; } = 10;

    // Objet servant de verrou
    private readonly object _verrou = new object();

    public void ConsommerSSD()
    {
        // Un seul thread à la fois peut modifier le stock
        lock (_verrou)
        {
            var stock = StockSSD;   // lecture du stock
            Task.Delay(10).Wait();  // délai avant de diminuer
            StockSSD = stock - 1;   // écriture du nouveau stock
        }
    }
}