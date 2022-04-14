using System;
using Microsoft.EntityFrameworkCore;
namespace BotFFlowers
{
	
	public class ProductContext : DbContext
	{
		public ProductContext(DbContextOptions<ProductContext> options) : base(options)
		{
			
		}

		public DbSet<Product> Products { get; set; }
	}
}

