using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Collections.Generic;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext walksDbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper, ILogger<RegionsController> logger)
        {
            this.walksDbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }
        /// <summary>
        /// This method will return all the regions
        /// </summary>
        /// <returns>List of regions</returns>
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAllRegions()
        {
            try
            {
                //throw new Exception("For testing purpose throwing Custom Error");
                logger.LogInformation("GetAllRegions Action method invoked");
                #region Test
                //var regions = new List<Region>{
                //    new Region()
                //    {
                //        Id = Guid.NewGuid(),
                //        Name = "TestRegion1",
                //        Code = "TR01",
                //        RegionImageUrl = "TestRegionImageURL1.png"

                //    },
                //    new Region()
                //    {
                //        Id = Guid.NewGuid(),
                //        Name = "TestRegion2",
                //        Code = "TR02",
                //        RegionImageUrl = "TestRegionImageURL2.png"

                //    },

                //};
                #endregion
                //Get Data from Database - Domain Model
                //var regions = await walksDbContext.Regions.ToListAsync();
                var regions = await regionRepository.GetAllRegionAsync();
                //Map Domain model to DTOs
                //List<RegionDto> regionsDto = new List<RegionDto>();
                //foreach (var region in regions)
                //{
                //    regionsDto.Add(new RegionDto()
                //    {
                //        Id = region.Id,
                //        Name = region.Name,
                //        Code = region.Code,
                //        RegionImageUrl = region.RegionImageUrl,
                //    });
                //}
                //var regionsDto = mapper.Map<List<RegionDto>>(regions);
                //Return Dto
                //return Ok(regionsDto);

                //Return Dto
                logger.LogInformation($"Finished GetAllRegions Request with data : {JsonSerializer.Serialize(regions)}");
                return Ok(mapper.Map<List<RegionDto>>(regions));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
            
        }
        /// <summary>
        /// This method will return Region By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Region</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetRegionByID([FromRoute] Guid id)
        {
            //Get Data from Database - DomainModel
            //Region regionDomain = await walksDbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            Region regionDomain = await regionRepository.GetRegionByIdAsync(id);
            //Region region = walksDbContext.Regions.Find(id);
            if (regionDomain == null)
            {
                return NotFound();
            }
            //Map Domain Model to DTOs
            //RegionDto regionDto = new RegionDto
            //{
            //    Id = regionDomain.Id,
            //    Name = regionDomain.Name,
            //    Code = regionDomain.Code,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomain);
            return Ok(regionDto);
        }
        /// <summary>
        /// This method will create Region
        /// </summary>
        /// <param name="regionRequestDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateRegion([FromBody] AddRegionRequestDto regionRequestDto)
        {
            if (ModelState.IsValid)
            {
                //Convert DTO to DOmain Model 
                //var regionDomainModel = new Region
                //{
                //    Code = regionRequestDto.Code,
                //    Name = regionRequestDto.Name,
                //    RegionImageUrl = regionRequestDto.RegionImageUrl
                //};
                var regionDomainModel = mapper.Map<Region>(regionRequestDto);
                //await walksDbContext.Regions.AddAsync(regionDomainModel);
                //await walksDbContext.SaveChangesAsync();
                regionDomainModel = await regionRepository.CreateRegionAsync(regionDomainModel);
                //convert domain model to dto
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name
                //};
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return CreatedAtAction(nameof(GetRegionByID), new { id = regionDto.Id }, regionDto);
            }
            else
            {
                return BadRequest(ModelState);
            }

        }
        /// <summary>
        /// this method will be use to update the region
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateRegionRequestDto"></param>
        /// <returns>RegionDto</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]//custom validation
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {

            //var regionDomainModel = new Region
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl,
            //};
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);
            regionDomainModel = await regionRepository.UpdateRegionAsync(id, regionDomainModel);
            //var regionDomainModel = await walksDbContext.Regions.FirstOrDefaultAsync(x=> x.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }
            //convert dto model to domain model
            //regionDomainModel.Code = updateRegionRequestDto.Code;
            //regionDomainModel.Name = updateRegionRequestDto.Name;
            //regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;
            //await walksDbContext.SaveChangesAsync();
            //map domain model to DTO model
            //var regionDtoModel = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name
            //};
            var regionDtoModel = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDtoModel);



        }
        /// <summary>
        /// this method will delete the exisitng region
        /// </summary>
        /// <param name="id"></param>
        /// <returns>RegionDto</returns>
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            //var regionDomainModel = await walksDbContext.Regions.FirstOrDefaultAsync(y=> y.Id == id);
            var regionDomainModel = await regionRepository.DeleteRegionAsync(id);
            if (regionDomainModel == null)
                return NotFound();
            //walksDbContext.Regions.Remove(regionDomainModel);
            //await walksDbContext.SaveChangesAsync();
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            return Ok(regionDto);
        }

    }
}







