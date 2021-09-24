using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;
using ProjetoOficinaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Data
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository // Segundo o dependency injection tenho de especificar o interface antes da classe
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public AppointmentRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if(user == null)
            {
                return;
            }

            var vehicle = await _context.vehicles.FindAsync(model.VehicleId);
            if (vehicle == null)
            {
                return;
            }

            var service = await _context.Services.FindAsync(model.ServiceId);
            if (service == null)
            {
                return;
            }

            var appointmentDetailTemp = await _context.AppointmentDetailsTemp
                .Where(adt => adt.User == user && adt.Vehicle == vehicle && adt.Service == service)
                .FirstOrDefaultAsync();

            if(appointmentDetailTemp == null)
            {
                appointmentDetailTemp = new AppointmentDetailTemp
                {
                    Price = service.Price,
                    Vehicle = vehicle,
                    Service = service,
                    User = user,
                };

                _context.AppointmentDetailsTemp.Add(appointmentDetailTemp);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var appointmentDetailTemp = await _context.AppointmentDetailsTemp.FindAsync(id);
            if(appointmentDetailTemp ==  null)
            {
                return;
            }

            _context.AppointmentDetailsTemp.Remove(appointmentDetailTemp);
            await _context.SaveChangesAsync();
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
                    .Include(o => o.RepairEntries) // ir buscar dados entre tabelas ligadas diretamente (como se fosse um join)
                    .ThenInclude(i => i.Vehicle) // ir buscar dados entre tabelas onde estejam ligadas por uma tabela a meio
                    .OrderByDescending(o => o.RepairDate);
            }

            return _context.Appointments // cada utilizador vê só os seus orders
                .Include(o => o.RepairEntries)
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
                .Include(s => s.Service)
                .Where(o => o.User == user)
                .OrderBy(o => o.Vehicle.LicensePlate);
        }
    }
}
