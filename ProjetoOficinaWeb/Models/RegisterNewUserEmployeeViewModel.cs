using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoOficinaWeb.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Models
{
    public class RegisterNewUserEmployeeViewModel : RegisterNewUserViewModel
    {
        [Display(Name = "Roles")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a role.")] // na combobox o utilizador tem que selecionar um
        // O Range 1 serve para que aceite apenas a "segunda" opcao da combobox sendo a primeira o select a product
        public int VehicleId { get; set; }

        public IEnumerable<User> Roles { get; set; }  // a lista com os produtos todos
    }
}
