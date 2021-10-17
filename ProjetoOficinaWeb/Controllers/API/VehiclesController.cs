using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoOficinaWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehiclesController(IVehicleRepository vehicleRepository)
        {
           _vehicleRepository = vehicleRepository;
        }

        [HttpGet]
        public IActionResult GetVehicles()
        {
            return Ok(_vehicleRepository.GetAll());
        }
    }
}
