using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoOficinaWeb.Data
{
    public class ReceptionistRepository : GenericRepository<Receptionist>, IReceptionistRepository
    {
        private readonly DataContext _context;

        public ReceptionistRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Receptionists.Include(p => p.User); // como se fosse um join do SQL
        }

        public IEnumerable<SelectListItem> GetComboReceptionists()
        {
            var list = _context.Receptionists.Select(p => new SelectListItem // como se fosse um foreach
            {
                Text = p.FullName, // texto que vai aparecer na combobox
                Value = p.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Receptionist...)",
                Value = "0"
            });

            return list;
        }
    }
}
