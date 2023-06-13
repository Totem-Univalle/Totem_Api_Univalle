namespace Totem_API.Models
{
    public class PublicidadUploadModel
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public IFormFile Imagen { get; set; }

        public int IdTotem { get; set; }
    }
}
