﻿using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;
using ProjetoOficinaWeb.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ProjetoOficinaWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;

        public AccountController(IUserHelper userHelper, IImageHelper imageHelper,IMailHelper mailHelper,IConfiguration configuration)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
        }


        public IActionResult Login()
        {

            //if (User.Identity.IsAuthenticated && this.User.IsInRole("Admin")) // se o utilizador quando fizer login já estiver autenticado
            //{
            //    return this.RedirectToAction("IndexDashboard", "Home"); // DASHBOARD
            //}

            //if (User.Identity.IsAuthenticated) // se o utilizador quando fizer login já estiver autenticado
            //{
            //    return this.RedirectToAction("Index", "Home"); // primeiro a action depois o controlador home
            //}

            //return View(); // se acontecer alguma coisa de errado fica na mesma view

            if (User.Identity.IsAuthenticated && this.User.IsInRole("Admin"))
            {
                return this.RedirectToAction("IndexDashboard", "Home");
            }
            else if (User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //if (ModelState.IsValid) // se estiver tudo preenchido(email, password requerido pelos data notations)
            //{
            //    var result = await _userHelper.LoginAsync(model);
            //    if (result.Succeeded)
            //    { // se na query(url) aparecer um ReturnUrl ele reencaminha o utilizador depois de fazer login para a página onde ia inicialmente
            //        if (this.Request.Query.Keys.Contains("ReturnUrl"))
            //        {
            //            return Redirect(this.Request.Query["ReturnUrl"].First()); // o primeiro returnUrl que encontrar
            //        }

            //        if(User.Identity.IsAuthenticated && this.User.IsInRole("Admin"))
            //        {
            //            return this.RedirectToAction("IndexDashboard", "Home"); // DASHBOARD
            //        }

            //        return this.RedirectToAction("Index", "Home");
            //    }
            //}
            //this.ModelState.AddModelError(string.Empty, "Failed to login");
            //return View(model);

            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {

                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }
                    return this.RedirectToAction("Index", "Home");


                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login!");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    var path = string.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Clients");
                    }
                    user = new User
                    {
                        Id = model.Id,
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
                    await _userHelper.AddUserToRoleAsync(user, "customer");

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken

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

        //-----------------------------RECOVER PASSWORD W/TOKEN
        public IActionResult RecoverPassword()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        //{
        //    if (this.ModelState.IsValid)
        //    {
        //        var user = await _userHelper.GetUserByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
        //            return View(model);
        //        }

        //        var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

        //        var link = this.Url.Action(
        //            "ResetPassword",
        //            "User",
        //            new { token = myToken }, protocol: HttpContext.Request.Scheme);

        //        Response response = _mailHelper.SendEmail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" +
        //        $"To reset the password click in this link:</br></br>" +
        //        $"<a href = \"{link}\">Reset Password</a>");

        //        if (response.IsSuccess)
        //        {
        //            this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
        //        }

        //        return this.View();

        //    }

        //    return this.View(model);
        //}

        public IActionResult RegisterMechanic()
        {
            return View();
        }

        [HttpPost]
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
                        Id = model.Id,
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
                        token = myToken

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

        public IActionResult RegisterReceptionist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterReceptionist(RegisterNewUserReceptionistViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    var path = string.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Receptionists");
                    }
                    user = new User
                    {
                        Id = model.Id,
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
                    await _userHelper.AddUserToRoleAsync(user, "Receptionist");

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken

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

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PostalCode = user.PostalCode;
               // model.PhoneNumber = user.PhoneNumber.ToString();
                model.TaxNumber = user.TaxNumber;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PostalCode = model.PostalCode;
                    user.TaxNumber = model.TaxNumber;
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

        public IActionResult ChangePassword()
        {
            return View(); // só se passa a view para as caixas estarem vazias, nao queremos preencher as caixas com as passwords
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            return View(); // em vez de passar pelo middleware todas as páginas de nao autorizado passaram por aqui (em vez de mandar para o login)
        }
    }
}
