using System;
using System.Collections.Generic;

namespace Totem_API.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte Rol { get; set; }

    public string Nombre { get; set; } = null!;
    public string Institucion { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public virtual Totem? Totem { get; set; }
}
