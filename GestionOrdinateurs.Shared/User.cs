using System;
using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;

public class User
{
    public int Id { get; set; } // clé primaire

    [Required]
    public string Login { get; set; } = string.Empty;

    public RoleEnum Role { get; set; }
}
