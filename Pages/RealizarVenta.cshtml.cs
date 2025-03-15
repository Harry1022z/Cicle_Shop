using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TiendaVelozWeb.Pages
{
    public class RealizarVentaModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int TrabajadorId { get; set; }

        [BindProperty]
        public int ProductoId { get; set; }

        [BindProperty]
        public int Cantidad { get; set; }

        public List<Producto> Productos { get; set; } = new List<Producto>();

        public void OnGet()
        {
            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT ID_Producto, Nombre, Cantidad FROM Productos JOIN Stock ON Productos.ID_Producto = Stock.ID_Producto", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Productos.Add(new Producto
                    {
                        ID_Producto = reader.GetInt32("ID_Producto"),
                        Nombre = reader.GetString("Nombre"),
                        Cantidad = reader.GetInt32("Cantidad")
                    });
                }
            }
        }

        public IActionResult OnPost()
        {
            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();

                // Actualizar el stock
                var updateStockCommand = new MySqlCommand("UPDATE Stock SET Cantidad = Cantidad - @Cantidad WHERE ID_Producto = @ProductoId", connection);
                updateStockCommand.Parameters.AddWithValue("@Cantidad", Cantidad);
                updateStockCommand.Parameters.AddWithValue("@ProductoId", ProductoId);
                updateStockCommand.ExecuteNonQuery();

                // Registrar la venta
                var insertVentaCommand = new MySqlCommand("INSERT INTO Ventas (ID_Trabajador, FechaVenta, TotalVenta) VALUES (@TrabajadorId, NOW(), (SELECT Precio * @Cantidad FROM Productos WHERE ID_Producto = @ProductoId))", connection);
                insertVentaCommand.Parameters.AddWithValue("@TrabajadorId", TrabajadorId);
                insertVentaCommand.Parameters.AddWithValue("@ProductoId", ProductoId);
                insertVentaCommand.Parameters.AddWithValue("@Cantidad", Cantidad);
                insertVentaCommand.ExecuteNonQuery();
            }

            return RedirectToPage("/Productos", new { SelectedTrabajadorId = TrabajadorId });
        }
    }
}