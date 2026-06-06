using Microsoft.EntityFrameworkCore;
using Sistema_Bibliotecario_BIBLIO_UTEC.Models;


namespace Sistema_Bibliotecario_BIBLIO_UTEC.Data
{
    public class BiblioUtecContext : DbContext
    {
        // El constructor recibe las opciones de configuración (como la conexión a SQLite)
        public BiblioUtecContext(DbContextOptions<BiblioUtecContext> options) : base(options)
        {
        }

        // Mapeo de las Tablas en la Base de Datos
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Multa> Multas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configuración de precisión para el monto de la Multa (Evita advertencias en la migración)
            modelBuilder.Entity<Multa>()
                .Property(m => m.Monto)
                .HasConversion<double>(); // SQLite no tiene tipo Decimal puro, se convierte a Double automáticamente

            // Relación de muchos a muchos indirecta (Un Libro tiene muchos préstamos, un Usuario tiene muchos préstamos)
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(p => p.LibroId);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(p => p.UsuarioId);
        }
    }
}
