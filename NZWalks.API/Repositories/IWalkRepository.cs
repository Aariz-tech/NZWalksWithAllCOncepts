using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk> CreateWalkAsync(Walk walk);
        Task<Walk?> DeleteWalkByIdAsync(Guid id);
        Task<List<Walk>> GetAllWalkAsync(string? filterOn=null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Walk?> GetWalkByIDAsync(Guid id);

        Task<Walk?> UpdateWalkByIdAsync(Guid id, Walk walk);
    }
}
