using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Data
{
    public interface IReceptionistRepository : IGenericRepository<Receptionist>
    {
        public IQueryable GetAllWithUsers();

        public IEnumerable<SelectListItem> GetComboReceptionists();
    }
}
