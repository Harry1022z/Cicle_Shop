namespace TiendaVelozWeb.Models
{
    public class Producto
    {
        public int ID_Producto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string ImagenURL { get; set; } = string.Empty;
        public int StockInicial { get; set; }
        public int Cantidad { get; set; }
    }
}