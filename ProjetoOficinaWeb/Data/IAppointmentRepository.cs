using System.Linq;
using System.Threading.Tasks;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Data
{
    public interface IAppointmentRepository : IGenericRepository<Appointment> // Passo já o CRUD básico
    {
        Task<IQueryable<Appointment>> GetAppointmentAsync(string userName); // dá todas as marcações de um determinado user

        Task<IQueryable<AppointmentDetailTemp>> GetDetailTempsAsync(string userName); // Dou lhe um user e ele dá o temporário desse user
    }
}
