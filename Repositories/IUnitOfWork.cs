using MapApp.Models;
using MapApp.Repositories;
using System.Threading.Tasks;

namespace MapApp.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<MapPoints> MapPoints { get; }
        Task<int> SaveChangesAsync();
    }
}

