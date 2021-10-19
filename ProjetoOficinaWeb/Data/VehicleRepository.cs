using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;

namespace ProjetoOficinaWeb.Data
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public VehicleRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Vehicles.Include(p => p.UserId); // como se fosse um join do SQL
        }

        public async Task<IQueryable<Vehicle>> GetVehicleAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Receptionist")) // se for Admin vê todos os veículos
            {
                return _context.Vehicles
                .Include(o => o.UserId) // ir buscar dados entre tabelas ligadas diretamente (como se fosse um join)
                //.Include(o => o.User.Email)
                .Include(o => o.LicensePlate); // ir buscar dados entre tabelas ligadas diretamente (como se fosse um join)
            }

            return _context.Vehicles // cada utilizador vê só os seus veículos
                .Include(o => o.LicensePlate);
                //.Include(o => o.User.Email);
                //.Where(o => o.UserId == user);
        }

        public async Task<Vehicle> GetVehicleAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public IEnumerable<SelectListItem> GetComboVehicles()
        {
            var list = _context.Vehicles.Select(p => new SelectListItem // como se fosse um foreach
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
