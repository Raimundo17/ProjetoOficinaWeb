using Microsoft.AspNetCore.Mvc;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Helpers;
using ProjetoOficinaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Controllers
{
    public class RepairsController : Controller
    {
        private readonly IRepairRepository _repairRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public RepairsController(IRepairRepository repairRepository, IConverterHelper converterHelper, IUserHelper userHelper)
        {
            _repairRepository = repairRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new RepairViewModel
            {
                Vehicles = _repairRepository.GetComboVehicles(),
                Services = _repairRepository.GetComboServices(),
                Appointments = _repairRepository.GetComboAppointments(),
                Users = _repairRepository.GetComboMechanics(),
            };

            return View(model);
        }

        // POST: RepairController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RepairViewModel model)
        {
            if (ModelState.IsValid)
            {

                var repair = new RepairViewModel
                {
                    Vehicles = _repairRepository.GetComboVehicles(),
                    Services = _repairRepository.GetComboServices(),
                    Appointments = _repairRepository.GetComboAppointments(),
                    Users = _repairRepository.GetComboMechanics(),
                };

                await _repairRepository.CreateAsync(repair); // recebe o veículo
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista de veículos)
            }
            return View(model);
        }
    }
}
