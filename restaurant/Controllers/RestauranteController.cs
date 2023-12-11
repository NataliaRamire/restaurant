using restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using VDS.RDF.Query;

namespace restaurant.Controllers
{
    public class RestauranteController : Controller
    {
        private static SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://localhost:3030/restaurante/sparql"));
        // GET: Restaurante
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Empleados()
        {
            List<Empleado> listaEmpleados = new List<Empleado>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
                "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>" +
                "PREFIX bd: <http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#>" +
                "SELECT  (REPLACE(STR(?documento), '^\" |\"$', '') AS ?DOCUMENTO)" +
                "(CONCAT(?nombre, \" \", ?Apellido) AS ?NOMBRECOMPLETO) " +
                "?CORREO " +
                "?CARGO " +
                "(REPLACE(STR(?salario), '^\" |\"$', '') AS ?SALARIO) " +
                "WHERE {  ?clientes rdf:type bd:Empleado." +
                "?clientes bd:Documento ?documento ." +
                "?clientes bd:Nombres ?nombre ." +
                "?clientes bd:Apellidos ?Apellido ." +
                "?clientes bd:Correo ?CORREO ." +
                "?clientes bd:Cargo ?CARGO." +
                "?clientes bd:Salario ?salario." +
                "}"
                );

            foreach (var result in resultado.Results)
            {
                Empleado empleado = new Empleado();
                empleado.documento = result["DOCUMENTO"].ToString();
                empleado.Nombres = result["NOMBRECOMPLETO"].ToString();
                empleado.Correo = result["CORREO"].ToString();
                empleado.Cargo = result["CARGO"].ToString();
                empleado.Salario = result["SALARIO"].ToString();

                listaEmpleados.Add(empleado);
            }


            return View(listaEmpleados);
        }
        public ActionResult Inicio()
        {
            return View();
        }

        public ActionResult PedidoComida()
        {
            List<Cliente> clientes = new List<Cliente>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
                "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                "PREFIX bd:<http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#> " +
                "SELECT (CONCAT(?NOMBRE, \" \", ?APELLIDO) AS ?NOMBRE_COMPLETO) " +
                "?ID_PEDIDO " +
                "?PRECIO " +
                "?NOMBRE_COMIDA " +
                "WHERE { " +
                "?cliente rdf:type bd:Cliente .  " +
                "?cliente bd:realizapedido ?pedido .  " +
                "?pedido bd:contienecomida ?comida .   " +
                "?cliente bd:Nombres ?NOMBRE . " +
                "?cliente bd:Apellidos ?APELLIDO . " +
                "?pedido bd:IdPedido ?IDPEDIDO .  " +
                "BIND(STR(?IDPEDIDO) AS ?ID_PEDIDO). " +
                "?pedido bd:PrecioPedido ?precio . " +
                "BIND(STR(?precio) AS ?PRECIO). " +
                "?comida bd:NombreComida ?NOMBRE_COMIDA . " +
                "}");


            foreach (var result in resultado.Results)
            {
                Cliente cliente = new Cliente
                {
                    Nombres = result["NOMBRE_COMPLETO"].ToString(),
                    pedidos = new List<Pedido>()
                };

                Pedido pedido = new Pedido
                {
                    IdPedido = int.Parse(result["ID_PEDIDO"].ToString()),
                    PrecioPedido = int.Parse(result["PRECIO"].ToString()),
                    comida = new List<Comida>()
                };

                Comida comida = new Comida
                {
                    NombreComida = result["NOMBRE_COMIDA"].ToString()
                };

                pedido.comida.Add(comida); // Agrega la comida al pedido
                cliente.pedidos.Add(pedido); // Agrega el pedido a la lista de pedidos del cliente
                clientes.Add(cliente); // Agrega el cliente a la lista de clientes
            }



            return View(clientes);
        }

        public ActionResult IngredienteComida(int identificador)
        {
            if (identificador == null)
            {
                return RedirectToAction("CatalogoIngredientes", "Restaurante");
            }

            List<Comida> comidas = new List<Comida>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
            "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
            "PREFIX bd:<http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#> " +
            "SELECT ?idComida ?NOMBRE ?Descripcion_comida ?Calificacion ?Origencomida ?Precio ?UbicacionImag " +
            "WHERE { " +
            "?comida rdf:type bd:Comida. " +
            "?comida bd:IdComida ?id_comida. " +
            "BIND(STR(?id_comida) AS ?idComida)." +
            "?comida bd:Descripcion ?Descripcion_comida. " +
            "?comida bd:Valoracion ?valora. " +
            "BIND(STR(?valora) AS ?Calificacion). " +
            "?comida bd:Orígen ?Origencomida. " +
            "?comida bd:PrecioComida ?PRECIO. " +
            "BIND(STR(?PRECIO) AS ?Precio). " +
            "?comida bd:UbicacionImagen ?UbicacionImag. " +
            "?comida bd:compuestapor ?ingrediente . " +
            "?ingrediente bd:NombreIngrediente ?nombreIngrediente . " +
            "?ingrediente bd:IdIngrediente ?idIngrediente . " +
            "BIND(STR(?idIngrediente) AS ?Id_Ingrediente). " +
            "FILTER(?idIngrediente = "+ identificador +") . " +
            "?comida bd:NombreComida ?NOMBRE . " +
            "}");

            foreach (var result in resultado.Results)
            {
                Comida comida = new Comida();
                var dato = result.ToList();
                comida.IdComida = int.Parse(dato[0].Value.ToString());
                comida.NombreComida = dato[1].Value.ToString();
                comida.DescripcioComida = dato[2].Value.ToString();
                comida.ValoracionComida = double.Parse(dato[3].Value.ToString());
                comida.OrigenComida = dato[4].Value.ToString();
                int variable = 0;
                // Opción 2: Utilizando Double.TryParse()
                if (int.TryParse(dato[5].Value.ToString(), out variable))
                {
                    comida.PrecioComida = variable;
                }
                else
                {
                    comida.PrecioComida = 300000;
                }
                comida.UbicacionImagen = dato[6].Value.ToString();

                comidas.Add(comida);

            }

            return View(comidas);
        }
        public ActionResult CatalogoComida()
        {
            List<Comida> listav = new List<Comida>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
            "PREFIX rdf:<http://www.w3.org/1999/02/22-rdf-syntax-ns#>" +
            "PREFIX bd:<http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#>" +
            "SELECT ?id_comida ?NOMBRE ?Descripcion_comida ?Calificacion ?Origencomida ?PRECIO ?UbicacionImag " +
            "WHERE {" +
            "?comida rdf:type bd:Comida." +
            "?comida bd:IdComida ?idcomida." +
            "BIND(STR(?idcomida) AS ?id_comida)" +
            "?comida bd:NombreComida ?NOMBRE." +
            "?comida bd:Descripcion ?Descripcion_comida." +
            "?comida bd:Valoracion ?valora." +
            "BIND(STR(?valora) AS ?Calificacion)." +
            "?comida bd:Orígen ?Origencomida." +
            "?comida bd:PrecioComida ?PRECIORA." +
            "BIND(STR(?PRECIORA) AS ?PRECIO). " +
            "?comida bd:UbicacionImagen  ?UbicacionImag" +
            "} "
                );
            foreach (var result in resultado.Results)
            {
                Comida comida = new Comida();
                var dato = result.ToList();
                comida.IdComida = int.Parse(dato[0].Value.ToString());
                comida.NombreComida = dato[1].Value.ToString();
                comida.DescripcioComida = dato[2].Value.ToString();
                comida.ValoracionComida = double.Parse(dato[3].Value.ToString());
                comida.OrigenComida = dato[4].Value.ToString();
                int variable = 0;
                // Opción 2: Utilizando Double.TryParse()
                if (int.TryParse(dato[5].Value.ToString(), out variable))
                {
                    comida.PrecioComida = variable;
                }
                else
                {
                    comida.PrecioComida = 300000;
                }
                comida.UbicacionImagen = dato[6].Value.ToString();

                listav.Add(comida);

            }

            return View(listav);


        }

        public ActionResult CatalogoIngredientes()
        {
            List<Ingrediente> lista = new List<Ingrediente>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
            "PREFIX rdf:<http://www.w3.org/1999/02/22-rdf-syntax-ns#>" +
            "PREFIX bd:<http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#>" +
            "SELECT ?id_ingrediente ?nombreIngrediente " +
            "WHERE {" +
            "?ingrediente rdf:type bd:Ingrediente ." +
            "?ingrediente bd:IdIngrediente ?idIngrediente." +
            "?ingrediente bd:NombreIngrediente ?nombreIngrediente." +
            "BIND(STR(?idIngrediente) AS ?id_ingrediente) ." +
            "} ORDER BY ASC(?id_ingrediente)");

            foreach (var result in resultado.Results)
            {
                Ingrediente ingrediente = new Ingrediente();
                var dato = result.ToList();
                ingrediente.NombreIngrediente = dato[1].Value.ToString();
                ingrediente.IdIngrediente = int.Parse(dato[0].Value.ToString());


                lista.Add(ingrediente);

            }

            return View(lista);
        }

        public ActionResult ConsultarIngrediente(int indentificador)
        {
            if (indentificador == null)
            {
                return RedirectToAction("CatalogoComida", "Restaurante");
            }
            List<Ingrediente> listain = new List<Ingrediente>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
            "PREFIX rdf:<http://www.w3.org/1999/02/22-rdf-syntax-ns#>" +
            "PREFIX bd:<http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#>" +
            "SELECT  ?id_Ingrediente ?Nombreingredientes ?Nutrientes_I " +
            "WHERE {" +
            "?comida rdf:type bd:Comida." +
            "?comida bd:compuestapor ?ingrediente." +
             "?comida bd:IdComida ?idcomida." +
            "BIND(STR(?idcomida) AS ?Idcomida)." +
             "FILTER(?idcomida = " + indentificador + " )." +
            "?comida bd:NombreComida ?NOMBRE." +
            "?ingrediente bd:IdIngrediente ?idgre." +
            "BIND(STR(?idgre) AS ?id_Ingrediente)." +
            "?ingrediente bd:NombreIngrediente ?Nombreingredientes." +
            "?ingrediente bd:Nutrientes ?Nutrientes_I." +
            "}"
                );

            foreach (var result in resultado.Results)
            {
                Ingrediente ingrediente = new Ingrediente();

                ingrediente.IdIngrediente = Convert.ToInt32(result["id_Ingrediente"].ToString());
                ingrediente.NombreIngrediente = result["Nombreingredientes"].ToString();
                ingrediente.Nutrientes = result["Nutrientes_I"].ToString();
                listain.Add(ingrediente);
            }

            return View(listain);



        }
        public ActionResult Clientes()
        {
            List<Cliente> listaClientes = new List<Cliente>();
            SparqlResultSet resultado = endpoint.QueryWithResultSet(
            "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>" +
            "PREFIX bd: <http://www.semanticweb.org/user/ontologies/2023/4/OntologiaRestaurante#>" +
            "SELECT  (REPLACE(STR(?documento), '^\" |\"$', '') AS ?DOCUMENTO) " +
            "(CONCAT(?nombre, \" \", ?apellido) AS ?nombreCompleto) " +
            "?correo " +
            "?ocupacion " +
            "WHERE {" +
            "?empleados rdf:type bd:Cliente ." +
            "?empleados bd:Documento ?documento ." +
            "?empleados bd:Nombres ?nombre ." +
            "?empleados bd:Apellidos ?apellido ." +
            "?empleados bd:Correo ?correo ." +
            "?empleados bd:Ocupacion ?ocupacion ." +
            "}"
            );

            foreach (var result in resultado.Results)
            {
                Cliente cliente = new Cliente();
                cliente.documento = result["DOCUMENTO"].ToString();
                cliente.Nombres = result["nombreCompleto"].ToString();
                cliente.Correo = result["correo"].ToString();
                cliente.ocupacion = result["ocupacion"].ToString();

                listaClientes.Add(cliente);
            }

            return View(listaClientes);
        }

    }
}