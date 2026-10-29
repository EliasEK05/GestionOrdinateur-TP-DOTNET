using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _context;

    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // Récupérer tous les utilisateurs
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _context.Users.ToListAsync()); // 200 OK
    }

    // Chercher un utilisateur par login
    [HttpGet("{login}")]
    public async Task<IActionResult> GetUserParLogin(string login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

        if (user == null)
        {
            return NotFound(); // 404 si pas trouvé
        }

        return Ok(user); // 200 OK
    }
}
