using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TiendaVelozWeb.Pages
{
    public class EditarProductoModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public Producto? Producto { get; set; }

        public void OnGet()
        {
            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT ID_Producto, Nombre, Descripcion, Precio, Categoria FROM Productos WHERE ID_Producto = @Id", connection);
                command.Parameters.AddWithValue("@Id", Id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Producto = new Producto
                    {
                        ID_Producto = reader.GetInt32("ID_Producto"),
                        Nombre = reader.GetString("Nombre"),
                        Descripcion = reader.GetString("Descripcion"),
                        Precio = reader.GetDecimal("Precio"),
                        Categoria = reader.GetString("Categoria")
                    };
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Productos SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Categoria = @Categoria WHERE ID_Producto = @Id", connection);
                if (Producto == null)
                {
                    throw new InvalidOperationException("Producto cannot be null.");
                }

                command.Parameters.AddWithValue("@Nombre", Producto.Nombre);
                command.Parameters.AddWithValue("@Descripcion", Producto.Descripcion);
                command.Parameters.AddWithValue("@Precio", Producto.Precio);
                command.Parameters.AddWithValue("@Categoria", Producto.Categoria);
                command.Parameters.AddWithValue("@Id", Producto.ID_Producto);
                command.ExecuteNonQuery();
            }

            return RedirectToPage("/Productos");
        }
    }
}