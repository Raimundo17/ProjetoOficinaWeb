using System.ComponentModel.DataAnnotations;

namespace ProjetoOficinaWeb.Data.Entities
{
    public class AppointmentDetailTemp : IEntity
    {
        public int Id { get; set; }

        [Required]
        public User User { get; set; } // na prática é o sistema que coloca o user (login) 

        [Required]
        public Vehicle Vehicle { get; set; }

        [Required]
        public Service Service { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        public decimal Value => Price * (decimal)Quantity; // get "moderno"
    }
}
