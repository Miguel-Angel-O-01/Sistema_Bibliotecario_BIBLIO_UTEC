namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{
    public class CarritoItem
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; } = string.Empty;      
        public string Isbn { get; set; } = string.Empty; public string Nao { get; set; } = string.Empty;
        // En bibliotecas usualmente se reserva 1 copia por libro, pero dejamos la estructura estándar
        public int Cantidad { get; set; } = 1;
    }

}
