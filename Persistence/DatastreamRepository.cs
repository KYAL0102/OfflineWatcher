using Base.Persistence;
using Core.Contracts;
using Core.Entities;

namespace Persistence;

public class DatastreamRepository : GenericRepository<DataStream>, IDatastreamRepository
{
    public DatastreamRepository(ApplicationDbContext dbContext): base(dbContext)
    {
        
    }
}