using Ecommerce.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {
        private readonly FileStorageOptions _options;

        public ProductImageController(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string baseFolder = _options.ImagePath;  // Path from appsettings.json

            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            string filePath = Path.Combine(baseFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Public URL for React UI
            string url = $"{Request.Scheme}://{Request.Host}/product-images/{file.FileName}";

            return Ok(new { imageUrl = url });
        }



        [HttpGet("all-images")]
        public IActionResult GetAllImages()
        {
            string imageFolder = _options.ImagePath;  // From appsettings.json

            if (!Directory.Exists(imageFolder))
                return NotFound("Image directory not found.");

            var files = Directory.GetFiles(imageFolder);

            var result = files.Select(file =>
            {
                string fileName = Path.GetFileName(file);

                // public URL for UI
                string url = $"{Request.Scheme}://{Request.Host}/product-images/{fileName}";

                return new
                {
                    FileName = fileName,
                    Url = url
                };
            });

            return Ok(result);
        }

        [HttpGet("ImageName")]
        public IActionResult GetImageByName(string name)
        {
            string imageFolder = _options.ImagePath;  // From appsettings.json

            if (!Directory.Exists(imageFolder))
                return NotFound("Image directory not found.");

            var files = Directory.GetFiles(imageFolder);

            var result = files.Select(file =>
            {
                string fileName = Path.GetFileName(file);

                // public URL for UI
                string url = $"{Request.Scheme}://{Request.Host}/product-images/{fileName}";

                return new
                {
                    FileName = fileName,
                    Url = url
                };
            });

            return Ok(result);
        }

    }

}
