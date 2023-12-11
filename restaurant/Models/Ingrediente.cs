using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace restaurant.Models
{
    public class Ingrediente
    {
        public string NombreIngrediente { get; set; }
        public string Nutrientes { get; set; }
        public int IdIngrediente { set; get; }
        public List<Comida> comidassas { get; set; }
    }
}