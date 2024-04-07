using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext _nZWalksDbContext;
        public SQLRegionRepository(NZWalksDbContext nZWalksDbContext)
        {
            this._nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<Region> CreateRegionAsync(Region region)
        {
            await _nZWalksDbContext.Regions.AddAsync(region);
            await _nZWalksDbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteRegionAsync(Guid id)
        {
            var exisitingRegion = await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x =>x.Id == id);
            if (exisitingRegion == null)
            {
                return null;
            }
            _nZWalksDbContext.Regions.Remove(exisitingRegion);
            await _nZWalksDbContext.SaveChangesAsync();
            return exisitingRegion;
        }

        /// <summary>
        /// this method will retrun All the Regions from Database.
        /// </summary>
        /// <returns>List<Region></returns>
        public async Task<List<Region>> GetAllRegionAsync()
        {
            return await _nZWalksDbContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetRegionByIdAsync(Guid id)
        {
            return await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<Region?> UpdateRegionAsync(Guid id, Region region)
        {
            Region exisitngRegion = await _nZWalksDbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if(exisitngRegion == null)
            {
                return null;
            }
            exisitngRegion.Code = region.Code;
            exisitngRegion.Name = region.Name;
            exisitngRegion.RegionImageUrl   = region.RegionImageUrl;
            await _nZWalksDbContext.SaveChangesAsync();
            return exisitngRegion;
        }
    }
}
