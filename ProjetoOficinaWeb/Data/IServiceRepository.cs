using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Data
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        public IEnumerable<SelectListItem> GetComboServices();
    }
}
