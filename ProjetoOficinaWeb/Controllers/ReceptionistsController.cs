using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Controllers
{
    public class ReceptionistsController : Controller
    {
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public ReceptionistsController(IReceptionistRepository receptionistRepository, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _receptionistRepository = receptionistRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Mechanics
        public IActionResult Index()
        {
            return View(_receptionistRepository.GetAll().OrderBy(p => p.FirstName)); // trás todos os veículos, ordenados pela marca
        }

        // GET: Mechanics/Details/5
        public async Task<IActionResult> Details(int? id) // pode aceitar null
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var receptionist = await _receptionistRepository.GetByIdAsync(id.Value); // tem que ser id.value para que se for null não "rebentar"

            if (receptionist == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(receptionist);
        }

        // GET: Mechanics/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() // Abrir a view do create (aquela janela que aparece assim que carregamos no botao do create new)
        {
            return View();
        }

        // Este Post corresponde ao botão create que aparece em baixo quando acabamos de preencher a informacao do novo veículo
        //Recebe o modelo e envia para a base de dados
        // POST: Receptionist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceptionistViewModel model) //Aqui já recebe o objeto
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty; // caminho da imagem

                if (model.ImageFile != null && model.ImageFile.Length > 0) // verificar se tem imagem
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "receptionists"); // guarda o ficheiro na pasta products
                }

                // coverte de product para view model
                var receptionist = _converterHelper.ToReceptionist(model, path, true); // é true porque é novo (create)

                //TODO : Modificar para o user que tiver logado
                receptionist.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"
                await _receptionistRepository.CreateAsync(receptionist); // recebe o veículo
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista de veículos)
            }
            return View(model); // se o veículo não passar nas validações mostra a view e deixa ficar lá o veículo,
                                // para o utilizador não ter que preencher tudo de novo
        }

        // GET: Receptionist/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var receptionist = await _receptionistRepository.GetByIdAsync(id.Value); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (receptionist == null)                            // tem que ser id.value para que se for null não "rebentar"
            {
                return new NotFoundViewResult("ProductNotFound");

            }

            var model = _converterHelper.ToReceptionistViewModel(receptionist); //vai á base de dados e converte de product para um product view model

            return View(model); // retorna a view e manda o veículo lá para dentro
        }

        // POST: Receptionist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReceptionistViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "receptionists");
                    }

                    var receptionist = _converterHelper.ToReceptionist(model, path, false); // o bool é false porque não é novo (edit)

                    //TODO : Modificar para o user que tiver logado
                    receptionist.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"
                    await _receptionistRepository.UpdateAsync(receptionist); // faz o update do veículo
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _receptionistRepository.ExistAsync(model.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este veículo
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
            return View(model);
        }

        // GET: Receptionist/Delete/5 // Só mostra o que for para apagar. Não apaga
        public async Task<IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var receptionist = await _receptionistRepository.GetByIdAsync(id.Value);
            if (receptionist == null)
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            return View(receptionist);
        }

        // POST: Receptionist/Delete/5
        [HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // o id é obrigatório
        {
            var receptionist = await _receptionistRepository.GetByIdAsync(id); ; // o id é verficado para ver se ainda existe
            await _receptionistRepository.DeleteAsync(receptionist); //remover em memória
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ProductNotFound()
        {
            return View();
        }
    }
}
