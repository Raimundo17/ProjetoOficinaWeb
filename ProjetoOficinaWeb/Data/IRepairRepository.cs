using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoOficinaWeb.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Data
{
    public interface IRepairRepository : IGenericRepository<Repair> // Passo o CRUD básico
    {
        public IQueryable GetAllAppointments(); // dá todas os repairs de um determinado user

        IEnumerable<SelectListItem> GetComboVehicles();

        IEnumerable<SelectListItem> GetComboServices();

        IEnumerable<SelectListItem> GetComboAppointments();

        IEnumerable<SelectListItem> GetComboMechanics();

        IEnumerable<SelectListItem> GetComboRepair();

        //Task<Repair> GetByIdRepairAsync(int id);
    }
}
