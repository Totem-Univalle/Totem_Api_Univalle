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
    public class TotemsController : ControllerBase
    {
        private readonly TotemContext _context;

        public TotemsController(TotemContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Totem>>> GetTotems()
        {
          if (_context.Totems == null)
          {
              return NotFound();
          }
            return await _context.Totems.ToListAsync();
        }

        // GET: api/Totems/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Totem>> GetTotem(int id)
        {
          if (_context.Totems == null)
          {
              return NotFound();
          }
            var totem = await _context.Totems.FindAsync(id);

            if (totem == null)
            {
                return NotFound();
            }

            return totem;
        }

        // PUT: api/Totems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTotem(int id, Totem totem)
        {
            if (id != totem.IdTotem)
            {
                return BadRequest();
            }

            _context.Entry(totem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TotemExists(id))
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

        // POST: api/Totems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Totem>> PostTotem(Totem totem)
        {
          if (_context.Totems == null)
          {
              return Problem("Entity set 'TotemContext.Totems'  is null.");
          }
            _context.Totems.Add(totem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTotem", new { id = totem.IdTotem }, totem);
        }

        // DELETE: api/Totems/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTotem(int id)
        {
            if (_context.Totems == null)
            {
                return NotFound();
            }
            var totem = await _context.Totems.FindAsync(id);
            if (totem == null)
            {
                return NotFound();
            }

            _context.Totems.Remove(totem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TotemExists(int id)
        {
            return (_context.Totems?.Any(e => e.IdTotem == id)).GetValueOrDefault();
        }
    }
}
