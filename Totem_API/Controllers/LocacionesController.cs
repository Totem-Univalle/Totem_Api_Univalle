using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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


        //[Authorize]
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

                // Upload the new carousel images to Blob Storage
                //var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                //var blobContainerName = "contenedortotem";
                //var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
                //var container = blobServiceClient.GetContainerReference(blobContainerName);
                //await container.CreateIfNotExistsAsync();

                var urls = new List<string>();
                foreach (var imagen in LocacionInput.ImagenesCarrucel) // AQUI HACER CONVERSION DE IMAGEN A BASE64
                {
                    //var blobName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    //var blob = container.GetBlockBlobReference(blobName);
                    //await blob.UploadFromStreamAsync(imagen.OpenReadStream());

                    string imageBase64 = ImageConversion.ConvertToBase64(imagen,10);
                    urls.Add(imageBase64);
                }

                locacion.UrlCarruselImagenes = string.Join(',', urls);
            }

            // Upload new map image to Blob Storage, if any
            if (LocacionInput.ImagenMapa != null)
            {
                //// Delete the old map image from Blob Storage
                //if (!string.IsNullOrEmpty(locacion.UrlMapa))
                //{
                //    var blobServiceClient2 = CloudStorageAccount.Parse(_configuration.GetConnectionString("AzureBlobStorage")).CreateCloudBlobClient();
                //    var container2 = blobServiceClient2.GetContainerReference("contenedortotem");
                //    var blob2 = container2.GetBlockBlobReference(Path.GetFileName(locacion.UrlMapa));
                //    await blob2.DeleteIfExistsAsync();
                //}
                //AQUI COLOCAR LA IMAGEN DEL MAPA EN BINARIO
                // Upload the new map image to Blob Storage
                //var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                //var blobContainerName = "contenedortotem";
                //var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
                //var container = blobServiceClient.GetContainerReference(blobContainerName);
                //await container.CreateIfNotExistsAsync();
                //var blobName = Guid.NewGuid().ToString() + Path.GetExtension(LocacionInput.ImagenMapa.FileName);
                //var blob = container.GetBlockBlobReference(blobName);
                //await blob.UploadFromStreamAsync(LocacionInput.ImagenMapa.OpenReadStream());
                string imageBase64 = ImageConversion.ConvertToBase64(LocacionInput.ImagenMapa, 10);
                locacion.UrlMapa = imageBase64;
            }

            // Save changes to database
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //[Authorize]
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

            // Upload image to Blob Storage IMAGEN MAPA
            string urlMapa = ImageConversion.ConvertToBase64(inputModel.ImagenMapa,10);
            //var blobName = Guid.NewGuid().ToString() + Path.GetExtension(inputModel.ImagenMapa.FileName);
            //var blobContainerName = "contenedortotem";
            //var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            //var container = blobServiceClient.GetContainerReference(blobContainerName);
            //await container.CreateIfNotExistsAsync();
            //var blob = container.GetBlockBlobReference(blobName);
            //await blob.UploadFromStreamAsync(inputModel.ImagenMapa.OpenReadStream());

            // Upload image to Blob Storage
            var urlsCarruselImagenes = new List<string>();
            foreach (var imagen in inputModel.ImagenesCarrucel)
            {
                //var blobName2 = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                //var blobContainerName2 = "contenedortotem";
                //var blobServiceClient2 = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
                //var container2 = blobServiceClient2.GetContainerReference(blobContainerName2);
                //await container2.CreateIfNotExistsAsync();
                //var blob2 = container2.GetBlockBlobReference(blobName2);
                //await blob2.UploadFromStreamAsync(imagen.OpenReadStream());
                string imageBase64 = ImageConversion.ConvertToBase64(imagen, 10);
                urlsCarruselImagenes.Add(imageBase64);
            }

            // Save data to database
            var locacion = new Locacion
            {
                Nombre = inputModel.Nombre,
                Descripcion = inputModel.Descripcion,
                Keywords = string.Join(",", inputModel.Keywords),
                UrlCarruselImagenes = string.Join("|", urlsCarruselImagenes), // Merge URLs of carousel images into a single string with '|' delimiter
                UrlMapa = urlMapa,
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
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocacion(int id)
        {
            var locacion = await _context.Locacions.FindAsync(id);
            if (locacion == null)
            {
                return NotFound();
            }

            // Eliminar las imágenes del carrusel de Azure Blob Storage
            //var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            //var containerName = "contenedortotem";
            //var blobServiceClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
            //var containerClient = blobServiceClient.GetContainerReference(containerName);

            //var urlsCarruselImagenes = locacion.UrlCarruselImagenes.Split('|');
            //foreach (var urlImagen in urlsCarruselImagenes)
            //{
            //    var blobName = Path.GetFileName(urlImagen);
            //    var blobClient = containerClient.GetBlockBlobReference(blobName);
            //    await blobClient.DeleteIfExistsAsync();
            //}

            //// Eliminar la imagen principal de Azure Blob Storage
            //if (!string.IsNullOrEmpty(locacion.UrlMapa))
            //{
            //    var blobName = Path.GetFileName(locacion.UrlMapa);
            //    var blobClient = containerClient.GetBlockBlobReference(blobName);
            //    await blobClient.DeleteIfExistsAsync();
            //}

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
