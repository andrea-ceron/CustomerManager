﻿using CustomerManager.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CustomerManager.Repository
{
	public class ClientsDbContext(DbContextOptions<ClientsDbContext> options) : DbContext(options)
	{
		protected override void OnModelCreating(ModelBuilder mb)
		{

			mb.Entity<Customer>().HasKey(i => i.Id);
			mb.Entity<Customer>()
				.Property(i => i.Id)
				.ValueGeneratedOnAdd(); 
			mb.Entity<Customer>().HasMany(i => i.Invoices)
				.WithOne(p => p.Customer)
				.HasForeignKey(p => p.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);
			mb.Entity<Customer>().HasMany(i => i.Address)
				.WithOne(p => p.Customer)
				.HasForeignKey(p => p.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);
			
			mb.Entity<Invoice>().HasKey(i => i.Id);
			mb.Entity<Invoice>()
				.Property(i => i.Id)
				.ValueGeneratedOnAdd();
			mb.Entity<Invoice>().HasMany(i => i.ProductList)
				.WithOne(p => p.Invoice)
				.HasForeignKey(p => p.InvoiceId);

			mb.Entity<Address>().HasKey(i => i.Id);
			mb.Entity<Address>()
				.Property(i => i.Id)
				.ValueGeneratedOnAdd();
			mb.Entity<Address>().HasMany(i => i.Invoices)
				.WithOne(p => p.Address)
				.HasForeignKey(p => p.AddressId);

			mb.Entity<Product>().HasKey(i => i.Id);
			mb.Entity<Product>()
				.Property(r => r.Id)
				.ValueGeneratedNever();
			mb.Entity<Product>()
				.HasMany(i => i.InvoiceProduct)
				.WithOne(p => p.Product)
				.HasForeignKey(p => p.ProductId);

			mb.Entity<TransactionalOutbox>()
				.HasKey(p => p.Id);

			base.OnModelCreating(mb);


		}
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<Address> Addresses { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<InvoiceProducts> InvoiceProducts { get; set; }
		public DbSet<TransactionalOutbox> TransactionalOutboxes { get; set; }



	}

}
