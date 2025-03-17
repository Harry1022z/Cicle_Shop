using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TiendaVelozWeb.Models;
using System.IO;

namespace TiendaVelozWeb.Pages
{
    public class AgregarProductoModel : PageModel
    {
        [BindProperty]
        public Producto NuevoProducto { get; set; } = new Producto();

        [BindProperty]
        public int StockInicial { get; set; } 

        [BindProperty]
        public IFormFile? Imagen { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var imagenUrl = Imagen != null ? GuardarImagen(Imagen) : string.Empty;

            using (var connection = new MySqlConnection("server=localhost;database=tienda_veloz;user=root;password=;"))
            {
                connection.Open();


                var command = new MySqlCommand("INSERT INTO Productos (Nombre, Descripcion, Precio, Categoria, ImagenURL) VALUES (@Nombre, @Descripcion, @Precio, @Categoria, @ImagenURL);", connection);
                command.Parameters.AddWithValue("@Nombre", NuevoProducto.Nombre);
                command.Parameters.AddWithValue("@Descripcion", NuevoProducto.Descripcion);
                command.Parameters.AddWithValue("@Precio", NuevoProducto.Precio);
                command.Parameters.AddWithValue("@Categoria", NuevoProducto.Categoria);
                command.Parameters.AddWithValue("@ImagenURL", imagenUrl);
                command.ExecuteNonQuery();


                var idProducto = (int)command.LastInsertedId;


                var stockCommand = new MySqlCommand("INSERT INTO Stock (ID_Producto, Cantidad, FechaActualizacion) VALUES (@ID_Producto, @Cantidad, NOW());", connection);
                stockCommand.Parameters.AddWithValue("@ID_Producto", idProducto);
                stockCommand.Parameters.AddWithValue("@Cantidad", StockInicial);
                stockCommand.ExecuteNonQuery();
            }

            return RedirectToPage("/Productos");
        }

        private string GuardarImagen(IFormFile imagen)
        {
            var rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var nombreUnico = Guid.NewGuid().ToString() + "_" + imagen.FileName;
            var rutaCompleta = Path.Combine(rutaImagenes, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                imagen.CopyTo(stream);
            }

            return "/images/" + nombreUnico;
        }
    }
}