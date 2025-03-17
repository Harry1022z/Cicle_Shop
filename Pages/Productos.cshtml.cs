using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TiendaVelozWeb.Models;

namespace TiendaVelozWeb.Pages
{
    public class ProductosModel : PageModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<Trabajador> Trabajadores { get; set; } = new List<Trabajador>();
        public List<Venta> Ventas { get; set; } = new List<Venta>();
        public List<Factura> Facturas { get; set; } = new List<Factura>();

        [BindProperty(SupportsGet = true)]
        public int SelectedTrabajadorId { get; set; }

        public void OnGet()
{
    try
    {
        using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
        {
            connection.Open();


            var command = new MySqlCommand("SELECT p.ID_Producto, p.Nombre, p.Descripcion, p.Precio, p.Categoria, p.ImagenURL, s.Cantidad AS StockInicial FROM Productos p JOIN Stock s ON p.ID_Producto = s.ID_Producto", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Productos.Add(new Producto
                    {
                        ID_Producto = reader.GetInt32("ID_Producto"),
                        Nombre = reader.GetString("Nombre"),
                        Descripcion = reader.GetString("Descripcion"),
                        Precio = reader.GetDecimal("Precio"),
                        Categoria = reader.GetString("Categoria"),
                        ImagenURL = reader.GetString("ImagenURL"),
                        StockInicial = reader.GetInt32("StockInicial"),
                        Cantidad = reader.GetInt32("StockInicial")
                    });
                }
            }


            command = new MySqlCommand("SELECT ID_Trabajador, Nombre, Apellido, ImagenURL FROM Trabajadores", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Trabajadores.Add(new Trabajador
                    {
                        ID_Trabajador = reader.GetInt32("ID_Trabajador"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        ImagenURL = reader.GetString("ImagenURL")
                    });
                }
            }


            if (SelectedTrabajadorId > 0)
            {
                command = new MySqlCommand("SELECT ID_Venta, FechaVenta, TotalVenta FROM Ventas WHERE ID_Trabajador = @ID_Trabajador", connection);
                command.Parameters.AddWithValue("@ID_Trabajador", SelectedTrabajadorId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ventas.Add(new Venta
                        {
                            ID_Venta = reader.GetInt32("ID_Venta"),
                            FechaVenta = reader.GetDateTime("FechaVenta"),
                            TotalVenta = reader.GetDecimal("TotalVenta")
                        });
                    }
                }


                command = new MySqlCommand("SELECT f.ID_Factura, f.Detalle FROM Facturas f JOIN Ventas v ON f.ID_Venta = v.ID_Venta WHERE v.ID_Trabajador = @ID_Trabajador", connection);
                command.Parameters.AddWithValue("@ID_Trabajador", SelectedTrabajadorId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Facturas.Add(new Factura
                        {
                            ID_Factura = reader.GetInt32("ID_Factura"),
                            Detalle = reader.GetString("Detalle")
                        });
                    }
                }


                command = new MySqlCommand("SELECT dv.ID_Producto, SUM(dv.Cantidad) AS CantidadVendida FROM DetalleVenta dv JOIN Ventas v ON dv.ID_Venta = v.ID_Venta WHERE v.ID_Trabajador = @ID_Trabajador GROUP BY dv.ID_Producto", connection);
                command.Parameters.AddWithValue("@ID_Trabajador", SelectedTrabajadorId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = Productos.FirstOrDefault(p => p.ID_Producto == reader.GetInt32("ID_Producto"));
                        if (producto != null)
                        {
                            producto.Cantidad -= reader.GetInt32("CantidadVendida");
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {

        Console.WriteLine($"Error: {ex.Message}");
    }
}
    }
}