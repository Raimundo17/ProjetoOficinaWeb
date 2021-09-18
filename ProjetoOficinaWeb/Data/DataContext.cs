using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Data
{
    public class DataContext : IdentityDbContext<User> // Classe específica que fica responsável pela ligação á base de dados, injeto o meu user
    {
        public DbSet<Client> Clients { get; set; }

        public DbSet<Mechanic> Mechanics { get; set; }

        public DbSet<Receptionist> Receptionists { get; set; }

        public DbSet<Vehicle> vehicles { get; set; } // criar a tabela
                                                     //vehicles -> propriedade que fica ligada á tabela Vehicle através do DataContext

        public DbSet<Service> Services { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<AppointmentDetail> AppointmentDetails { get; set; }

        public DbSet<AppointmentDetailTemp> AppointmentDetailsTemp { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) // Injetar o DataContext da Entity Framework Core na minha
        {
        }
    }
}
