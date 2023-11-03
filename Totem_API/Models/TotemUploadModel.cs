namespace Totem_API.Models
{
    public class TotemUploadModel
    {
        public string Nombre { get; set; } = null;

        public IFormFile? Imagen { get; set; }

        public short NumeroPlantilla { get; set; }

        public int? IdUsuario { get; set; }
    }
}
