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
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public ClientsController(IClientRepository clientRepository, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Clients
        public IActionResult Index()
        {
            return View(_clientRepository.GetAll().OrderBy(p => p.FirstName)); // trás todos os veículos, ordenados pela marca
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id) // pode aceitar null
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var client = await _clientRepository.GetByIdAsync(id.Value); // tem que ser id.value para que se for null não "rebentar"

            if (client == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(client);
        }

        // GET: Mechanics/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() // Abrir a view do create (aquela janela que aparece assim que carregamos no botao do create new)
        {
            return View();
        }

        // Este Post corresponde ao botão create que aparece em baixo quando acabamos de preencher a informacao do novo veículo
        //Recebe o modelo e envia para a base de dados
        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel model) //Aqui já recebe o objeto
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty; // caminho da imagem

                if (model.ImageFile != null && model.ImageFile.Length > 0) // verificar se tem imagem
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "clients"); // guarda o ficheiro na pasta products
                }

                // coverte de product para view model
                var client = _converterHelper.ToClient(model, path, true); // é true porque é novo (create)

                //TODO : Modificar para o user que tiver logado
                client.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"
                await _clientRepository.CreateAsync(client); // recebe o veículo
                return RedirectToAction(nameof(Index)); // redireciona para a action index (mostra a lista de veículos)
            }
            return View(model); // se o veículo não passar nas validações mostra a view e deixa ficar lá o veículo,
                                // para o utilizador não ter que preencher tudo de novo
        }

        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var client = await _clientRepository.GetByIdAsync(id.Value); // coloca o id em memória e verifica caso o id tenha sido eliminado entretanto
            if (client == null)                            // tem que ser id.value para que se for null não "rebentar"
            {
                return new NotFoundViewResult("ProductNotFound");

            }

            var model = _converterHelper.ToClientViewModel(client); //vai á base de dados e converte de product para um product view model

            return View(model); // retorna a view e manda o veículo lá para dentro
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "clients");
                    }

                    var client = _converterHelper.ToClient(model, path, false); // o bool é false porque não é novo (edit)

                    //TODO : Modificar para o user que tiver logado
                    client.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // dá o utilizador que estiver "logado"
                    await _clientRepository.UpdateAsync(client); // faz o update do veículo
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _clientRepository.ExistAsync(model.Id)) // verifica se o id existe devido a alguem entretanto ter apagado este veículo
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

        // GET: Clients/Delete/5 // Só mostra o que for para apagar. Não apaga
        public async Task<IActionResult> Delete(int? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client == null)
            {
                return new NotFoundViewResult("ProductNotFound"); // passo a minha view ; genérico dá para produtos, clientes, fornecedores, etc
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // o id é obrigatório
        {
            var client = await _clientRepository.GetByIdAsync(id); ; // o id é verficado para ver se ainda existe
            await _clientRepository.DeleteAsync(client); //remover em memória
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ProductNotFound()
        {
            return View();
        }
    }
}