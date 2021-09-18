using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Models
{
    public class MechanicViewModel : Mechanic
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
