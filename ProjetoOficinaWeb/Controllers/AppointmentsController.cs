using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository, IVehicleRepository vehicleRepository, IServiceRepository serviceRepository)
        {
            _appointmentRepository = appointmentRepository;
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
        }

        // GET: Appointments 
        public async Task<IActionResult> Index()
        {
            var model = await _appointmentRepository.GetAppointmentAsync(this.User.Identity.Name);
            return View(model);
        }

        [Authorize(Roles = "Receptionist,Mechanic")]
        public async Task<IActionResult> Create()
        {
            var model = await _appointmentRepository.GetDetailTempsAsync(this.User.Identity.Name);
            return View(model);
        }

        
        public IActionResult AddVehicle()
        {
            var model = new AddItemViewModel
            {
                Vehicles = _vehicleRepository.GetComboVehicles(),
                Services = _serviceRepository.GetComboServices()
            };

            return View(model);
        }

        [Authorize(Roles = "Receptionist,Mechanic")]
        [HttpPost]
        public async Task<IActionResult> AddVehicle(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _appointmentRepository.AddItemToAppointmentAsync(model, this.User.Identity.Name);
                return RedirectToAction("Create");
            }

            return View(model);
        }

        [Authorize(Roles = "Receptionist,Mechanic")]
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _appointmentRepository.DeleteDetailTempAsync(id.Value);
            return RedirectToAction("Create");
        }

        [Authorize(Roles = "Receptionist,Mechanic")]
        public async Task<IActionResult> Increase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _appointmentRepository.ModifyAppointmentDetailTempQuantityAsync(id.Value, 1);
            return RedirectToAction("Create");
        }

        [Authorize(Roles = "Receptionist,Mechanic")]
        public async Task<IActionResult> Decrease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _appointmentRepository.ModifyAppointmentDetailTempQuantityAsync(id.Value, -1);
            return RedirectToAction("Create");
        }

        [Authorize(Roles = "Receptionist,Mechanic")]
        public async Task<IActionResult> ConfirmOrder()
        {
            var response = await _appointmentRepository.ConfirmAppointmentAsync(this.User.Identity.Name);
            if (response)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }


        public async Task<IActionResult> Repair(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _appointmentRepository.GetAppointmentAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            var model = new RepairViewModel
            {
                Id = order.Id,
                RepairDate = DateTime.Today
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Repair(RepairViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _appointmentRepository.RepairOrder(model);
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
