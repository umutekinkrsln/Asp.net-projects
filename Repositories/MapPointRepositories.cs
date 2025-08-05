
using MapApp.Data;
using MapApp.Models;
using MapApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MapApp.Repositories
{
    public class MapPointRepository : IMapPointRepository
    {
        private readonly AppDbContext _context;

        public MapPointRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MapPoints>> GetAllAsync()
        {
            return await _context.MapPoints.ToListAsync();
        }

        public async Task<MapPoints?> GetByIdAsync(int id)
        {
            return await _context.MapPoints.FindAsync(id);
        }

        public async Task<MapPoints> CreateAsync(MapPoints point)
        {
            _context.MapPoints.Add(point);
            await _context.SaveChangesAsync();
            return point;
        }

        public async Task UpdateAsync(MapPoints point)
        {
            _context.MapPoints.Update(point);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var point = await _context.MapPoints.FindAsync(id);
            if (point != null)
            {
                _context.MapPoints.Remove(point);
                await _context.SaveChangesAsync();
            }
        }
    }
}

