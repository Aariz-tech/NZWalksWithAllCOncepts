using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Net;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        /// <summary>
        /// This method will create Walk
        /// </summary>
        /// <param name="addWalkRequestDto"></param>
        /// <returns>AddWalkRequestDto</returns>
        [HttpPost]
        public async Task<IActionResult> CreateWalk([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            if(ModelState.IsValid)
            {
                //Map Dto to Domain model
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
                walkDomainModel = await walkRepository.CreateWalkAsync(walkDomainModel);
                if (walkDomainModel == null)
                {
                    return NotFound();
                }
                //Map domain model to dto model 
                var walkDTOModel = mapper.Map<WalkDto>(walkDomainModel);
                return Ok(walkDTOModel);
                //return CreatedAtAction(nameof(GetRegionByID), new { id = regionDto.Id }, regionDto);
            }
            return BadRequest(ModelState);

        }
        /// <summary>
        /// This Method will return all the Walks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetALLWalks([FromQuery] string? filterOn, [FromQuery] string? filterQuery
            , [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000
            )
        {
            //try
            //{
                //throw new Exception("There was an error");
                var getAllWalksDomainModel = await walkRepository.GetAllWalkAsync(filterOn, filterQuery, sortBy, isAscending ?? true
                        , pageNumber, pageSize);
                if (getAllWalksDomainModel.Count == 0)
                {
                    return NotFound();
                }
            //create an exception 
            throw new Exception("This is a custom exception");
                //domain model to dto
                var getALLDto = mapper.Map<List<WalkDto>>(getAllWalksDomainModel);
                return Ok(getALLDto);
           // }
           // catch (Exception ex)
            //{
                //log the error message
               // return Problem("Something went wrong!", null, (int)HttpStatusCode.InternalServerError);
                
            //}
        }
        /// <summary>
        /// This method will retrun Walk by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>WalkDto</returns>
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetWalkByID([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.GetWalkByIDAsync(id);
            if(walkDomainModel == null)
                return NotFound();
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }
        /// <summary>
        /// this method will update walk by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateWalkRequestDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateWalkById([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            if(ModelState.IsValid)
            {
                //map dto to domain model
                var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);
                walkDomainModel = await walkRepository.UpdateWalkByIdAsync(id, walkDomainModel);
                if (walkDomainModel == null)
                    return NotFound();
                //map domain model to dto 
                var walkDtoModel = mapper.Map<WalkDto>(walkDomainModel);
                return Ok(walkDtoModel);
            }
            return BadRequest(ModelState);
            
        }
        /// <summary>
        /// this method will delete walk by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteWalkByID([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepository.DeleteWalkByIdAsync(id);
            if (walkDomainModel == null) return NotFound();
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

    }
}
