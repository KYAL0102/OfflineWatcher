using Core.Contracts;

namespace Persistence;

using Base.Persistence;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public IDoctorRepository Doctors { get; }
    public IPatientRepository Patients { get; }
    public IExaminationRepository Examinations { get; }
    public IDatastreamRepository Datastreams { get; }

    public UnitOfWork(ApplicationDbContext context) : base(context)
    {
        Doctors = new DoctorRepository(context);
        Patients = new PatientRepository(context);
        Examinations = new ExaminationRepository(context);
        Datastreams = new DatastreamRepository(context);
    }

    public UnitOfWork() : this(new ApplicationDbContext())
    {
        
    }
}