using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Totem_API.Data;
using Totem_API.Models;

namespace Totem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly TotemContext _context;
        private readonly string key;

        public UsuariosController(TotemContext context, IConfiguration config)
        {
            _context = context;
            key = config.GetSection("JWTsetting").GetSection("securitykey").ToString();
        }

        //POST: api/Autentificacion
        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Autentificacion([FromBody] Auth user)
        {
            var userLogin = _context.Usuarios.FirstOrDefault(usr => usr.Email == user.Email && usr.Password == user.Password);
            if (userLogin == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var keyT = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(keyT, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return StatusCode(StatusCodes.Status200OK, new { token, user = userLogin });

        }


        //POST: Autenticacion sin token para totems
        [HttpPost]
        [Route("LoginTotem")]
        public IActionResult Login([FromBody] Auth user)
        {
            var userLogin = _context.Usuarios.FirstOrDefault(usr => usr.Email == user.Email && usr.Password == user.Password);
            if (userLogin == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new { user = userLogin });
            }
        }

        // GET: api/Usuarios

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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



        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'TotemContext.Usuarios'  is null.");
            }
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.IdUsuario }, usuario);
        }

        // DELETE: api/Usuarios/5
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        //[Authorize]
        [HttpPut("{id}/update-user")]

        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUplod usuarioUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userToUpdate = await _context.Usuarios.FindAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }
            // Verifica si existe otro usuario con el mismo correo electrónico
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarioUpdateDTO.Email && u.IdUsuario != id);
            if (existingUser != null)
            {
                return Conflict("Ya existe otro usuario con el mismo correo electrónico.");
            }
            userToUpdate.Nombre = usuarioUpdateDTO.Nombre;
            userToUpdate.Apellido = usuarioUpdateDTO.Apellido;
            userToUpdate.Email = usuarioUpdateDTO.Email;

            // Restablecer contraseña solo si se proporciona un valor nuevo
            if (!string.IsNullOrEmpty(usuarioUpdateDTO.Password))
            {
                userToUpdate.Password = usuarioUpdateDTO.Password;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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
        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
