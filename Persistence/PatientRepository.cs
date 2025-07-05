using Core.Contracts;
using Core.DataTransferObjects;
using Core.Entities;

namespace Persistence;

using Base.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    private readonly ApplicationDbContext _dbContext;
    public PatientRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PatientDTO>> GetAllPatients()
    {
        var queue = _dbContext.Patients.AsQueryable();

        return await queue.Select(patient => new PatientDTO
        {
            SSID = patient.SSID,
            LastName = patient.LastName,
            FirstName = patient.FirstName,
            ExaminationCount = patient.Examinations!.Count,
            LastExamination = patient.Examinations.OrderByDescending(e => e.ExaminationDate).FirstOrDefault()!.ExaminationDate
        }).ToListAsync();

        /*return ;*/
    }

    public async Task UpdateFirstAndLastNameOfPatient(string ssid, string firstName, string lastName)
    {
        var queue = _dbContext.Patients.AsQueryable();
        
        var patient = queue.FirstOrDefault(p => p.SSID == ssid);
        
        patient!.FirstName = firstName;
        patient.LastName = lastName;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Patient?> GetPatientBySSID(string ssid)
    {
        var queue = _dbContext.Patients.AsQueryable();
        
        return await queue
            .Include(p => p.Examinations)!
            .ThenInclude(e => e.DataStreams)
            .SingleOrDefaultAsync(p => p.SSID == ssid);
    }
}