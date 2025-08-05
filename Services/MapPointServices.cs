
using MapApp.Models;
using MapApp.Repositories;

namespace MapApp.Services
{
    public class MapPointServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public MapPointServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<MapPoints>> GetAllAsync() =>
            _unitOfWork.MapPoints.GetAllAsync();

        public Task<MapPoints?> GetByIdAsync(int id) =>
            _unitOfWork.MapPoints.GetByIdAsync(id);

        public async Task<MapPoints> CreateAsync(MapPoints point)
        {
            var created = await _unitOfWork.MapPoints.CreateAsync(point);
            await _unitOfWork.SaveChangesAsync();
            return created;
        }

        public async Task UpdateAsync(MapPoints point)
        {
            await _unitOfWork.MapPoints.UpdateAsync(point);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.MapPoints.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

