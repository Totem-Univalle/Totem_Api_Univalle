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
    public class LocacionesController : ControllerBase
    {
        private readonly TotemContext _context;

        public LocacionesController(TotemContext context)
        {
            _context = context;
        }

        // GET: api/Locaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locacion>>> GetLocacions()
        {
          if (_context.Locacions == null)
          {
              return NotFound();
          }
            return await _context.Locacions.ToListAsync();
        }

        // GET: api/Locaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Locacion>> GetLocacion(int id)
        {
          if (_context.Locacions == null)
          {
              return NotFound();
          }
            var locacion = await _context.Locacions.FindAsync(id);

            if (locacion == null)
            {
                return NotFound();
            }

            return locacion;
        }

        // PUT: api/Locaciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocacion(int id, Locacion locacion)
        {
            if (id != locacion.IdLocacion)
            {
                return BadRequest();
            }

            _context.Entry(locacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocacionExists(id))
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

        // POST: api/Locaciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Locacion>> PostLocacion(Locacion locacion)
        {
          if (_context.Locacions == null)
          {
              return Problem("Entity set 'TotemContext.Locacions'  is null.");
          }
            _context.Locacions.Add(locacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocacion", new { id = locacion.IdLocacion }, locacion);
        }

        // DELETE: api/Locaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocacion(int id)
        {
            if (_context.Locacions == null)
            {
                return NotFound();
            }
            var locacion = await _context.Locacions.FindAsync(id);
            if (locacion == null)
            {
                return NotFound();
            }

            _context.Locacions.Remove(locacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocacionExists(int id)
        {
            return (_context.Locacions?.Any(e => e.IdLocacion == id)).GetValueOrDefault();
        }
    }
}
