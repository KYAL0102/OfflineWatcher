using Base.Persistence;

using Core.Contracts;
using Core.Entities;

namespace Persistence;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ExaminationRepository : GenericRepository<Examination>, IExaminationRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ExaminationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetCountOfDataStreams()
    {
        var queue = _dbContext.Examinations.AsQueryable();

        return await queue.SumAsync(e => e.DataStreams.Count);
    }
}