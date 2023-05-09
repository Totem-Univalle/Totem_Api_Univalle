using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Totem_API.Data;
using Totem_API.Models;

namespace Totem_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PublicidadController : ControllerBase
    {
        private readonly TotemContext _context;

        public PublicidadController(TotemContext context)
        {
            _context = context;
        }

        // GET: api/Publicidad
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publicidad>>> GetPublicidads()
        {
          if (_context.Publicidads == null)
          {
              return NotFound();
          }
            return await _context.Publicidads.ToListAsync();
        }

        // GET: api/Publicidad/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Publicidad>> GetPublicidad(int id)
        {
          if (_context.Publicidads == null)
          {
              return NotFound();
          }
            var publicidad = await _context.Publicidads.FindAsync(id);

            if (publicidad == null)
            {
                return NotFound();
            }

            return publicidad;
        }

        // PUT: api/Publicidad/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublicidad(int id, Publicidad publicidad)
        {
            if (id != publicidad.IdPublicidad)
            {
                return BadRequest();
            }

            _context.Entry(publicidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicidadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Publicidad
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Publicidad>> PostPublicidad(Publicidad publicidad)
        {
          if (_context.Publicidads == null)
          {
              return Problem("Entity set 'TotemContext.Publicidads'  is null.");
          }
            _context.Publicidads.Add(publicidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublicidad", new { id = publicidad.IdPublicidad }, publicidad);
        }

        // DELETE: api/Publicidad/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicidad(int id)
        {
            if (_context.Publicidads == null)
            {
                return NotFound();
            }
            var publicidad = await _context.Publicidads.FindAsync(id);
            if (publicidad == null)
            {
                return NotFound();
            }

            _context.Publicidads.Remove(publicidad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublicidadExists(int id)
        {
            return (_context.Publicidads?.Any(e => e.IdPublicidad == id)).GetValueOrDefault();
        }
    }
}
