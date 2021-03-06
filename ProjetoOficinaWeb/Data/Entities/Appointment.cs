using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProjetoOficinaWeb.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Apointment date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/DD hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Repair date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/DD hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime RepairDate { get; set; }

        [Required]
        public User User { get; set; } // Ligação de muitos para 1

        public IEnumerable<AppointmentDetail> Items { get; set; } // faz a ligacao com a tabela AppointmentDetails (ligação de 1 para muitos)
                                                                  // Uma lista de OrderDetails ; São as "linhas"

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Lines => Items == null ? 0 : Items.Count();

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity => Items == null ? 0 : Items.Sum(i => i.Quantity); // Vai verificar se a lista é null (coloca 0) senão soma os valores todos


        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Items == null ? 0 : Items.Sum(i => i.Value);

        [Display(Name = "Order date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? AppointmentDateLocal =>
#pragma warning disable CS8073
            this.AppointmentDate == null
#pragma warning restore CS8073
            ? null
           : this.AppointmentDate.ToLocalTime();

    }
}
