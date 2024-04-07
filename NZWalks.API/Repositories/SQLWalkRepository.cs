using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;

        public SQLWalkRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }
        public async Task<Walk> CreateWalkAsync(Walk walk)
        {
            await nZWalksDbContext.Walks.AddAsync(walk);
            await nZWalksDbContext.SaveChangesAsync();
            return walk;

        }

        public async Task<Walk?> DeleteWalkByIdAsync(Guid id)
        {
            var existingWalkDomainModel = await GetWalkByIDAsync(id);
            if (existingWalkDomainModel == null) {
                return null;
            }
            nZWalksDbContext.Walks.Remove(existingWalkDomainModel);
            await nZWalksDbContext.SaveChangesAsync();
            return existingWalkDomainModel;
        }

        public async Task<List<Walk>> GetAllWalkAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000
            )
        {
            var walks = nZWalksDbContext.Walks.Include("Difficulty").Include(x => x.Region).AsQueryable();
            //filtering
            if(!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery)) 
            { 
                if(filterOn.Equals("Name",StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
                
            
            }

            // sorting
            if(!string.IsNullOrWhiteSpace(sortBy))
            {
                if(sortBy.Equals("Name",StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x=>x.Name) :walks.OrderByDescending(x=>x.Name);
                }
                else if (sortBy.Equals("LengthInKm", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }
            //Pagination
            var skipResults = (pageNumber - 1)*pageSize;
            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
            //return await walks.ToListAsync();

            //return await nZWalksDbContext.Walks.Include("Difficulty").Include(x=>x.Region).ToListAsync();


        }

        public async Task<Walk?> GetWalkByIDAsync(Guid id)
        {
            var walkDomainModel = await nZWalksDbContext.Walks.Include(x=>x.Difficulty).Include(x=>x.Region).FirstOrDefaultAsync(x => x.Id == id);
            if (walkDomainModel == null)
            {
                return null;
            }
            return walkDomainModel;
        }
        /// <summary>
        /// this method will return updated walk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="walk"></param>
        /// <returns></returns>
        public async Task<Walk?> UpdateWalkByIdAsync(Guid id, Walk walk)
        {
            var existingWalkDomainModel = await nZWalksDbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWalkDomainModel == null)
            {
                return null;
            }
            existingWalkDomainModel.Name = walk.Name;
            existingWalkDomainModel.Description = walk.Description;
            existingWalkDomainModel.LengthInKm = walk.LengthInKm;
            existingWalkDomainModel.DifficultyId = walk.DifficultyId;
            existingWalkDomainModel.WalkImageUrl = walk.WalkImageUrl;
            existingWalkDomainModel.RegionId = walk.RegionId;
            await nZWalksDbContext.SaveChangesAsync();
            return existingWalkDomainModel;
        }
    }
}
