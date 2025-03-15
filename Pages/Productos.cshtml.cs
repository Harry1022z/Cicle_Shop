using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

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
            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();

                // Cargar productos con su stock inicial
                var command = new MySqlCommand("SELECT p.ID_Producto, p.Nombre, p.Descripcion, p.Precio, p.Categoria, p.ImagenURL, s.Cantidad AS StockInicial FROM Productos p JOIN Stock s ON p.ID_Producto = s.ID_Producto", connection);
                var reader = command.ExecuteReader();
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
                        StockInicial = reader.GetInt32("StockInicial"), // Stock inicial
                        Cantidad = reader.GetInt32("StockInicial") // Stock disponible (se actualizará más adelante)
                    });
                }
                reader.Close();

                // Cargar trabajadores
                command = new MySqlCommand("SELECT ID_Trabajador, Nombre, Apellido FROM Trabajadores", connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Trabajadores.Add(new Trabajador
                    {
                        ID_Trabajador = reader.GetInt32("ID_Trabajador"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido")
                    });
                }
                reader.Close();

                // Cargar ventas y facturas si se selecciona un trabajador
                if (SelectedTrabajadorId > 0)
                {
                    // Cargar ventas del trabajador seleccionado
                    command = new MySqlCommand("SELECT ID_Venta, FechaVenta, TotalVenta FROM Ventas WHERE ID_Trabajador = @ID_Trabajador", connection);
                    command.Parameters.AddWithValue("@ID_Trabajador", SelectedTrabajadorId);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Ventas.Add(new Venta
                        {
                            ID_Venta = reader.GetInt32("ID_Venta"),
                            FechaVenta = reader.GetDateTime("FechaVenta"),
                            TotalVenta = reader.GetDecimal("TotalVenta")
                        });
                    }
                    reader.Close();

                    // Cargar facturas asociadas a las ventas del trabajador
                    command = new MySqlCommand("SELECT f.ID_Factura, f.Detalle FROM Facturas f JOIN Ventas v ON f.ID_Venta = v.ID_Venta WHERE v.ID_Trabajador = @ID_Trabajador", connection);
                    command.Parameters.AddWithValue("@ID_Trabajador", SelectedTrabajadorId);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Facturas.Add(new Factura
                        {
                            ID_Factura = reader.GetInt32("ID_Factura"),
                            Detalle = reader.GetString("Detalle")
                        });
                    }
                    reader.Close();

                    // Calcular el stock disponible en función de las ventas del trabajador
                    command = new MySqlCommand("SELECT dv.ID_Producto, SUM(dv.Cantidad) AS CantidadVendida FROM DetalleVenta dv JOIN Ventas v ON dv.ID_Venta = v.ID_Venta WHERE v.ID_Trabajador = @ID_Trabajador GROUP BY dv.ID_Producto", connection);
                    command.Parameters.AddWithValue("@ID_Trabajador", SelectedTrabajadorId);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var producto = Productos.FirstOrDefault(p => p.ID_Producto == reader.GetInt32("ID_Producto"));
                        if (producto != null)
                        {
                            producto.Cantidad -= reader.GetInt32("CantidadVendida"); // Actualizar el stock
                        }
                    }
                }
            }
        }
    }

    // Clase Producto
    public class Producto
    {
        public int ID_Producto { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenURL { get; set; }
        public int StockInicial { get; set; } // Stock inicial
        public int Cantidad { get; set; } // Stock disponible (se actualiza dinámicamente)
    }

    // Clase Trabajador
    public class Trabajador
    {
        public int ID_Trabajador { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }

    // Clase Venta
    public class Venta
    {
        public int ID_Venta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal TotalVenta { get; set; }
    }

    // Clase Factura
    public class Factura
    {
        public int ID_Factura { get; set; }
        public string? Detalle { get; set; }
    }
}