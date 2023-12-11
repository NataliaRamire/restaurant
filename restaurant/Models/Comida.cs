using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace restaurant.Models
{
    public class Comida
    {
        public int IdComida { set; get; }
        public string NombreComida { set; get; }
        public string DescripcioComida { set; get; }
        public double ValoracionComida { set; get; }
        public string OrigenComida { set; get; }
        public string UbicacionImagen { set; get; }


        public int PrecioComida { set; get; }
        public List<Ingrediente> ingredientes { set; get; }
        public List<Pedido> pedidoscomida { set; get; }
    }
}