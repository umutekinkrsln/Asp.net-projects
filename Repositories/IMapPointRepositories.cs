
using MapApp.Models;

namespace MapApp.Repositories
{
    public interface IMapPointRepository
    {
        Task<IEnumerable<MapPoints>> GetAllAsync();
        Task<MapPoints?> GetByIdAsync(int id);
        Task<MapPoints> CreateAsync(MapPoints point);
        Task UpdateAsync(MapPoints point);
        Task DeleteAsync(int id);
    }
}
