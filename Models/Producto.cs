namespace TiendaVelozWeb.Models
{
    public class Producto
    {
        public int ID_Producto { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public required string Categoria { get; set; }
        public int Cantidad { get; set; }
    }
}