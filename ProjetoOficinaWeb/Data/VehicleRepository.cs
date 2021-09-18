using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Data
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly DataContext _context;

        public VehicleRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.vehicles.Include(p => p.User); // como se fosse um join do SQL
        }

        public IEnumerable<SelectListItem> GetComboVehicles()
        {
            var list = _context.vehicles.Select(p => new SelectListItem // como se fosse um foreach
            {
                Text = p.LicensePlate, // texto que vai aparecer na combobox
                Value = p.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a licence Plate...)",
                Value = "0"
            });

            return list;
        }
    }
}
