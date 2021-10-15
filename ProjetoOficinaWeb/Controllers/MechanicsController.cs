﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;

        public MechanicsController(UserManager<User> userManager, IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper, IMailHelper mailHelper, IConfiguration configuration)
        {
            _userManager = userManager;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
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
                model.ImageUrl = user.ImageUrl;
            }
            return View(userRolesViewModel);
        }

        public IActionResult RegisterMechanic()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> RegisterMechanic(RegisterNewUserMechanicViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    var path = string.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Mechanics");
                    }
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.Email,
                        TaxNumber = model.TaxNumber,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        PostalCode = model.PostalCode,
                        ImageUrl = path
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }
                    await _userHelper.AddUserToRoleAsync(user, "Mechanic");

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken,
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(model.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                    if (response.IsSuccess)
                    {
                        ViewBag.Message = "The instructions to allow you user has been sent to email";
                        return View(model);
                    }

                    ModelState.AddModelError(string.Empty, "The user couldn't be logged.");

                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);

                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {

            }

            return View();

        }

        // GET: Mechanics/Details/5
        public async Task<IActionResult> Details(string id) // pode aceitar null
        {
            if (id == null)
            {
                return new NotFoundViewResult("Error404");
            }

            var mechanic = await _userHelper.GetUserByIdAsync(id);

            if (mechanic == null)
            {
                return new NotFoundViewResult("Error404");
            }

            var model = new RegisterNewUserMechanicViewModel();
            if (mechanic != null)
            {
                model.FirstName = mechanic.FirstName;
                model.LastName = mechanic.LastName;
                model.Address = mechanic.Address;
                model.PhoneNumber = mechanic.PhoneNumber;
                model.Email = mechanic.Email;
                model.PostalCode = mechanic.PostalCode;
                model.TaxNumber = mechanic.TaxNumber;
            }

            return View(mechanic);
        }

        [Authorize(Roles = "Admin")]
        // GET: Mechanics/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            var user = await _userHelper.GetUserByIdAsync(id.ToString());

            var model = new RegisterNewUserMechanicViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;
                model.TaxNumber = user.TaxNumber;
                model.PostalCode = user.PostalCode;
                model.ImageUrl = user.ImageUrl;
            }

            return View(model);
        }

        // POST: Mechanics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegisterNewUserMechanicViewModel model, string id)
        {
            if (!ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "Mechanics");
                }
                else
                {
                    path = model.ImageUrl;
                }

                var user = await _userHelper.GetUserByIdAsync(id.ToString());
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.TaxNumber = model.TaxNumber;
                    user.PostalCode = model.PostalCode;
                    user.ImageUrl = path;

                    var response = await _userHelper.UpdateUserAsync(user);
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

        [Authorize]
        [Authorize(Roles = "Admin")]
        // GET: Mechanics/Delete/5 // Só mostra o que for para apagar. Não apaga
        public async Task<IActionResult> Delete(string? id) // O ? permite que o id seja opcional de forma a que mesmo que o id vá vazio (url) o programa não "rebente"
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _userManager.FindByIdAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(mechanic);
            return RedirectToAction(nameof(Index));
        }

        //// POST: Mechanics/Delete/5
        //[HttpPost, ActionName("Delete")] // quando houver um action chamada "Delete" mas que seja com um Post faz o DeleteConfirmed
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id) // o id é obrigatório
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customer = await _userHelper.GetUserByIdAsync(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    await _userManager.DeleteAsync(customer);
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
