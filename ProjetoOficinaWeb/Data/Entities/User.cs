using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProjetoOficinaWeb.Data.Entities
{
    public class User : IdentityUser
    {
        // Além das propiedades por defeito, acrescento as minhas

        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string LastName { get; set; }


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName}{LastName}";
    }
}
