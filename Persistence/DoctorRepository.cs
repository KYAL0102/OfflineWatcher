using Core.Contracts;
using Core.Entities;

namespace Persistence;

using Base.Persistence;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}