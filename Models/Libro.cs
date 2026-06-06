using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{

    // 1. Tabla de Libros (Datos importados de Google + Tu inventario físico)
    public class Libro
    {
        [Key]
        public int Id { get; set; } 
        public string Isbn { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? PortadaUrl { get; set; } = string.Empty;
        public string Sinopsis { get; set; } = string.Empty;
        public decimal Precio { get; set; } = decimal.Zero;

        // Control de inventario real 
        public int StockTotal { get; set; }      // Ej: 5 (Cuántos libros compraste en total)
        public int StockDisponible { get; set; } // Ej: 3 (Cuántos quedan libres en el estante)
        public string Categoria { get; internal set; } = string.Empty;
    

         
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
 }