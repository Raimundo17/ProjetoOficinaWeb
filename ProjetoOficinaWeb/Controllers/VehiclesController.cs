using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Helpers;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public VehiclesController(IVehicleRepository vehicleRepository, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _vehicleRepository = vehicleRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
           return View (_vehicleRepository.GetAll().OrderBy(p => p.Brand)); // trás todos os veículos, ordenados pela marca

            //var model = await _vehicleRepository.GetVehicleAsync(this.User.Identity.Name); // Cada utilizador só vê o seu veículo
            //return View(model);
        }                                                                  


        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id) // pode aceitar null
        {
            if (id == null)
            {
                return new NotFoundViewResult("Error404"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(id.Value); // tem que ser id.value para que se for null não "rebentar"

            if (vehicle == null)
            {
                return new NotFoundViewResult("Error404");
            }

            return View(vehicle);
        }

        [Authorize(Roles = "Receptionist, Mechanic")]
        // GET: Vehicles/Create
        public IActionResult Create() // Abrir a view do create (aquela janela que aparece assim que carregamos no botao do create new)
        {
            //return View();

            var model = new VehicleViewModel
            {
                Users = _userHelper.GetComboUsers()
            };

            return View(model);
        }

        // Este Post corresponde ao botão create que aparece em baixo quando acabamos de preencher a informacao do novo veículo, Recebe o modelo e envia para a base de dados
        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Receptionist, Mechanic")]
        public async Task<IActionResult> Create(VehicleViewModel modell) //Aqui já recebe o objeto
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty; // caminho da imagem

                if (modell.ImageFile != null && modell.ImageFile.Length > 0) // verificar se tem imagem
                {
                    path = await _imageHelper.UploadImageAsync(modell.ImageFile, "vehicles"); // guarda o ficheiro na pasta products
                }

                // coverte de product para view model
                var vehicle = _converterHelper.ToVehicle(modell, path, true); // é true porque é novo (create)

                //TODO : Modificar para o user que tiver logado
                //vehicle.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"

                var model = new VehicleViewModel
                {
                    Users = _userHelper.GetComboUsers(),
                };

                await _vehicleRepository.CreateAsync(vehicle); // recebe o veículo
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista de veículos)
            }
            return View(modell); // se o veículo não passar nas validações mostra a view e deixa ficar lá o veículo,
                                 // para o utilizador não ter que preencher tudo de novo

            //if(ModelState.IsValid)
            //{
            //    var vehicle = _converterHelper.ToVehicle(modell, true);
            //    vehicle.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            //    await _vehicleRepository.CreateAsync(vehicle);
            //    return RedirectToAction(nameof(Index));
            //}

            //return View(modell);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                return new NotFoundViewResult("Error404"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(id.Value); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (vehicle == null)                            // tem que ser id.value para que se for null não "rebentar"
            {
                return new NotFoundViewResult("Error404");

            }

            var model = _converterHelper.ToVehicleViewModel(vehicle); //vai á base de dados e converte de product para um product view model

            return View(model); // retorna a view e manda o veículo lá para dentro
        }


        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleViewModel modell)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var path = modell.ImageUrl;

                    if (modell.ImageFile != null && modell.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(modell.ImageFile, "vehicles");
                    }

                    var vehicle = _converterHelper.ToVehicle(modell, path, false); // o bool é false porque não é novo (edit)

                    //TODO : Modificar para o user que tiver logado
                    //vehicle.UserId = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"
                    await _vehicleRepository.UpdateAsync(vehicle); // faz o update do veículo
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _vehicleRepository.ExistAsync(modell.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este veículo
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(modell);
        }

        // GET: Vehicles/Delete/5 // Só mostra o que for para apagar. Não apaga
        [Authorize(Roles = "Receptionist, Mechanic")]
        public async Task<IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return new NotFoundViewResult("Error404"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(id.Value);
            if (vehicle == null)
            {
                return new NotFoundViewResult("Error404"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [Authorize(Roles = "Receptionist, Mechanic")]
        [HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // o id é obrigatório
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id); ; // o id é verficado para ver se ainda existe
            
            try
            {
                await _vehicleRepository.DeleteAsync(vehicle); //remover em memória
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{vehicle.LicensePlate} is being used!!";
                    ViewBag.ErrorMessage = $"{vehicle.LicensePlate} it´s not possible to delete because there are appointments with this vehicle.</br></br>";
                }

                return View("Error");
            }
        }

        public IActionResult Error404()
        {
            return View();
        }
    }
}
