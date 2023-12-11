using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace restaurant.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int PrecioPedido { get; set; }
        public string FechaPedido { get; set; }
        public string EstadoPedido { get; set; }
        public List<Comida> comida { get; set; }
    }
}