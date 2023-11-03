using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Totem_API.Data;
using Totem_API.Models;

namespace Totem_API.Controllers
{
    public class TotemLocacionController : Controller
    {
        private readonly TotemContext _context;
        public TotemLocacionController(TotemContext context)
        {
            _context = context;
        }

        [Route("api/[controller]")]
        [HttpGet("{id,keys}")]
        public async Task<ActionResult<Locacion>> GetTotem(int id, string keys)
        {

            if (_context.Locacions == null)
            {
                return NotFound();
            }

            var locaciones = await _context.Locacions.Where(u => u.IdTotem == id).ToListAsync();
            string[] ArrayCheck = keys.Split(',');
            int[] coincidence = new int[locaciones.Count];
            int index = 0;
            foreach (var item in locaciones)
            {
                string[] arrayKeysL = item.Keywords.Split(',');

                for (int i = 0; i < arrayKeysL.Length; i++)
                {
                    foreach (var keyWord in ArrayCheck)
                    {
                        if (arrayKeysL[i] == keyWord)
                        {
                            coincidence[index] += 1;
                        }
                    }
                }
                index++;
            }
            if (locaciones == null)
            {
                return NotFound();
            }
            int indexResult = Array.IndexOf(coincidence, coincidence.Max());
            return locaciones[indexResult];
        }


        [Route("api/TotemU/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Totem>>> GetTotem(int id)
        {
            if (_context.Totems == null)
            {
                return NotFound();
            }

            var totem = await _context.Totems.Where(u => u.IdUsuario == id).ToListAsync();

            if (totem == null)
            {
                return NotFound();
            }
            return totem;

        }


    }
}