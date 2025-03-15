using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TiendaVelozWeb.Pages
{
    public class AgregarProductoModel : PageModel
    {
        [BindProperty]
        public Producto NuevoProducto { get; set; } = new Producto();

        public void OnGet()
        {
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

                // Insertar el producto en la tabla Productos
                var command = new MySqlCommand("INSERT INTO Productos (Nombre, Descripcion, Precio, Categoria) VALUES (@Nombre, @Descripcion, @Precio, @Categoria);", connection);
                command.Parameters.AddWithValue("@Nombre", NuevoProducto.Nombre);
                command.Parameters.AddWithValue("@Descripcion", NuevoProducto.Descripcion);
                command.Parameters.AddWithValue("@Precio", NuevoProducto.Precio);
                command.Parameters.AddWithValue("@Categoria", NuevoProducto.Categoria);
                command.ExecuteNonQuery();

                // Obtener el ID del producto recién insertado
                var idProducto = (int)command.LastInsertedId;

                // Insertar el stock inicial en la tabla Stock
                var stockCommand = new MySqlCommand("INSERT INTO Stock (ID_Producto, Cantidad, FechaActualizacion) VALUES (@ID_Producto, @Cantidad, NOW());", connection);
                stockCommand.Parameters.AddWithValue("@ID_Producto", idProducto);
                stockCommand.Parameters.AddWithValue("@Cantidad", 0); // Stock inicial
                stockCommand.ExecuteNonQuery();
            }

            return RedirectToPage("/Productos");
        }
    }
}