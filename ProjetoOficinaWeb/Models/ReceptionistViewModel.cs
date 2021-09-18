using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Models
{
    public class ReceptionistViewModel : Receptionist
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
