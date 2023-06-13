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
    public class TotemsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TotemContext _context;

        public TotemsController(IConfiguration configuration, TotemContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/Totems
        //[Authorize]
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
        [HttpGet("{id}")]
        public async Task<ActionResult<Totem>> GetTotem(int id)
        {
            if (_context.Totems == null)
            {
                return NotFound();
            }
            var totems = await _context.Totems.FindAsync(id);

            if (totems == null)
            {
                return NotFound();
            }

            return totems;
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> PutTotem(int id, [FromForm] TotemUploadModel totemInput)
        {
            var totems = await _context.Totems.FindAsync(id);

            if (totems == null)
            {
                return NotFound();
            }

            // Actualizar los campos del totems con los datos del modelo
            totems.Nombre = totemInput.Nombre;
            totems.NumeroPlantilla = totemInput.NumeroPlantilla;
            totems.IdUsuario = totemInput.IdUsuario;

            // Handle the image upload
            var formFile = totemInput.Imagen;

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
                    totems.UrlLogo = blobClient.Uri.ToString();
                }
            }

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
        //[Authorize]
        [HttpPut("{id}/numero-plantilla")]
        public async Task<IActionResult> PutTotemNumeroPlantilla(int id, [FromBody] int nuevoNumeroPlantilla)
        {
            var totem = await _context.Totems.FindAsync(id);

            if (totem == null)
            {
                return NotFound();
            }

            totem.NumeroPlantilla = (short)nuevoNumeroPlantilla;

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
        //[Authorize]
        [HttpPost]

        public async Task<ActionResult<Totem>> PostTotem([FromForm] TotemUploadModel inputModel)
        {
            if (inputModel == null || inputModel.Imagen == null || inputModel.Nombre == null || inputModel.NumeroPlantilla == null || inputModel.IdUsuario == null)
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

            var usuario = await _context.Usuarios.FindAsync(inputModel.IdUsuario);
            if (usuario == null)
            {
                return NotFound($"El usuario con Id {inputModel.IdUsuario} no existe");
            }

            // Save data to database
            var totems = new Totem
            {

                Nombre = inputModel.Nombre,
                UrlLogo = blob.Uri.ToString(),
                NumeroPlantilla = inputModel.NumeroPlantilla,
                IdUsuario = inputModel.IdUsuario,
            };

            if (_context.Totems == null)
            {
                return Problem("Entity set 'TotemContext.Totems' is null.");
            }

            _context.Totems.Add(totems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTotem", new { id = totems.IdTotem }, totems);
        }



        // DELETE: api/Totems/5
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTotem(int id)
        {
            var totems = await _context.Totems.FindAsync(id);
            if (totems == null)
            {
                return NotFound();
            }

            // Eliminar las imágenes del carrusel de Azure Blob Storage
            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            var containerName = "contenedortotem";
            var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            var containerClient = blobServiceClient.GetContainerReference(containerName);

            if (!string.IsNullOrEmpty(totems.UrlLogo))
            {
                var blobName = Path.GetFileName(totems.UrlLogo);
                var blobClient = containerClient.GetBlockBlobReference(blobName);
                await blobClient.DeleteIfExistsAsync();
            }


            _context.Totems.Remove(totems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TotemExists(int id)
        {
            return (_context.Totems?.Any(e => e.IdTotem == id)).GetValueOrDefault();
        }
    }
}
