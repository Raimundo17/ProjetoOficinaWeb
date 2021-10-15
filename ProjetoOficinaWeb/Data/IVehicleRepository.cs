using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Data
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        public IQueryable GetAllWithUsers();

        public IEnumerable<SelectListItem> GetComboVehicles();

        Task<IQueryable<Vehicle>> GetVehicleAsync(string userName); // dá todas as marcações de um determinado user
    }
}
