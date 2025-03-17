namespace TiendaVelozWeb.Models
{
    public class Trabajador
    {
        public int ID_Trabajador { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string ImagenURL { get; set; } = string.Empty;
    }
}