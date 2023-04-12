using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backendventas.Models;

public partial class NetventasContext : DbContext
{
    public DbSet<Products> Products { get; set; }
    public DbSet<Inventory> Inventory { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<SaleDetails> SaleDetails { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    //here the tables are created
    public NetventasContext()
    {
    }

    public NetventasContext(DbContextOptions<NetventasContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { 
    
    
    }
    // the connection string is in the appsettings.json file
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //      => optionsBuilder.UseMySql("server=localhost;port=3306;database=netventas;uid=root;password=sistemas123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Definir la relación entre las entidades Inventory y Product
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Products) // Una instancia de Inventory tiene una instancia de Product
            .WithMany(p => p.Inventories) // Una instancia de Product puede tener muchas instancias de Inventory
            .HasForeignKey(i => i.ProductId); // La propiedad ProductId de Inventory es la clave foránea que apunta a la propiedad Id de Product

        // Definir la clave primaria compuesta para la entidad SaleDetail
        modelBuilder.Entity<SaleDetails>()
            .HasKey(sd => new { sd.SaleId, sd.ProductId });

        // Definir la relación entre las entidades SaleDetail y Sale
        modelBuilder.Entity<SaleDetails>()
            .HasOne(sd => sd.Sales) // Una instancia de SaleDetail tiene una instancia de Sale
            .WithMany(s => s.SaleDetails) // Una instancia de Sale puede tener muchas instancias de SaleDetail
            .HasForeignKey(sd => sd.SaleId); // La propiedad SaleId de SaleDetail es la clave foránea que apunta a la propiedad Id de Sale

        // Definir la relación entre las entidades SaleDetail y Product
        modelBuilder.Entity<SaleDetails>()
            .HasOne(sd => sd.Products) // Una instancia de SaleDetail tiene una instancia de Product
            .WithMany(p => p.SalesDetails) // Una instancia de Product puede tener muchas instancias de SaleDetail
            .HasForeignKey(sd => sd.ProductId); // La propiedad ProductId de SaleDetail es la clave foránea que apunta a la propiedad Id de Product

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
