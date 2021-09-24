using System.Linq;
using System.Threading.Tasks;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Data
{
    public interface IAppointmentRepository : IGenericRepository<Appointment> // Passo o CRUD básico
    {
        Task<IQueryable<Appointment>> GetAppointmentAsync(string userName); // dá todas as marcações de um determinado user
        // lista da tabela não enumerada

        Task<IQueryable<AppointmentDetailTemp>> GetDetailTempsAsync(string userName); // Dou lhe um user e ele dá o temporário desse user

        Task AddItemToOrderAsync(AddItemViewModel model, string userName);

        Task DeleteDetailTempAsync(int id);
    }
}
