using Sistema_Bibliotecario_BIBLIO_UTEC.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{
    public class Carrito
    {
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();

        public void AgregarItem(Libro libro)
        {
            var item = Items.FirstOrDefault(i => i.LibroId == libro.Id);
            if (item == null)
            {
                Items.Add(new CarritoItem { LibroId = libro.Id, Titulo = libro.Titulo, Isbn = libro.Isbn });
            }
        }

        public void RemoverItem(int libroId)
        {
            Items.RemoveAll(i => i.LibroId == libroId);
        }

        public void Limpiar() => Items.Clear();
    }
}
