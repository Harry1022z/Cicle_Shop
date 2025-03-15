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
                var command = new MySqlCommand("DELETE FROM Productos WHERE ID_Producto = @Id", connection);
                command.Parameters.AddWithValue("@Id", Id);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Productos");
        }
    }
}