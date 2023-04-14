using System;
using System.Collections.Generic;

namespace Totem_API.Models;

public partial class Totem
{
    public int IdTotem { get; set; }

    public string Nombre { get; set; } = null!;

    public string UrlLogo { get; set; } = null!;

    public short NumeroPlantilla { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario? IdTotemNavigation { get; set; }

    public virtual ICollection<Locacion> Locacions { get; } = new List<Locacion>();

    public virtual ICollection<Publicidad> Publicidads { get; } = new List<Publicidad>();
}
