using System.ComponentModel.DataAnnotations;

namespace ProjetoOficinaWeb.Data.Entities
{
    public class Vehicle : IEntity // Entidade que vai dar origem a uma tabela na base de dados
    {
        public int Id { get; set; } // como é Id automaticamente fica como chave primária

        [Required]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Color { get; set; }

        [Display(Name = "Image")] // Na página da web vai aparecer Image e não ImageUrl como nome do campo
        public string ImageUrl { get; set; } // Link da imagem dos produtos

        public int? Year { get; set; } // ? -> o required field validator desaparece

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

                return $"https://localhost:44310{ImageUrl.Substring(1)}"; // quando tiver no azure colocar o endereço completo
            }
        }
    }
}
