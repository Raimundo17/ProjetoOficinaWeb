using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProjetoOficinaWeb.Data.Entities;

namespace ProjetoOficinaWeb.Models
{
    public class VehicleViewModel : Vehicle
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
