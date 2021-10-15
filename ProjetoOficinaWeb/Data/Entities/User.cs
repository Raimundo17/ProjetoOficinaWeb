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

        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }

       // [MaxLength(9, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public int TaxNumber { get; set; }

        public string PostalCode { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        public string ImageUrl { get; set; } // Link das imagens

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:44310{ImageUrl.Substring(1)}"; // quando tiver no azure colocar o endereço completo
            }
        }
    }
}
