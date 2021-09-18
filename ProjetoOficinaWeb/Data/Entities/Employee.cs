using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Data.Entities
{
    public abstract class Employee
    {
        public int Id { get; set; }


        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string Adress { get; set; }


        public int PhoneNumber { get; set; }


        public string Email { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName}{LastName}";

        [Display(Name = "Image")] // Na página da web vai aparecer Image e não ImageUrl como nome do campo
        public string ImageUrl { get; set; } // Link da imagem dos produtos

        public User User { get; set; } // cria uma relação de 1 para muitos (um user pode ter vários produtos)
                                       // quem foi o user que colocou aquele produto

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:44342{ImageUrl.Substring(1)}"; // quando tiver no azure colocar o endereço completo
            }
        }
    }
}
