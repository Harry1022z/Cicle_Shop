using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TiendaVelozWeb.Pages
{
    public class EliminarProductoModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();


                var deleteDetalleVentaCommand = new MySqlCommand("DELETE FROM DetalleVenta WHERE ID_Producto = @Id", connection);
                deleteDetalleVentaCommand.Parameters.AddWithValue("@Id", Id);
                deleteDetalleVentaCommand.ExecuteNonQuery();


                var deleteStockCommand = new MySqlCommand("DELETE FROM Stock WHERE ID_Producto = @Id", connection);
                deleteStockCommand.Parameters.AddWithValue("@Id", Id);
                deleteStockCommand.ExecuteNonQuery();


                var deleteProductoCommand = new MySqlCommand("DELETE FROM Productos WHERE ID_Producto = @Id", connection);
                deleteProductoCommand.Parameters.AddWithValue("@Id", Id);
                deleteProductoCommand.ExecuteNonQuery();
            }

            return RedirectToPage("/Productos");
        }
    }
}