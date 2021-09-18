using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjetoOficinaWeb.Models
{
    public class AddItemViewModel // só para a view
    {
        [Display(Name = "Vehicle")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a vehicle.")] // na combobox o utilizador tem que selecionar um
        // O Range 1 serve para que aceite apenas a "segunda" opcao da combobox sendo a primeira o select a product
        public int VehicleId { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "The quantity must be a positive number.")]
        public double Quantity { get; set; } // quantos produtos o utilizador quer

        public IEnumerable<SelectListItem> Vehicles { get; set; }  // a lista com os produtos todos
    }
}
