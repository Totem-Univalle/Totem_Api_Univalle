using System;
using System.Collections.Generic;

namespace Totem_API.Models;

public partial class Publicidad
{
    public int IdPublicidad { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string UrlPublicidad { get; set; } = null!;

    public int IdTotem { get; set; }

    public virtual Totem? IdTotemNavigation { get; set; }
}
