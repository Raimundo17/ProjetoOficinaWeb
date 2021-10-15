using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email); // dou o email e dá uma string (Bypass)

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model); // verifica se o utilizador entrou ou não

        Task LogoutAsync();


        Task CheckRoleAsync(string roleName); // verifica se tem um determinado role se nao tiver cria

        Task AddUserToRoleAsync(User user, string roleName); // adiciona um role a um determinado user

        Task<bool> IsUserInRoleAsync(User user, string roleName); // verifica se o user já tem este role ou nao


        Task<SignInResult> ValidatePasswordAsync(User user, string password);


        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User> GetUserByIdAsync(string userId); // dou o email e dá uma string (Bypass)


        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);


        Task<IdentityResult> UpdateUserAsync(User user); // muda o primeiro e o último nome

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<User> GetUserAsync(string id);

        IEnumerable<SelectListItem> GetComboUsers();
    }
}
