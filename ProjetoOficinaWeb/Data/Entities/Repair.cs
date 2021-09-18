using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Data.Entities
{
    public class Repair : IEntity
    {
        public int Id { get; set; }

        public Vehicle x { get; set; }

        public Client c { get; set; }

        public Appointment a { get; set; }

        public Mechanic m { get; set; }
    }
}
