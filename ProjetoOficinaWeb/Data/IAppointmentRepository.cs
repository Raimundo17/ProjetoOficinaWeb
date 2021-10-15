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

        Task AddItemToAppointmentAsync(AddItemViewModel model, string userName);

        Task ModifyAppointmentDetailTempQuantityAsync(int id, double quantity);

        Task DeleteDetailTempAsync(int id);

        Task<bool> ConfirmAppointmentAsync(string userName);

        Task RepairOrder(RepairViewModel model);

        Task<Appointment> GetAppointmentAsync(int id);
    }
}
