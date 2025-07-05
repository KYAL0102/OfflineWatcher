using Base.Tools;

using Core.Entities;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Persistence;


public class ApplicationDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Examination> Examinations { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DataStream> DataStream { get; set; }

    /// <summary>
    /// Parameterless constructor reads the connection string from appsettings.json (at design time)
    /// For migration generation! Note: The constructor must be the first one in order.
    /// </summary>
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //We need this for migration
            var connectionString = ConfigurationHelper.GetConfiguration().Get("DefaultConnection", "ConnectionStrings");
            optionsBuilder.UseSqlServer(connectionString);
        }

        optionsBuilder
            .EnableSensitiveDataLogging()
            .LogTo(message => Trace.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}