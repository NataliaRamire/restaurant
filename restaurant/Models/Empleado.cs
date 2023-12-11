using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace restaurant.Models
{
    public class Empleado
    {
        public string documento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Celular { get; set; }
        public string Correo { get; set; }
        public string Cargo { get; set; }
        public string Salario { get; set; }
        public
          List<Pedido> pedidoempleado
        { get; set; }
    }
}