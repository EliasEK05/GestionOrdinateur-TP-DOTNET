using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Prend le login et renvoie un token JWT
    [HttpPost]
    public async Task<IActionResult> Login(string login)
    {
        // Appel de l'API "utilisateur par login" via HttpClientFactory
        var client = _httpClientFactory.CreateClient("UsersApi");
        var response = await client.GetAsync($"api/users/{login}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound("L'utilisateur n'existe pas"); // 404
        }

        var user = await response.Content.ReadFromJsonAsync<User>();

        // Informations mises dans le token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user!.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // Signature avec la clé du appsettings.json
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) }); // 200 OK
    }
}
