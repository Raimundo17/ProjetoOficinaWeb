using Microsoft.AspNetCore.Identity;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email); // dou o email e dá uma string (Bypass)

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model); // verifica se o utilizador entrou ou não

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user); // muda o primeiro e o último nome

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName); // verifica se tem um determinado role se nao tiver cria

        Task AddUserToRoleAsync(User user, string roleName); // adiciona um role a um determinado user

        Task<bool> IsUserInRoleAsync(User user, string roleName); // verifica se o user já tem este role ou nao
    }
}
