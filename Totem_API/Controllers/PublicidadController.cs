using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Totem_API.Data;
using Totem_API.Models;

namespace Totem_API.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class PublicidadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TotemContext _context;

        public PublicidadController(IConfiguration configuration, TotemContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/Publicidad
        //[Authorize]
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
        //[Authorize]
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

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublicidad(int id, [FromForm] PublicidadUploadModel publicidadInput)
        {
            var publicidad = await _context.Publicidads.FindAsync(id);

            if (publicidad == null)
            {
                return NotFound();
            }

            // Actualizar los campos del publicidad con los datos del modelo
            publicidad.FechaInicio = publicidadInput.FechaInicio;
            publicidad.FechaFin = publicidadInput.FechaFin;
            publicidad.IdTotem = publicidadInput.IdTotem;

            // Handle the image upload
            var formFile = publicidadInput.Imagen;

            if (formFile != null && formFile.Length > 0)
            {
                // Convierte la imagen a un array de bytes
                using (var ms = new MemoryStream())
                {
                    await formFile.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();

                    // Sube la imagen al Blob Storage
                    var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                    var containerName = "contenedortotem";
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);

                    var blobClient = new BlobClient(connectionString, containerName, fileName);
                    await blobClient.UploadAsync(new MemoryStream(fileBytes), true);

                    // Actualiza la URL de la imagen en la base de datos
                    publicidad.UrlPublicidad = blobClient.Uri.ToString();
                }
            }

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
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<Publicidad>> PostPublicidad([FromForm] PublicidadUploadModel inputModel)
        {
            if (inputModel == null || inputModel.Imagen == null || inputModel.FechaInicio == null || inputModel.FechaFin == null || inputModel.IdTotem == null)
            {
                return BadRequest("Invalid request");
            }

            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");

            if (connectionString == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "AzureBlobStorage connection string is missing");
            }

            // Upload image to Blob Storage
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(inputModel.Imagen.FileName);
            var blobContainerName = "contenedortotem";
            var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            var container = blobServiceClient.GetContainerReference(blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(inputModel.Imagen.OpenReadStream());

            //var usuario = await _context.Usuarios.FindAsync(inputModel.IdUsuario);
            //if (usuario == null)
            //{
            //    return NotFound($"El usuario con Id {inputModel.IdUsuario} no existe");
            //}

            // Save data to database
            var publicidad = new Publicidad
            {
                FechaInicio = inputModel.FechaInicio,
                FechaFin = inputModel.FechaFin,
                UrlPublicidad = blob.Uri.ToString(),
                IdTotem = inputModel.IdTotem
            };

            if (_context.Totems == null)
            {
                return Problem("Entity set 'TotemContext.Totems' is null.");
            }

            _context.Publicidads.Add(publicidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublicidad", new { id = publicidad.IdPublicidad }, publicidad);
        }

        // DELETE: api/Publicidad/5
        // [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublicidad(int id)
        {

            var publicidad = await _context.Publicidads.FindAsync(id);
            if (publicidad == null)
            {
                return NotFound();
            }


            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            var containerName = "contenedortotem";
            var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            var containerClient = blobServiceClient.GetContainerReference(containerName);

            if (!string.IsNullOrEmpty(publicidad.UrlPublicidad))
            {
                var blobName = Path.GetFileName(publicidad.UrlPublicidad);
                var blobClient = containerClient.GetBlockBlobReference(blobName);
                await blobClient.DeleteIfExistsAsync();
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
