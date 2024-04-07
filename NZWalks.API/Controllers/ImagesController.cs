using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }
        [HttpPost]
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadRequestDto imageUploadRequestDto)
        {
            ValidateImageUpload(imageUploadRequestDto);
            if(ModelState.IsValid)
            {
                //convert dto to domain model 
                var imageUploadDomainModel = new Image
                {
                    File = imageUploadRequestDto.File,
                    FileDescription = imageUploadRequestDto.FileDescription,
                    FileExtension = Path.GetExtension(imageUploadRequestDto.File.FileName),
                    FileName = imageUploadRequestDto.FileName,
                    FileSizeInBytes = imageUploadRequestDto.File.Length
                };
                //User Respository to upload image
                imageUploadDomainModel = await imageRepository.UploadImage(imageUploadDomainModel);
                return Ok(imageUploadDomainModel);
            }
            return BadRequest(ModelState);
        }
        private void ValidateImageUpload(ImageUploadRequestDto imageUploadRequestDto)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if(!allowedExtensions.Contains(Path.GetExtension(imageUploadRequestDto.File.FileName))) {
                ModelState.AddModelError("file", "Unsupported file extension");
            }
            if(imageUploadRequestDto.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size is more than 10MB, please upload smaller image size.");
            }
        }
    }
}
