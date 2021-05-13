using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CursoEFCore.Data
{
    public class ApplicationContext : DbContext
    {
        //private static readonly ILoggerFactory _logger = LoggerFactory.Create(p => p.AddConsole());
        private static readonly ILoggerFactory _logger;

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #region Conexão 1

            //optionsBuilder.UseSqlServer("Server=BRBRI1LT138\\SQLEXPRESS;Database=CursoEFCore;Trusted_Connection=True;");

            #endregion

            #region Conexão 2

            // Resiliência da conexão
            optionsBuilder
                .UseLoggerFactory(_logger)
                .EnableSensitiveDataLogging()
                .UseSqlServer("Server=BRBRI1LT138\\SQLEXPRESS;Database=CursoEFCore;Trusted_Connection=True;",

            //p => p.EnableRetryOnFailure() // Por default tenta reconectar 6x até completar 1 minuto
            p => p.EnableRetryOnFailure(
                maxRetryCount: 2,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null)
            .MigrationsHistoryTable("curso_MigrationsHistory") // Alterando o nome da tabela histórico de migrações de "__EFMigrationsHistory" para a tabela que informar no parâmetro
                );

            #endregion
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Exemplo 1

            //modelBuilder.Entity<Cliente>(p => {
            //    p.ToTable("Clientes");
            //    p.HasKey(p => p.Id);
            //    p.Property("Nome").HasColumnType("VARCHAR(80)").IsRequired();
            //    p.Property(p => p.Telefone).HasColumnType("CHAR(11)");
            //    p.Property(p => p.CEP).HasColumnType("CHAR(8)").IsRequired();
            //    p.Property(p => p.Estado).HasColumnType("CHAR(2)").IsRequired();
            //    p.Property(p => p.Cidade).HasMaxLength(60).IsRequired();

            //    p.HasIndex(i => i.Telefone).HasName("idx_cliente_telefone");
            //});

            #endregion

            #region Exemplo 2

            //modelBuilder.ApplyConfiguration(new Configurations.ClienteConfiguration());

            #endregion

            #region Exemplo 3

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            MapearPropriedadesEsquecidas(modelBuilder);

            #endregion
        }

        private void MapearPropriedadesEsquecidas(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));

                foreach (var property in properties)
                {
                    if (string.IsNullOrEmpty(property.GetColumnType())
                        && !property.GetMaxLength().HasValue)
                    {
                        //property.SetMaxLength(100);
                        property.SetColumnType("VARCHAR(100)");
                    }
                }
            }
        }
    }
}