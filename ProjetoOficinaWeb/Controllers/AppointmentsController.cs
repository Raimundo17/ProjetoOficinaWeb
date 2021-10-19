using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Helpers;
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
            //var model = await _appointmentRepository.GetAppointmentAsync(this.User.Identity.Name);
            var model =  _appointmentRepository.GetAll();
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
                //RepairDate = DateTime.Today
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

        // GET: Appointments/Delete/5 // Só mostra o que for para apagar. Não apaga
        [Authorize(Roles = "Receptionist, Mechanic")]
        public async Task<IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return new NotFoundViewResult("Error404"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);
            if (appointment == null)
            {
                return new NotFoundViewResult("Error404"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [Authorize(Roles = "Receptionist, Mechanic")]
        [HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // o id é obrigatório
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id); ; // o id é verficado para ver se ainda existe

            try
            {
                await _appointmentRepository.DeleteAsync(appointment); //remover em memória
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{appointment.AppointmentDate} is being used!!";
                    ViewBag.ErrorMessage = $"{appointment.AppointmentDate} it´s not possible to delete</br></br>";
                }

                return View("Error");
            }
        }
    }
}
