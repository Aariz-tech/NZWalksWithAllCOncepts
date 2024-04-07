using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LocalImageRepository(NZWalksDbContext nZWalksDbContext, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            this.nZWalksDbContext = nZWalksDbContext;
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Image> UploadImage(Image image)
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images"
                ,$"{image.FileName}{image.FileExtension}" );

            //upload image to local path 
            using var stream = new FileStream(localFilePath,FileMode.Create);
            await image.File.CopyToAsync(stream);
            //https://localhost:1234/Images/image.png
            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;
            // add image to database
            await nZWalksDbContext.Images.AddAsync(image);
            await nZWalksDbContext.SaveChangesAsync();
            return image;

        }
    }
}
