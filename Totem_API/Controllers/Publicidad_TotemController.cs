using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Totem_API.Data;
using Totem_API.Models;

namespace Totem_API.Controllers
{
    public class Publicidad_TotemController : Controller
    {
        private readonly TotemContext _context;



        public Publicidad_TotemController(TotemContext context)
        {
            _context = context;
        }



        [Route("api/PublicidadT/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Publicidad>>> GetPublicidadId(int id)
        {

            if (_context.Publicidads == null)
            {
                return NotFound();
            }

            var publicidad = await _context.Publicidads.Where(t => t.IdTotem == id).ToListAsync();

            if (publicidad == null)
            {
                return NotFound();
            }
            return publicidad;
        }
    }
}
