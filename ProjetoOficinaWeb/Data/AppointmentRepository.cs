using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Data
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public AppointmentRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Appointment>> GetAppointmentAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Admin")) // se for Admin vê todos as marcações
            {
                return _context.Appointments
                    .Include(o => o.Items) // ir buscar dados entre tabelas ligadas diretamente
                    .ThenInclude(i => i.Vehicle) // ir buscar dados entre tabelas onde estejam ligadas por uma tabela a meio
                    .OrderByDescending(o => o.RepairDate);
            }

            return _context.Appointments // cada utilizador vê só os seus orders
                .Include(o => o.Items)
                .ThenInclude(p => p.Vehicle)
                .Where(o => o.User == user)
                .OrderByDescending(o => o.RepairDate);
        }

        public async Task<IQueryable<AppointmentDetailTemp>> GetDetailTempsAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            return _context.AppointmentDetailsTemp
                .Include(v => v.Vehicle)
                .Where(o => o.User == user)
                .OrderBy(o => o.Vehicle.LicensePlate);
        }
    }
}
