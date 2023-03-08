using Microsoft.AspNetCore.Mvc;

namespace uploadAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{

private readonly ILogger<ImageController> _logger;
private string _imagePath = string.Empty;
        public ImageController(ILogger<ImageController> logger, IConfiguration configuration)
    {
        _imagePath = configuration["_ImagePath"] ?? String.Empty;
        _logger = logger;
    }
[HttpPost("upload"), DisableRequestSizeLimit] 
public IActionResult UploadImage() 
{ 
    _logger.LogInformation("dette er imagepath ved init: " + _imagePath);
    List<Uri> images = new List<Uri>(); 
    _logger.LogInformation("Upload funktion ramt");
    try 
    { 
        foreach (var formFile in Request.Form.Files)  
        { 
            // Validate file type and size 
            _logger.LogInformation("Upload funktion ramt2");
                        if (formFile.ContentType != "image/jpeg" && formFile.ContentType != "image/png") 
            { 
                return BadRequest($"Invalid file type for file {formFile.FileName}. Only JPEG and PNG files are allowed."); 
            } 
            if (formFile.Length > 1048576) // 1MB 
            { 
                return BadRequest($"File {formFile.FileName} is too large. Maximum file size is 1MB."); 
            } 
            if (formFile.Length > 0) 
            { 
                _logger.LogInformation("Upload funktion ramt3");
                var fileName = "image-" + Guid.NewGuid().ToString() + ".jpg"; 
                var fullPath = _imagePath + Path.DirectorySeparatorChar + fileName; 

                _logger.LogInformation("Dette er fullPath efter pil: " + fullPath.ToString());
                
                using (var stream = new FileStream(fullPath, FileMode.Create)) 
                { 
                    formFile.CopyTo(stream); 
                } 
 
                var imageURI = new Uri(fileName, UriKind.RelativeOrAbsolute); 
                images.Add(imageURI); 
            }  
            else  
            { 
                return BadRequest("Empty file submited."); 
            } 
        } 
 
    } 
    catch (Exception ex) 
    { 
        _logger.LogError(ex.ToString());
        return StatusCode(500, $"Internal server error."); 
    } 
 
    return Ok(images); 
} 
}