
using MapApp.Data;
using MapApp.Models;
using MapApp.Repositories;

namespace MapApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<MapPoints> MapPoints { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            MapPoints = new GenericRepository<MapPoints>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

