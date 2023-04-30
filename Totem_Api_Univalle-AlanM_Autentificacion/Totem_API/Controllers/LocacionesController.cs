using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Totem_API.Data;
using Totem_API.Models;

namespace Totem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocacionesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TotemContext _context;

        public LocacionesController(IConfiguration configuration, TotemContext context)
        {
            _configuration = configuration;
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



        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocacion(int id, [FromForm] LocacionUploadModel LocacionInput)
        {
            var locacion = await _context.Locacions.FindAsync(id);

            if (locacion == null)
            {
                return NotFound();
            }

            // Actualizar los campos del publicidad con los datos del modelo
            locacion.Nombre = LocacionInput.Nombre;
            locacion.Descripcion = LocacionInput.Descripcion;
            locacion.Keywords = string.Join(",", LocacionInput.Keywords);
            locacion.IdTotem = LocacionInput.IdTotem;
            // Upload new carousel images to Blob Storage, if any
            if (LocacionInput.ImagenesCarrucel != null)
            {
                // Delete the old carousel images from Blob Storage
                if (!string.IsNullOrEmpty(locacion.UrlCarruselImagenes))
                {
                    var blobServiceClient1 = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureBlobStorage")).CreateCloudBlobClient();
                    var container1 = blobServiceClient1.GetContainerReference("contenedortotem");
                    var blob1 = container1.GetBlockBlobReference(Path.GetFileName(locacion.UrlCarruselImagenes));
                    await blob1.DeleteIfExistsAsync();
                }

                // Upload the new carousel images to Blob Storage
                var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                var blobContainerName = "contenedortotem";
                var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
                var container = blobServiceClient.GetContainerReference(blobContainerName);
                await container.CreateIfNotExistsAsync();

                var urls = new List<string>();
                foreach (var imagen in LocacionInput.ImagenesCarrucel)
                {
                    var blobName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    var blob = container.GetBlockBlobReference(blobName);
                    await blob.UploadFromStreamAsync(imagen.OpenReadStream());
                    urls.Add(blob.Uri.ToString());
                }

                locacion.UrlCarruselImagenes = string.Join(',', urls);
            }

            // Upload new map image to Blob Storage, if any
            if (LocacionInput.ImagenMapa != null)
            {
                // Delete the old map image from Blob Storage
                if (!string.IsNullOrEmpty(locacion.UrlMapa))
                {
                    var blobServiceClient2 = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureBlobStorage")).CreateCloudBlobClient();
                    var container2 = blobServiceClient2.GetContainerReference("contenedortotem");
                    var blob2 = container2.GetBlockBlobReference(Path.GetFileName(locacion.UrlMapa));
                    await blob2.DeleteIfExistsAsync();
                }

                // Upload the new map image to Blob Storage
                var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                var blobContainerName = "contenedortotem";
                var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
                var container = blobServiceClient.GetContainerReference(blobContainerName);
                await container.CreateIfNotExistsAsync();
                var blobName = Guid.NewGuid().ToString() + Path.GetExtension(LocacionInput.ImagenMapa.FileName);
                var blob = container.GetBlockBlobReference(blobName);
                await blob.UploadFromStreamAsync(LocacionInput.ImagenMapa.OpenReadStream());
                locacion.UrlMapa = blob.Uri.ToString();
            }

            // Save changes to database
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost]
        public async Task<ActionResult<Locacion>> PostLocacion([FromForm] LocacionUploadModel inputModel)
        {
            if (inputModel == null || inputModel.Nombre == null || inputModel.Descripcion == null || inputModel.Keywords == null || inputModel.ImagenesCarrucel == null || inputModel.ImagenMapa == null || inputModel.IdTotem == null)
            {
                return BadRequest("Invalid request");
            }

            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");

            if (connectionString == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "AzureBlobStorage connection string is missing");
            }

            // Upload image to Blob Storage
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(inputModel.ImagenMapa.FileName);
            var blobContainerName = "contenedortotem";
            var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            var container = blobServiceClient.GetContainerReference(blobContainerName);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(inputModel.ImagenMapa.OpenReadStream());

            // Upload image to Blob Storage
            var urlsCarruselImagenes = new List<string>();
            foreach (var imagen in inputModel.ImagenesCarrucel)
            {
                var blobName2 = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                var blobContainerName2 = "contenedortotem";
                var blobServiceClient2 = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
                var container2 = blobServiceClient2.GetContainerReference(blobContainerName2);
                await container2.CreateIfNotExistsAsync();
                var blob2 = container2.GetBlockBlobReference(blobName2);
                await blob2.UploadFromStreamAsync(imagen.OpenReadStream());
                urlsCarruselImagenes.Add(blob2.Uri.ToString());
            }

            // Save data to database
            var locacion = new Locacion
            {
                Nombre = inputModel.Nombre,
                Descripcion = inputModel.Descripcion,
                Keywords = string.Join(",", inputModel.Keywords),
                UrlCarruselImagenes = string.Join("|", urlsCarruselImagenes), // Merge URLs of carousel images into a single string with '|' delimiter
                UrlMapa = blob.Uri.ToString(),
                IdTotem = inputModel.IdTotem
            };

            if (_context.Totems == null)
            {
                return Problem("Entity set 'TotemContext.Totems' is null.");
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
