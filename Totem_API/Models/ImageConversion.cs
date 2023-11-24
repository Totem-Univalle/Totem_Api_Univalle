namespace Totem_API.Models
{
    using Microsoft.AspNetCore.Http;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.IO;
    public class ImageConversion
    {
        public static string ConvertToBase64(IFormFile imageFile, int maxSizeInBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                imageFile.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                if (imageBytes.Length > maxSizeInBytes)
                {
                    using (var image = Image.Load(imageBytes))
                    {
                        int maxWidth = 1280;
                        int maxHeight = 720;

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(maxWidth, maxHeight),
                            Mode = ResizeMode.Max
                        }));

                        using (var resizedMemoryStream = new MemoryStream())
                        {
                            image.SaveAsJpeg(resizedMemoryStream);
                            imageBytes = resizedMemoryStream.ToArray();
                        }
                    }
                }

                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}
