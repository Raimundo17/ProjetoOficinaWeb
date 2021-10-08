using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Controllers
{
    public class MechanicsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public MechanicsController(UserManager<User> userManager, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _userManager = userManager;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Mechanics
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("Mechanic");
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (User user in users)
            {
                var model = new UserRolesViewModel();
                model.Id = user.Id;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PostalCode = user.PostalCode;
                model.TaxNumber = user.TaxNumber;
                model.PhoneNumber = user.PhoneNumber;
                model.Email = user.Email;
                userRolesViewModel.Add(model);
            }
            return View(userRolesViewModel);
        }

        // GET: Mechanics/Details/5
        public async Task<IActionResult> Details(string id) // pode aceitar null
        {
           if(id == null)
            {
                return new NotFoundViewResult("Error404");
            }

            var mech = await _userHelper.GetUserByIdAsync(id);

            if(mech == null)
            {
                return new NotFoundViewResult("Error404");
            }

            var model = new RegisterNewUserMechanicViewModel();
            if(mech != null)
            {
                model.FirstName = mech.FirstName;
                model.LastName = mech.LastName;
                model.Address = mech.Address;
                model.PhoneNumber = mech.PhoneNumber;
                model.Username = mech.UserName;
                model.PostalCode = mech.PostalCode;
                model.TaxNumber = mech.TaxNumber;
                model.Username = mech.UserName;
            }

            return View(mech);

        }

        // GET: Mechanics/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var mech = await _userHelper.GetUserByIdAsync(id);
            var model = new ChangeUserViewModel();
            if (mech != null)
            {
                model.FirstName = mech.FirstName;
                model.LastName = mech.LastName;
                model.Address = mech.Address;
                model.PostalCode = mech.PostalCode;
                model.TaxNumber = mech.TaxNumber;
                //model.PhoneNumber = mech.PhoneNumber;
            }

            return View(model);

        }

        // POST: Mechanics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mech = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (mech != null)
                {
                    mech.FirstName = model.FirstName;
                    mech.LastName = model.LastName;
                    mech.Address = model.Address;
                    mech.PostalCode = model.PostalCode;
                    mech.TaxNumber = model.TaxNumber;
                    //mech.PhoneNumber = model.PhoneNumber;
                    
                    var response = await _userHelper.UpdateUserAsync(mech);
                    
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }
            return View(model);
        }

        // GET: Mechanics/Delete/5 // Só mostra o que for para apagar. Não apaga
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            var mech = await _userHelper.GetUserByIdAsync(id);
            var model = new ChangeUserViewModel();
            if (mech != null)
            {
                model.FirstName = mech.FirstName;
                model.LastName = mech.LastName;
                model.Address = mech.Address;
                model.PostalCode = mech.PostalCode;
                model.TaxNumber = mech.TaxNumber;
            }

            return View(model);
        }

        // POST: Mechanics/Delete/5
        [HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id) // o id é obrigatório
        {
            if (id == null)
            {
                return NotFound();
            }

            var mech = await _userHelper.GetUserByIdAsync(id);
            if (mech == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(mech);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ProductNotFound()
        {
            return View();
        }
    }

}
