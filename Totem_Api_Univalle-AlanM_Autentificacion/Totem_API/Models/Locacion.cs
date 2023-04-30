using System;
using System.Collections.Generic;

namespace Totem_API.Models;

public partial class Locacion
{
    public int IdLocacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Keywords { get; set; } = null!;

    public string UrlCarruselImagenes { get; set; } = null!;

    public string UrlMapa { get; set; } = null!;

    public int IdTotem { get; set; }

    public virtual Totem? IdTotemNavigation { get; set; }
}
