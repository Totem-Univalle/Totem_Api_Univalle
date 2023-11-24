namespace Totem_API.Models
{
    public class LocacionUploadModel
    {
        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public List<string> Keywords { get; set; } = new List<string>();

        public List<IFormFile>? ImagenesCarrucel { get; set; } = null!;

        public IFormFile? ImagenMapa { get; set; } = null!;

        public int IdTotem { get; set; }
    }
}
