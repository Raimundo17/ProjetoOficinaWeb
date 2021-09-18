using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository, IVehicleRepository vehicleRepository)
        {
            _appointmentRepository = appointmentRepository;
            _vehicleRepository = vehicleRepository;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var model = await _appointmentRepository.GetAppointmentAsync(this.User.Identity.Name);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _appointmentRepository.GetDetailTempsAsync(this.User.Identity.Name);
            return View(model);
        }

        public IActionResult AddVehicle()
        {
            var model = new AddItemViewModel
            {
                Quantity = 1,
                Vehicles = _vehicleRepository.GetComboVehicles()
            };

            return View(model);
        }
    }
}
